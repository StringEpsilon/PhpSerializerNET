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
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace PhpSerializerNET;

internal class TypedArrayDeserializer : ArrayDeserializer {
	public TypedArrayDeserializer(PhpDeserializationOptions options) : base(options) {
	}

	internal override object Deserialize(PhpSerializeToken token) {
		return token.Type switch {
			PhpSerializerType.Array => this.DeserializeArray(token),
			PhpSerializerType.Object => this.ObjectDeserializer.Deserialize(token),
			_ => this.PrimitiveDeserializer.Deserialize(token),
		};
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

		foreach(var item in token.Children) {
			object propertyName;
			if (item.Key.Type == PhpSerializerType.String) {
				propertyName = this._options.CaseSensitiveProperties ? item.Key.Value : item.Key.Value.ToLower();
			} else if (item.Key.Type == PhpSerializerType.Integer) {
				propertyName = item.Key.Value.PhpToLong();
			} else {
				throw new DeserializationException(
					$"Error encountered deserizalizing an object of type '{targetType.FullName}': " +
					$"The key '{item.Key.Value}' (from the token at position {item.Key.Position}) has an unsupported type of '{item.Key.Type}'."
				);
			}
			if (!properties.ContainsKey(propertyName)) {
				if (!this._options.AllowExcessKeys) {
					throw new DeserializationException(
						$"Could not bind the key \"{item.Key.Value}\" to object of type {targetType.Name}: No such property."
					);
				}
				continue;
			}
			var property = properties[propertyName];
			if (property != null) { // null if PhpIgnore'd
				try {
					property.SetValue(
						result,
						this.Deserialize(item.Value, property.PropertyType)
					);
				} catch (Exception exception) {
					throw new DeserializationException(
						$"Exception encountered while trying to assign '{item.Value.Value}' to {targetType.Name}.{property.Name}. See inner exception for details.",
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
				foreach(var item in token.Children) {
				result.Add(
					this.Deserialize(item.Key),
					this.Deserialize(item.Value)
				);
			}
			return result;
		}
		Type keyType = targetType.GenericTypeArguments[0];
		Type valueType = targetType.GenericTypeArguments[1];

		foreach(var item in token.Children) {
			result.Add(
				keyType == typeof(object)
					? this.Deserialize(item.Key)
					: this.Deserialize(item.Key, keyType),
				valueType == typeof(object)
					? this.Deserialize(item.Value)
					: this.Deserialize(item.Value, valueType)
			);
		}
		return result;
	}

	public object DeserializeArray(PhpSerializeToken token) {
		if (this._options.UseLists == ListOptions.Never) {
			var result = new Dictionary<object, object>();
			foreach(var item in token.Children) {
				result.Add(
					this.Deserialize(item.Key),
					this.Deserialize(item.Value)
				);
			}
			return result;
		}
		long previousKey = -1;
		bool isList = true;
		bool consecutive = true;
		for (int i = 0; i < token.Children.Length; i ++) {
			var item = token.Children[i];
			if (item.Key.Type != PhpSerializerType.Integer) {
				isList = false;
				break;
			} else {
				var key = item.Key.Value.PhpToLong();
				if (i == 0 || key == previousKey + 1) {
					previousKey = key;
				} else {
					consecutive = false;
				}
			}
		}
		if (!isList || (this._options.UseLists == ListOptions.Default && consecutive == false)) {
			var result = new Dictionary<object, object>();
			foreach(var item in token.Children) {
				result.Add(
					this.Deserialize(item.Key),
					this.Deserialize(item.Value)
				);
			}
			return result;
		} else {
			var result = new List<object>();
			foreach(var item in token.Children) {
				result.Add(this.Deserialize(item.Value));
			}
			return result;
		}
	}

	private object MakeArray(Type targetType, PhpSerializeToken token) {
		var elementType = targetType.GetElementType() ?? throw new InvalidOperationException("targetType.GetElementType() returned null");
		Array result = Array.CreateInstance(elementType, token.Children.Length);

		var arrayIndex = 0;
		foreach(var item in token.Children) {
			result.SetValue(
				elementType == typeof(object)
					? this.Deserialize(item.Value)
					: this.Deserialize(item.Value, elementType),
				arrayIndex
			);
			arrayIndex++;
		}
		return result;
	}

	private object MakeList(Type targetType, PhpSerializeToken token) {
		for (int i = 0; i < token.Children.Length; i ++) {
			var item = token.Children[i];
			if (item.Key.Type != PhpSerializerType.Integer) {
				throw new DeserializationException(
					$"Can not deserialize array at position {token.Position} to list: " +
					$"It has a non-integer key '{item.Key.Value}' at element {i} (position {item.Key.Position})."
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

		foreach(var item in token.Children) {
			result.Add(
				itemType == typeof(object)
					? this.Deserialize(item.Key)
					: this.Deserialize(item.Value, itemType)
			);
		}
		return result;
	}

	private object MakeStruct(Type targetType, PhpSerializeToken token) {
		var result = Activator.CreateInstance(targetType);
		Dictionary<string, FieldInfo> fields = TypeLookup.GetFieldInfos(targetType, this._options);

		foreach(var item in token.Children) {
			var fieldName = this._options.CaseSensitiveProperties ? item.Key.Value : item.Key.Value.ToLower();
			if (!fields.ContainsKey(fieldName)) {
				if (!this._options.AllowExcessKeys) {
					throw new DeserializationException(
						$"Could not bind the key \"{item.Key.Value}\" to struct of type {targetType.Name}: No such field."
					);
				}
				continue;
			}
			if (fields[fieldName] != null) {
				var field = fields[fieldName];
				try {
					field.SetValue(result, this.Deserialize(item.Value, field.FieldType));
				} catch (Exception exception) {
					throw new DeserializationException(
						$"Exception encountered while trying to assign '{item.Value}' to {targetType.Name}.{field.Name}. " +
						"See inner exception for details.",
						exception
					);
				}
			}
		}
		return result;
	}
}