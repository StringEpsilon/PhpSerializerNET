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
					return token.Value == "1" ? true : false;
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
				default:
					throw new Exception("Unsupported datatype.");
			}
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
					if (targetType.IsIConvertible()) {
						if (targetType == typeof(bool)) {
							if (_options.NumberStringToBool && (token.Value == "0" || token.Value == "1")) {
								return token.ToBool();
							}
						}
						return ((IConvertible)token.Value).ToType(targetType, CultureInfo.InvariantCulture);
					} else {
						throw new DeserializationException(
							$"Can not assign value \"{token.Value}\" (at position {token.Position}) to target type of {targetType.Name}."
						);
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
					throw new Exception("Unsupported datatype.");
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
				field.SetValue(result, this.DeserializeToken(field.FieldType, valueToken));
			}
			return result;
		}


		private object MakeObject(Type targetType, PhpSerializeToken token) {
			var result = targetType.GetConstructor(new Type[0]).Invoke(null);
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
				property.SetValue(result, this.DeserializeToken(property.PropertyType, valueToken));
			}
			return result;
		}

		private object MakeList(Type targetType, PhpSerializeToken token) {
			var result = targetType.GetConstructor(new Type[0]).Invoke(null) as IList;
			Type itemType = typeof(object);
			if (targetType.GenericTypeArguments.Length >= 1) {
				itemType = targetType.GenericTypeArguments[0];
			}

			// Don't use the options for normal list serialization. Just check that the keys are all integers:
			for (int i = 0; i < token.Children.Count; i += 2) {
				if (token.Children[i].Type != PhpSerializerType.Integer) {
					throw new DeserializationException(
						$"Can not deserialize array at position {token.Position} to list: " +
						$"It has a non-integer key at element {i} (position {token.Children[i].Position})."
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
			var result = targetType.GetConstructor(new Type[0]).Invoke(null) as IDictionary;
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