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
			for (int i = 0; i < token.Children.Length; i += 2) {
				result.TryAdd(
					token.Children[i].Value,
					this.Deserialize(token.Children[i + 1])
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
}