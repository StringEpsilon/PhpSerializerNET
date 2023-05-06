/*!
   This Source Code Form is subject to the terms of the Mozilla Public
   License, v. 2.0. If a copy of the MPL was not distributed with this
   file, You can obtain one at http://mozilla.org/MPL/2.0/.
!*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PhpSerializerNET;

internal class TypedArrayDeserializer : ArrayDeserializer {
	public TypedArrayDeserializer(PhpDeserializationOptions options) : base(options) {
	}

	internal override object Deserialize(PhpSerializeToken token) {
		switch (token.Type) {
			case PhpSerializerType.Array:
				return this.DeserializeArray(token);
			case PhpSerializerType.Object:
				return this.ObjectDeserializer.Deserialize(token);
			default:
				return this.PrimitiveDeserializer.Deserialize(token);
		}
	}

	internal override object Deserialize(PhpSerializeToken token, Type targetType) {
		switch (token.Type) {
			case PhpSerializerType.Array:
				if (targetType.IsAssignableTo(typeof(IList))) {
					return this.MakeList(targetType, token);
				} else if (targetType.IsAssignableTo(typeof(IDictionary))) {
					return this.MakeDictionary(targetType, token);
				} else if (targetType.IsClass) {
					return this.MakeObject(targetType, token);
				} else {
					return this.MakeStruct(targetType, token);
				}
			case PhpSerializerType.Object:
				return this.ObjectDeserializer.Deserialize(token, targetType);
			default:
				return this.PrimitiveDeserializer.Deserialize(token, targetType);
		}
	}

	private object MakeObject(Type targetType, PhpSerializeToken token) {
		var result = Activator.CreateInstance(targetType);
		Dictionary<object, PropertyInfo> properties = TypeLookup.GetPropertyInfos(targetType, this._options);

		for (int i = 0; i < token.Children.Length; i += 2) {
			object propertyName;
			if (token.Children[i].Type == PhpSerializerType.String) {
				propertyName = this._options.CaseSensitiveProperties ? token.Children[i].Value : token.Children[i].Value.ToLower();
			} else if (token.Children[i].Type == PhpSerializerType.Integer) {
				propertyName = token.Children[i].Value.PhpToLong();
			} else {
				throw new DeserializationException(
					$"Error encountered deserizalizing an object of type '{targetType.FullName}': " +
					$"The key '{token.Children[i].Value}' (from the token at position {token.Children[i].Position}) has an unsupported type of '{token.Children[i].Type}'."
				);
			}

			var valueToken = token.Children[i + 1];

			if (!properties.ContainsKey(propertyName)) {
				if (!this._options.AllowExcessKeys) {
					throw new DeserializationException(
						$"Could not bind the key \"{token.Children[i].Value}\" to object of type {targetType.Name}: No such property."
					);
				}
				continue;
			}
			var property = properties[propertyName];
			if (property != null) { // null if PhpIgnore'd
				try {
					property.SetValue(
						result,
						this.Deserialize(valueToken, property.PropertyType)
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
					this.Deserialize(keyToken),
					this.Deserialize(valueToken)
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
					? this.Deserialize(keyToken)
					: this.Deserialize(keyToken, keyType),
				valueType == typeof(object)
					? this.Deserialize(valueToken)
					: this.Deserialize(valueToken, valueType)
			);
		}
		return result;
	}

	public object DeserializeArray(PhpSerializeToken token) {
		if (this._options.UseLists == ListOptions.Never) {
			var result = new Dictionary<object, object>();
			for (int i = 0; i < token.Children.Length; i += 2) {
				result.Add(
					this.Deserialize(token.Children[i]),
					this.Deserialize(token.Children[i + 1])
				);
			}
			return result;
		}
		long previousKey = -1;
		bool isList = true;
		bool consecutive = true;
		for (int i = 0; i < token.Children.Length; i += 2) {
			if (token.Children[i].Type != PhpSerializerType.Integer) {
				isList = false;
				break;
			} else {
				var key = token.Children[i].Value.PhpToLong();
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
					this.Deserialize(token.Children[i]),
					this.Deserialize(token.Children[i + 1])
				);
			}
			return result;
		} else {
			var result = new List<object>();
			for (int i = 1; i < token.Children.Length; i += 2) {
				result.Add(this.Deserialize(token.Children[i]));
			}
			return result;
		}
	}

	private object MakeArray(Type targetType, PhpSerializeToken token) {
		var elementType = targetType.GetElementType() ?? throw new InvalidOperationException("targetType.GetElementType() returned null");
		Array result = Array.CreateInstance(elementType, token.Children.Length / 2);

		var arrayIndex = 0;
		for (int i = 1; i < token.Children.Length; i += 2) {
			result.SetValue(
				elementType == typeof(object)
					? this.Deserialize(token.Children[i])
					: this.Deserialize(token.Children[i], elementType),
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
			return this.MakeArray(targetType, token);
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
					? this.Deserialize(token.Children[i])
					: this.Deserialize(token.Children[i], itemType)
			);
		}
		return result;
	}

	private object MakeStruct(Type targetType, PhpSerializeToken token) {
		var result = Activator.CreateInstance(targetType);
		Dictionary<string, FieldInfo> fields = TypeLookup.GetFieldInfos(targetType, this._options);

		for (int i = 0; i < token.Children.Length; i += 2) {
			var fieldName = this._options.CaseSensitiveProperties ? token.Children[i].Value : token.Children[i].Value.ToLower();
			var valueToken = token.Children[i + 1];
			if (!fields.ContainsKey(fieldName)) {
				if (!this._options.AllowExcessKeys) {
					throw new DeserializationException(
						$"Could not bind the key \"{token.Children[i].Value}\" to struct of type {targetType.Name}: No such field."
					);
				}
				continue;
			}
			if (fields[fieldName] != null) {
				var field = fields[fieldName];
				try {
					field.SetValue(result, this.Deserialize(valueToken, field.FieldType));
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
}