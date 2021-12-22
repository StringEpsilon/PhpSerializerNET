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
		private readonly PhpDeserializationOptions _options;
		private PhpSerializeToken _token;
		private static readonly Dictionary<string, Type> TypeLookupCache = new() {
			{ "DateTime", typeof(PhpDateTime) }
		};

		private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> PropertyInfoCache = new();

		private static Dictionary<Type, Dictionary<string, FieldInfo>> FieldInfoCache { get; set; } = new();

		public PhpDeserializer(string input, PhpDeserializationOptions options) {
			_options = options;
			if (_options == null) {
				_options = PhpDeserializationOptions.DefaultOptions;
			}
			this._token = new PhpTokenizer(input, this._options.InputEncoding).Tokenize();
		}

		public object Deserialize() {
			return this.DeserializeToken(this._token);
		}

		public object Deserialize(Type targetType) {
			return this.DeserializeToken(targetType, this._token);
		}

		public T Deserialize<T>() {
			return (T) this.Deserialize(typeof(T));
		}

		/// <summary>
		/// Reset the type lookup cache.
		/// Can be useful for scenarios in which new types are loaded at runtime in between deserialization tasks.
		/// </summary>
		public static void ClearTypeCache() {
			TypeLookupCache.Clear();
			TypeLookupCache.Add("DateTime", typeof(PhpDateTime));
		}

		/// <summary>
		/// Reset the property info cache.
		/// Can be useful for scenarios in which new types are loaded at runtime in between deserialization tasks.
		/// </summary>
		public static void ClearPropertyInfoCache() {
			PropertyInfoCache.Clear();
		}

		private object DeserializeToken(PhpSerializeToken token) {
			switch (token.Type) {
				case PhpSerializerType.Boolean:
					return token.ToBool();
				case PhpSerializerType.Integer:
					return token.ToLong();
				case PhpSerializerType.Floating:
					return token.ToDouble();
				case PhpSerializerType.String:
					if (this._options.NumberStringToBool && (token.Value == "0" || token.Value == "1")) {
						return token.ToBool();
					}
					return token.Value;
				case PhpSerializerType.Array:
					return MakeCollection(token);
				case PhpSerializerType.Object:
					return MakeClass(token);
				case PhpSerializerType.Null:
				default:
					return null;
			}
		}

		private object MakeClass(PhpSerializeToken token) {
			var typeName = token.Value;
			object constructedObject;
			Type targetType = null;
			if (typeName != "sdtClass" && this._options.EnableTypeLookup) {
				if (TypeLookupCache.ContainsKey(typeName)) {
					targetType = TypeLookupCache[typeName];
				} else {
					foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic)) {
						// TODO: PhpClass attribute should win over classes who happen to have the name...?
						targetType = assembly
							.GetExportedTypes()
							.FirstOrDefault(y => y.Name == typeName || y.GetCustomAttribute<PhpClass>()?.Name == typeName);
						if (targetType != null) {
							break;
						}
					}
					if (this._options.TypeCache.HasFlag(TypeCacheFlag.ClassNames)) {
						TypeLookupCache.Add(typeName, targetType);
					}
				}
			}
			if (targetType != null && typeName != "stdClass") {
				constructedObject = this.DeserializeToken(targetType, token);
			} else {
				dynamic result;
				if (_options.StdClass == StdClassOption.Dynamic) {
					result = new PhpDynamicObject();
				} else if (this._options.StdClass == StdClassOption.Dictionary) {
					result = new PhpObjectDictionary();
				} else {
					throw new DeserializationException("Encountered 'stdClass' and the behavior 'Throw' was specified in deserialization options.");
				}
				for (int i = 0; i < token.Children.Length; i += 2) {
					result.TryAdd(
						token.Children[i].Value,
						this.DeserializeToken(token.Children[i + 1])
					);
				}
				constructedObject = result;
			}
			if (constructedObject is IPhpObject phpObject and not PhpDateTime) {
				phpObject.SetClassName(typeName);
			}
			return constructedObject;
		}

		private object DeserializeToken(Type targetType, PhpSerializeToken token) {
			if (targetType == null) {
				throw new ArgumentNullException(nameof(targetType));
			}

			switch (token.Type) {
				case PhpSerializerType.Boolean: {
						return DeserializeBoolean(targetType, token);
					}
				case PhpSerializerType.Integer:
					return DeserializeInteger(targetType, token);
				case PhpSerializerType.Floating:
					return DeserializeDouble(targetType, token);
				case PhpSerializerType.String:
					return DeserializeTokenFromSimpleType(targetType, token);
				case PhpSerializerType.Object: {
						object result;
						if (typeof(IDictionary).IsAssignableFrom(targetType)) {
							result = MakeDictionary(targetType, token);
						} else if (targetType.IsClass) {
							result = MakeObject(targetType, token);
						} else {
							result = MakeStruct(targetType, token);
						}
						if (result is IPhpObject phpObject and not PhpDateTime) {
							phpObject.SetClassName(token.Value);
						}
						return result;
					}
				case PhpSerializerType.Array: {
						if (targetType.IsAssignableTo(typeof(IList))) {
							return this.MakeList(targetType, token);
						} else if (targetType.IsAssignableTo(typeof(IDictionary))) {
							return this.MakeDictionary(targetType, token);
						} else if (targetType.IsClass) {
							return this.MakeObject(targetType, token);
						} else {
							return this.MakeStruct(targetType, token);
						}
					}
				case PhpSerializerType.Null:
				default:
					if (targetType.IsValueType) {
						return Activator.CreateInstance(targetType);
					} else {
						return null;
					}
			}
		}

		private object DeserializeInteger(Type targetType, PhpSerializeToken token) {
			return Type.GetTypeCode(targetType) switch {
				TypeCode.Int16 => short.Parse(token.Value),
				TypeCode.Int32 => int.Parse(token.Value),
				TypeCode.Int64 => long.Parse(token.Value),
				TypeCode.UInt16 => ushort.Parse(token.Value),
				TypeCode.UInt32 => uint.Parse(token.Value),
				TypeCode.UInt64 => ulong.Parse(token.Value),
				TypeCode.SByte => sbyte.Parse(token.Value),
				_ => this.DeserializeTokenFromSimpleType(targetType, token),
			};
		}

		private object DeserializeDouble(Type targetType, PhpSerializeToken token) {
			if (targetType == typeof(double) || targetType == typeof(float)) {
				return token.ToDouble();
			};
			token.Value = token.Value switch {
				"INF" => double.PositiveInfinity.ToString(CultureInfo.InvariantCulture),
				"-INF" => double.NegativeInfinity.ToString(CultureInfo.InvariantCulture),
				_ => token.Value,
			};
			return this.DeserializeTokenFromSimpleType(targetType, token);
		}

		private static object DeserializeBoolean(Type targetType, PhpSerializeToken token) {
			if (targetType == typeof(bool) || targetType == typeof(bool?)) {
				return token.ToBool();
			}
			Type underlyingType = targetType;
			if (targetType.IsNullableReferenceType()) {
				underlyingType = targetType.GenericTypeArguments[0];
			}

			if (underlyingType.IsIConvertible()) {
				return ((IConvertible)token.ToBool()).ToType(underlyingType, CultureInfo.InvariantCulture);
			} else {
				throw new DeserializationException(
					$"Can not assign value \"{token.Value}\" (at position {token.Position}) to target type of {targetType.Name}."
				);
			}
		}

		private object DeserializeTokenFromSimpleType(Type givenType, PhpSerializeToken token) {
			var targetType = givenType;
			if (!targetType.IsPrimitive && targetType.IsNullableReferenceType()) {
				if (token.Value == "" && _options.EmptyStringToDefault) {
					return null;
				}

				targetType = targetType.GenericTypeArguments[0];
				if (targetType == null) {
					throw new NullReferenceException("Could not get underlying type for nullable reference type " + givenType);
				}
			}

			// Short-circuit strings:
			if (targetType == typeof(string)) {
				return token.Value == "" && _options.EmptyStringToDefault
					? default
					: token.Value;
			}

			if (targetType.IsEnum) {
				// Enums are converted by name if the token is a string and by underlying value if they are not

				if (token.Type != PhpSerializerType.String) {
					return Enum.Parse(targetType, token.Value);
				}
				var foundFieldInfo = targetType
					.GetFields()
					.FirstOrDefault(y => y.Name == token.Value);

				if (foundFieldInfo == null) {
					if (_options.EmptyStringToDefault) {
						return Activator.CreateInstance(targetType);
					} else {
						throw new DeserializationException(
							$"Exception encountered while trying to assign '{token.Value}' to type '{targetType.Name}'. " +
							$"The value could not be matched to an enum member.");
					}
				}

				return foundFieldInfo.GetRawConstantValue();
			}

			if (targetType.IsIConvertible()) {
				if (token.Value == "" && _options.EmptyStringToDefault) {
					return Activator.CreateInstance(targetType);
				}

				if (targetType == typeof(bool)) {
					if (_options.NumberStringToBool && token.Value is "0" or "1") {
						return token.ToBool();
					}
				}

				try {
					return ((IConvertible)token.Value).ToType(targetType, CultureInfo.InvariantCulture);
				} catch (Exception exception) {
					throw new DeserializationException(
						$"Exception encountered while trying to assign '{token.Value}' to type {targetType.Name}. See inner exception for details.",
						exception
					);
				}
			}

			if (targetType == typeof(Guid)) {
				return token.Value == "" && _options.EmptyStringToDefault
					? default
					: new Guid(token.Value);
			}

			if (targetType == typeof(object)) {
				return token.Value == "" && _options.EmptyStringToDefault
					? default
					: token.Value;
			}

			throw new DeserializationException($"Can not assign value \"{token.Value}\" (at position {token.Position}) to target type of {targetType.Name}.");
		}

		private object MakeStruct(Type targetType, PhpSerializeToken token) {
			var result = Activator.CreateInstance(targetType);
			Dictionary<string, FieldInfo> fields;
			if (FieldInfoCache.ContainsKey(targetType)) {
				fields = FieldInfoCache[targetType];
			} else {
				fields = targetType.GetFields().GetAllFields(this._options);
				if (this._options.TypeCache.HasFlag(TypeCacheFlag.PropertyInfo)) {
					FieldInfoCache.Add(targetType, fields);
				}
			}

			for (int i = 0; i < token.Children.Length; i += 2) {
				var fieldName = this._options.CaseSensitiveProperties ? token.Children[i].Value : token.Children[i].Value.ToLower();
				var valueToken = token.Children[i + 1];
				if (!fields.ContainsKey(fieldName)) {
					if (!this._options.AllowExcessKeys) {
						throw new DeserializationException(
							$"Could not bind the key \"{token.Children[i].Value}\" to struct of type {targetType.Name}: No such field."
						);
					}
					break;
				}
				if (fields[fieldName] != null) {
					var field = fields[fieldName];
					try {
						field.SetValue(result, DeserializeToken(field.FieldType, valueToken));
					} catch (Exception exception) {
						throw new DeserializationException(
							$"Exception encountered while trying to assign '{valueToken.Value}' to {targetType.Name}.{field.Name}. " +
							"See inner exception for details.",
							exception
						);
					}
				}
			}
			return result;
		}

		private object MakeObject(Type targetType, PhpSerializeToken token) {
			var result = Activator.CreateInstance(targetType);
			Dictionary<string, PropertyInfo> properties;
			if (PropertyInfoCache.ContainsKey(targetType)) {
				properties = PropertyInfoCache[targetType];
			} else {
				properties = targetType.GetProperties().GetAllProperties(this._options);
				if (this._options.TypeCache.HasFlag(TypeCacheFlag.PropertyInfo)) {
					PropertyInfoCache.Add(targetType, properties);
				}
			}

			for (int i = 0; i < token.Children.Length; i += 2) {
				var propertyName = this._options.CaseSensitiveProperties ? token.Children[i].Value : token.Children[i].Value.ToLower();
				var valueToken = token.Children[i + 1];

				if (!properties.ContainsKey(propertyName)) {
					if (!this._options.AllowExcessKeys) {
						throw new DeserializationException(
							$"Could not bind the key \"{token.Children[i].Value}\" to object of type {targetType.Name}: No such property."
						);
					}
					break;
				}
				var property = properties[propertyName];
				if (property != null) { // null if PhpIgnore'd
					try {
						property.SetValue(
							result,
							DeserializeToken(property.PropertyType, valueToken)
						);
					} catch (Exception exception) {
						throw new DeserializationException(
							$"Exception encountered while trying to assign '{valueToken.Value}' to {targetType.Name}.{property.Name}. See inner exception for details.",
							exception
						);
					}
				}
			}
			return result;
		}

		private object MakeArray(Type targetType, PhpSerializeToken token) {
			var elementType = targetType.GetElementType();
			Array result = System.Array.CreateInstance(elementType, token.Children.Length / 2);

			var arrayIndex = 0;
			for (int i = 1; i < token.Children.Length; i += 2) {
				result.SetValue(
					elementType == typeof(object)
						? DeserializeToken(token.Children[i])
						: DeserializeToken(elementType, token.Children[i]),
					arrayIndex
				);
				arrayIndex++;
			}
			return result;
		}

		private object MakeList(Type targetType, PhpSerializeToken token) {
			for (int i = 0; i < token.Children.Length; i += 2) {
				if (token.Children[i].Type != PhpSerializerType.Integer) {
					throw new DeserializationException(
						$"Can not deserialize array at position {token.Position} to list: " +
						$"It has a non-integer key '{token.Children[i].Value}' at element {i} (position {token.Children[i].Position})."
					);
				}
			}

			if (targetType.IsArray) {
				return MakeArray(targetType, token);
			}
			var result = (IList)Activator.CreateInstance(targetType);
			if (result == null) {
				throw new NullReferenceException("Activator.CreateInstance(targetType) returned null");
			}
			Type itemType = typeof(object);
			if (targetType.GenericTypeArguments.Length >= 1) {
				itemType = targetType.GenericTypeArguments[0];
			}

			for (int i = 1; i < token.Children.Length; i += 2) {
				result.Add(
					itemType == typeof(object)
						? DeserializeToken(token.Children[i])
						: DeserializeToken(itemType, token.Children[i])
				);
			}
			return result;
		}

		private object MakeDictionary(Type targetType, PhpSerializeToken token) {
			var result = (IDictionary)Activator.CreateInstance(targetType);
			if (result == null) {
				throw new NullReferenceException($"Activator.CreateInstance({targetType.FullName}) returned null");
			}
			if (!targetType.GenericTypeArguments.Any()) {
				for (int i = 0; i < token.Children.Length; i += 2) {
					var keyToken = token.Children[i];
					var valueToken = token.Children[i + 1];
					result.Add(
						DeserializeToken(keyToken),
						DeserializeToken(valueToken)
					);
				}
				return result;
			}
			Type keyType = targetType.GenericTypeArguments[0];
			Type valueType = targetType.GenericTypeArguments[1];

			for (int i = 0; i < token.Children.Length; i += 2) {
				var keyToken = token.Children[i];
				var valueToken = token.Children[i + 1];
				result.Add(
					keyType == typeof(object)
						? DeserializeToken(keyToken)
						: DeserializeToken(keyType, keyToken),
					valueType == typeof(object)
						? DeserializeToken(valueToken)
						: DeserializeToken(valueType, valueToken)
				);
			}
			return result;
		}

		private object MakeCollection(PhpSerializeToken token) {
			if (this._options.UseLists == ListOptions.Never) {
				return this.MakeDictionary(typeof(Dictionary<object, object>), token);
			}
			long previousKey = -1;
			bool isList = true;
			bool consecutive = true;
			for (int i = 0; i < token.Children.Length; i += 2) {
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
			if (!isList || (this._options.UseLists == ListOptions.Default && consecutive == false)) {
				var result = new Dictionary<object, object>();
				for (int i = 0; i < token.Children.Length; i += 2) {
					result.Add(
						this.DeserializeToken(token.Children[i]),
						this.DeserializeToken(token.Children[i + 1])
					);
				}
				return result;
			} else {
				var result = new List<object>();
				for (int i = 1; i < token.Children.Length; i += 2) {
					result.Add(this.DeserializeToken(token.Children[i]));
				}
				return result;
			}
		}
	}
}