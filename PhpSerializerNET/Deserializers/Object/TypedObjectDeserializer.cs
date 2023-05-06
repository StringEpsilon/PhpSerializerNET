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

internal class TypedObjectDeserializer : ObjectDeserializer {
	public TypedObjectDeserializer(PhpDeserializationOptions options) : base(options) {
	}
	internal override object Deserialize(PhpSerializeToken token) {
		switch (token.Type) {
			case PhpSerializerType.Array:
				return this.ArrayDeserializer.Deserialize(token);
			case PhpSerializerType.Object:
				return this.CreateObject(token);
			default:
				return this.PrimitiveDeserializer.Deserialize(token);
		}
	}

	internal override object Deserialize(PhpSerializeToken token, Type targetType) {
		switch (token.Type) {
			case PhpSerializerType.Array:
				return this.ArrayDeserializer.Deserialize(token);
			case PhpSerializerType.Object:
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
			default:
				return this.PrimitiveDeserializer.Deserialize(token, targetType);
		}
	}

	private object CreateObject(PhpSerializeToken token) {
		var typeName = token.Value;
		object constructedObject;
		Type targetType = null;
		if (typeName != "stdClass" && this._options.EnableTypeLookup) {
			targetType = TypeLookup.FindTypeInAssymbly(typeName, this._options.TypeCache.HasFlag(TypeCacheFlag.ClassNames));
		}
		if (targetType != null && typeName != "stdClass") {
			constructedObject = this.Deserialize(token, targetType);
		} else {
			dynamic result;
			if (_options.StdClass == StdClassOption.Dynamic) {
				result = new PhpDynamicObject();
			} else if (this._options.StdClass == StdClassOption.Dictionary) {
				result = new PhpObjectDictionary();
			} else {
				throw new DeserializationException("Encountered 'stdClass' and the behavior 'Throw' was specified in deserialization options.");
			}
			foreach(var item in token.Children) {
				result.TryAdd(
					item.Key.Value,
					this.Deserialize(item.Value)
				);
			}
			constructedObject = result;
		}
		if (constructedObject is IPhpObject phpObject and not PhpDateTime) {
			phpObject.SetClassName(typeName);
		}
		return constructedObject;
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
}