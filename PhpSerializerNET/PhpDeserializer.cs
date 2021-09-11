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

		public PhpDeserializer(PhpDeserializationOptions options, List<PhpSerializeToken> tokens) {
			this._options = options;
			this._tokens = tokens;
		}

		public object Deserialize() {
			if (this._tokens.Count == 1) {
				return this.DeserializeToken(_tokens[0]);
			} else {
				return this._tokens.Select(x => this.DeserializeToken(x)).ToList();
			}
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
					return token.Value;
				case PhpSerializerType.Array:
					return token.ToCollection(_options);
				default:
					throw new Exception("Unsupported datatype.");
			}
		}

		private object DeserializeToken(Type targetType, PhpSerializeToken token) {
			switch (token.Type) {
				case PhpSerializerType.Null:
					if (targetType.IsValueType) {
						return Activator.CreateInstance(targetType);
					}
					if (targetType.IsClass) {
						return null;
					} else {
						throw new DeserializationException(
							$"Can not assign null (at position {token.Position}) to target type of {targetType.Name}."
						);
					}
				case PhpSerializerType.Boolean:
					if (targetType.IsIConvertible()) {
						return ((IConvertible)token.ToBool()).ToType(targetType, CultureInfo.InvariantCulture);
					} else {
						throw new DeserializationException(
							$"Can not assign value {token.Value} (at position {token.Position}) to target type of {targetType.Name}."
						);
					}
				case PhpSerializerType.Integer:
				case PhpSerializerType.Floating:
				case PhpSerializerType.String:
					if (targetType.IsIConvertible()) {
						return ((IConvertible)token.Value).ToType(targetType, CultureInfo.InvariantCulture);
					} else {
						throw new DeserializationException(
							$"Can not assign value {token.Value} (at position {token.Position}) to target type of {targetType.Name}."
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
						throw new DeserializationException(
							$"Can not assign array (at position {token.Position}) to target type of {targetType.Name}."
						);
					}
				default:
					throw new Exception("Unsupported datatype.");
			}
		}

		private object MakeObject(Type targetType, PhpSerializeToken token) {
			var result = targetType.GetConstructor(new Type[0]).Invoke(null);
			var targetProperties = targetType.GetProperties();

			for (int i = 0; i < token.Children.Count; i += 2) {
				var propertyName = token.Children[i].Value;
				var valueToken = token.Children[i + 1];

				var property = targetProperties.FindProperty(propertyName, _options);

				if (property == null) {
					if (!_options.AllowExcessKeys) {
						throw new DeserializationException(
							$"Error: Could not bind the key {propertyName} to object of type {targetType.Name}: No such property."
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
				result.Add(this.DeserializeToken(itemType, token.Children[i]));
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
	}
}