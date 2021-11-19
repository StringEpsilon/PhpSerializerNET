/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace PhpSerializerNET {
	internal class PhpDeserializer {
		private PhpDeserializationOptions _options;
		private List<PhpSerializeToken> _tokens;
		private static Dictionary<string, Type> TypeLookupCache = new(){
			{ "DateTime", typeof(PhpDateTime) }
		};

		public PhpDeserializer(string input, PhpDeserializationOptions options) {
			this._options = options;
			if (this._options == null) {
				this._options = PhpDeserializationOptions.DefaultOptions;
			}
			this._tokens = new PhpTokenizer(input, _options).Tokenize();

			if (_tokens.Count > 1) {
				throw new DeserializationException("Can not deserialize loose collection of values into object");
			}
			if (_tokens.Count == 0) {
				throw new DeserializationException("No PHP serialization data found.");
			}
		}

		public object Deserialize() {
			return this.DeserializeToken(_tokens[0]);
		}

		public T Deserialize<T>() {
			Type targetType = typeof(T);
			return (T)this.DeserializeToken(targetType, _tokens[0]);
		}

		private object DeserializeToken(PhpSerializeToken token) {
			switch (token.Type) {
				case PhpSerializerType.Null:
					return null;
				case PhpSerializerType.Boolean:
					return token.ToBool();
				case PhpSerializerType.Integer:
					return token.ToLong();
				case PhpSerializerType.Floating:
					return token.ToDouble();
				case PhpSerializerType.String:
					if (_options.NumberStringToBool && (token.Value == "0" || token.Value == "1")) {
						return token.ToBool();
					}
					return token.Value;
				case PhpSerializerType.Array:
					return this.MakeCollection(token);
				case PhpSerializerType.Object:
					return this.MakeClass(token);
				default:
					throw new Exception("Unsupported datatype.");
			}
		}

		private object MakeClass(PhpSerializeToken token) {
			var typeName = token.Value;
			object constructedObject;
			Type targetType = null;
			if (typeName != "sdtClass" && _options.EnableTypeLookup) {
				if (TypeLookupCache.ContainsKey(typeName)) {
					targetType = TypeLookupCache[typeName];
				} else {
					foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic)) {
						// TODO: PhpClass attribute should win over classes who happen to have the name...?
						targetType = assembly.GetExportedTypes()
							.Where(y => y.Name == typeName || y.GetCustomAttribute<PhpClass>()?.Name == typeName)
							.FirstOrDefault();
						if (targetType != null) {
							TypeLookupCache.Add(typeName, targetType);
							break;
						}
					}
				}
			}
			if (targetType != null && typeName != "stdClass") {

				constructedObject = DeserializeToken(targetType, token);
			} else {
				dynamic result;
				if (this._options.StdClass == StdClassOption.Dynamic) {
					result = new PhpDynamicObject();
				} else if (_options.StdClass == StdClassOption.Dictionary) {
					result = new PhpObjectDictionary();
				} else {
					throw new DeserializationException("Encountered 'stdClass' and the behavior 'Throw' was specified in deserialization options.");
				}
				for (int i = 0; i < token.Children.Count; i += 2) {
					result.TryAdd(
						token.Children[i].Value,
						DeserializeToken(token.Children[i + 1])
					);
				}
				constructedObject = result;
			}
			if (constructedObject is IPhpObject phpObject){
				phpObject.SetClassName(typeName);
			}
			return constructedObject;
		}

		private object DeserializeToken(Type targetType, PhpSerializeToken token) {
			switch (token.Type) {
				case PhpSerializerType.Null:
					if (targetType.IsValueType) {
						return Activator.CreateInstance(targetType);
					} else {
						return null;
					}
				case PhpSerializerType.Boolean:
					if (targetType.IsIConvertible()) {
						return ((IConvertible)token.ToBool()).ToType(targetType, CultureInfo.InvariantCulture);
					} else {
						throw new DeserializationException(
							$"Can not assign value \"{token.Value}\" (at position {token.Position}) to target type of {targetType.Name}."
						);
					}
				case PhpSerializerType.Integer:
				case PhpSerializerType.Floating:
				case PhpSerializerType.String:
					// Short-circuit strings:
					if (targetType == typeof(string)) {
						return token.Value;
					}

					if (targetType.IsEnum){
						if (token.Type != PhpSerializerType.String){
							return ((IConvertible)token.Value).ToType(targetType.GetEnumUnderlyingType(), CultureInfo.InvariantCulture);
						} else {
							return targetType
								.GetFields()
								.FirstOrDefault(y => y.Name == token.Value)
								.GetRawConstantValue();
						}
					}
					if (targetType.IsIConvertible()) {
						if (token.Value == "" && this._options.EmptyStringToDefault){
							return Activator.CreateInstance(targetType);
						}
						if (targetType == typeof(bool)) {
							if (_options.NumberStringToBool && (token.Value == "0" || token.Value == "1")) {
								return token.ToBool();
							}
						}
						return ((IConvertible)token.Value).ToType(targetType, CultureInfo.InvariantCulture);
					} else if(targetType == typeof(System.Guid)) {
						return new Guid(token.Value);
					} else {
						throw new DeserializationException(
							$"Can not assign value \"{token.Value}\" (at position {token.Position}) to target type of {targetType.Name}."
						);
					}
				case PhpSerializerType.Object: {
						if (typeof(IDictionary).IsAssignableFrom(targetType)) {
							return this.MakeDictionary(targetType, token);
						} else if (targetType.IsClass) {
							return this.MakeObject(targetType, token);
						} else {
							return this.MakeStruct(targetType, token);
						}
					}
				case PhpSerializerType.Array:
					if (typeof(IList).IsAssignableFrom(targetType)) {
						return this.MakeList(targetType, token);
					} else if (typeof(IDictionary).IsAssignableFrom(targetType)) {
						return this.MakeDictionary(targetType, token);
					} else if (targetType.IsClass) {
						return this.MakeObject(targetType, token);
					} else {
						return this.MakeStruct(targetType, token);
					}
				default:
					throw new DeserializationException($"Unsupported datatype {targetType.Name}.");
			}
		}

		private object MakeStruct(Type targetType, PhpSerializeToken token) {
			var result = Activator.CreateInstance(targetType);
			var targetFields = targetType.GetFields();

			for (int i = 0; i < token.Children.Count; i += 2) {
				var fieldName = token.Children[i].Value;
				var valueToken = token.Children[i + 1];

				var field = targetFields.FindField(fieldName, _options) as FieldInfo;

				if (field == null) {
					if (!_options.AllowExcessKeys) {
						throw new DeserializationException(
							$"Could not bind the key \"{fieldName}\" to struct of type {targetType.Name}: No such field."
						);
					}
					break;
				}
				if (field.GetCustomAttribute<PhpIgnoreAttribute>() != null) {
					break;
				}
				try{
					field.SetValue(result, this.DeserializeToken(field.FieldType, valueToken));
				} catch(Exception exception){
					throw new DeserializationException(
						$"Exception encountered while trying to assign '{token.Value}' to {targetType.Name}.{field.Name}. See inner exception for details.",
						exception
					);
				}
			}
			return result;
		}


		private object MakeObject(Type targetType, PhpSerializeToken token) {
			var result = Activator.CreateInstance(targetType);
			var targetProperties = targetType.GetProperties();

			for (int i = 0; i < token.Children.Count; i += 2) {
				var propertyName = token.Children[i].Value;
				var valueToken = token.Children[i + 1];

				var property = targetProperties.FindProperty(propertyName, _options) as PropertyInfo;

				if (property == null) {
					if (!_options.AllowExcessKeys) {
						throw new DeserializationException(
							$"Could not bind the key \"{propertyName}\" to object of type {targetType.Name}: No such property."
						);
					}
					break;
				}
				if (property.GetCustomAttribute<PhpIgnoreAttribute>() != null) {
					break;
				}
				try{
					property.SetValue(result, this.DeserializeToken(property.PropertyType, valueToken));
				} catch(Exception exception){
					throw new DeserializationException(
						$"Exception encountered while trying to assign '{token.Value}' to {targetType.Name}.{property.Name}. See inner exception for details.",
						exception
					);
				}
			}
			return result;
		}

		private object MakeList(Type targetType, PhpSerializeToken token) {
			var result = (IList)Activator.CreateInstance(targetType);
			Type itemType = typeof(object);
			if (targetType.GenericTypeArguments.Length >= 1) {
				itemType = targetType.GenericTypeArguments[0];
			}

			// Don't use the options for normal list serialization. Just check that the keys are all integers:
			for (int i = 0; i < token.Children.Count; i += 2) {
				if (token.Children[i].Type != PhpSerializerType.Integer) {
					throw new DeserializationException(
						$"Can not deserialize array at position {token.Position} to list: " +
						$"It has a non-integer key '{token.Value}' at element {i} (position {token.Children[i].Position})."
					);
				}
			}

			for (int i = 1; i < token.Children.Count; i += 2) {
				result.Add(
					itemType == typeof(object)
						? this.DeserializeToken(token.Children[i])
						: this.DeserializeToken(itemType, token.Children[i])
				);
			}
			return result;
		}

		private object MakeDictionary(Type targetType, PhpSerializeToken token) {
			var result = (IDictionary)Activator.CreateInstance(targetType);
			if (targetType.GenericTypeArguments.Count() == 0) {
				for (int i = 0; i < token.Children.Count; i += 2) {
					var keyToken = token.Children[i];
					var valueToken = token.Children[i + 1];
					result.Add(
						this.DeserializeToken(keyToken),
						this.DeserializeToken(valueToken)
					);
				}
				return result;
			}
			Type keyType = targetType.GenericTypeArguments[0];
			Type valueType = targetType.GenericTypeArguments[1];

			for (int i = 0; i < token.Children.Count; i += 2) {
				var keyToken = token.Children[i];
				var valueToken = token.Children[i + 1];
				result.Add(
					keyType == typeof(object)
						? this.DeserializeToken(keyToken)
						: this.DeserializeToken(keyType, keyToken),
					valueType == typeof(object)
						? this.DeserializeToken(valueToken)
						: this.DeserializeToken(valueType, valueToken)
				);
			}
			return result;
		}

		private object MakeCollection(PhpSerializeToken token) {
			if (_options.UseLists == ListOptions.Never) {
				return this.MakeDictionary(typeof(Dictionary<object, object>), token);
			}
			long previousKey = -1;
			bool isList = true;
			bool consecutive = true;
			for (int i = 0; i < token.Children.Count; i += 2) {
				if (token.Children[i].Type != PhpSerializerType.Integer) {
					isList = false;
					break;
				} else {
					var key = token.Children[i].ToLong();
					if (i == 0 || key == previousKey + 1) {
						previousKey = key;
					} else {
						consecutive = false;
					}
				}
			}
			if (!isList || (_options.UseLists == ListOptions.Default && consecutive == false)) {
				return this.MakeDictionary(typeof(Dictionary<object, object>), token);
			}
			return this.MakeList(typeof(List<object>), token);
		}
	}
}