/*!
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

namespace PhpSerializerNET;

internal ref struct PhpDeserializer {
	private readonly PhpDeserializationOptions _options;
	private readonly Span<PhpToken> _tokens;
	private readonly ReadOnlySpan<byte> _input;
	private int _currentToken = 0;

	internal PhpDeserializer(Span<PhpToken> tokens, ReadOnlySpan<byte> input, PhpDeserializationOptions options) {
		this._options = options;
		this._input = input;
		this._tokens = tokens;
	}

	internal object Deserialize() {
		return this.Next();
	}

	internal object Deserialize(Type targetType) {
		return this.Next(targetType);
	}

	private object DeserializeToken(in PhpToken token) {
		switch (token.Type) {
			case PhpDataType.Reference:
				for(int i = 0; i < this._tokens.Length; i++) {
					if (this._tokens[i].Reference == token.Reference) {
						// This is a hack because this whole class was never designed with references in mind.
						// Because of that we're always assuming that this._currentToken points to the token
						// that needs to be deserialized next when filling arrays and objects.
						// Going back in the token-list therefore does not work if we don't also reset the
						// currentToken position and restore it after.
						var tokenPosition = this._currentToken;
						this._currentToken = i+1;
						var reference = this.DeserializeToken(this._tokens[i]);
						this._currentToken = tokenPosition;
						return reference;
					}
				}
				throw new DeserializationException("Can not resolve reference");
			case PhpDataType.Boolean:
				return token.Value.GetBool(this._input);
			case PhpDataType.Integer:
				return token.Value.GetInt(this._input);
			case PhpDataType.Floating:
				return token.Value.GetDouble(this._input);
			case PhpDataType.String:
				if (this._options.NumberStringToBool) {
					if (this._input[token.Value.Start] == (byte)'1' || this._input[token.Value.Start] == (byte)'0') {
						return token.Value.GetBool(this._input);
					}
				}
				return this.GetString(token);
			case PhpDataType.Array:
				return this.MakeCollection(token);
			case PhpDataType.Object:
				return this.MakeClass(token);
			case PhpDataType.Null:
			default:
				return null;
		}
	}
	private object Next() {
		var token = this._tokens[this._currentToken];
		this._currentToken++;
		return this.DeserializeToken(token);
	}

	private object DeserializeToken(Type targetType, in PhpToken token) {
		switch (token.Type) {
			case PhpDataType.Reference:
				for(int i = 0; i < this._tokens.Length; i++) {
					if (this._tokens[i].Reference == token.Reference) {
						// This is a hack because this whole class was never designed with references in mind.
						// Because of that we're always assuming that this._currentToken points to the token
						// that needs to be deserialized next when filling arrays and objects.
						// Going back in the token-list therefore does not work if we don't also reset the
						// currentToken position and restore it after.
						var tokenPosition = this._currentToken;
						this._currentToken = i+1;
						var reference = this.DeserializeToken(targetType, this._tokens[i]);
						this._currentToken = tokenPosition;
						return reference;
					}
				}
				throw new DeserializationException("Can not resolve reference");
			case PhpDataType.Boolean:
				return this.DeserializeBoolean(targetType, token);
			case PhpDataType.Integer:
				return this.DeserializeInteger(targetType, token);
			case PhpDataType.Floating:
				return this.DeserializeDouble(targetType, token);
			case PhpDataType.String:
				return this.DeserializeTokenFromSimpleType(
					targetType,
					token.Type,
					this.GetString(token),
					token.Position
				);
			case PhpDataType.Object:
				object result;
				if (typeof(IDictionary).IsAssignableFrom(targetType)) {
					result = this.MakeDictionary(targetType, token);
				} else if (targetType.IsClass) {
					result = this.MakeObject(targetType, token);
				} else {
					result = this.MakeStruct(targetType, token);
				}

				if (result is IPhpObject phpObject and not PhpDateTime) {
					phpObject.SetClassName(this.GetString(token));
				}
				return result;
			case PhpDataType.Array:
				if (targetType.IsAssignableTo(typeof(IList))) {
					return this.MakeList(targetType, token);
				}
				if (targetType.IsAssignableTo(typeof(IDictionary))) {
					return this.MakeDictionary(targetType, token);
				}
				return targetType.IsClass
					? this.MakeObject(targetType, token)
					: this.MakeStruct(targetType, token);
			case PhpDataType.Null:
			default:
				return targetType.IsValueType
					? Activator.CreateInstance(targetType)
					: null;
		}
	}

	private object Next(Type targetType) {
		var token = this._tokens[this._currentToken];
		this._currentToken++;
		return DeserializeToken(targetType, token);
	}

	private object DeserializeInteger(Type targetType, in PhpToken token) {
		return Type.GetTypeCode(targetType) switch {
			TypeCode.Int16 => short.Parse(token.Value.GetSlice(in this._input), CultureInfo.InvariantCulture),
			TypeCode.Int32 => int.Parse(token.Value.GetSlice(in this._input), CultureInfo.InvariantCulture),
			TypeCode.Int64 => long.Parse(token.Value.GetSlice(in this._input), CultureInfo.InvariantCulture),
			TypeCode.UInt16 => ushort.Parse(token.Value.GetSlice(in this._input), CultureInfo.InvariantCulture),
			TypeCode.UInt32 => uint.Parse(token.Value.GetSlice(in this._input), CultureInfo.InvariantCulture),
			TypeCode.UInt64 => ulong.Parse(token.Value.GetSlice(in this._input), CultureInfo.InvariantCulture),
			TypeCode.SByte => sbyte.Parse(token.Value.GetSlice(in this._input), CultureInfo.InvariantCulture),
			_ => this.DeserializeTokenFromSimpleType(targetType, token.Type, this.GetString(token), token.Position),
		};
	}

	private object DeserializeDouble(Type targetType, in PhpToken token) {
		if (targetType == typeof(double) || targetType == typeof(float)) {
			return token.Value.GetDouble(this._input);
		}

		string value = this.GetString(token);
		if (value == "INF") {
			value = double.PositiveInfinity.ToString(CultureInfo.InvariantCulture);
		} else if (value == "-INF") {
			value = double.NegativeInfinity.ToString(CultureInfo.InvariantCulture);
		}

		return this.DeserializeTokenFromSimpleType(targetType, token.Type, value, token.Position);
	}

	private object DeserializeBoolean(Type targetType, in PhpToken token) {
		if (targetType == typeof(bool) || targetType == typeof(bool?)) {
			return token.Value.GetBool(this._input);
		}

		Type underlyingType = targetType;
		if (targetType.IsNullableReferenceType()) {
			underlyingType = targetType.GenericTypeArguments[0];
		}

		if (!underlyingType.IsIConvertible()) {
			throw new DeserializationException(
				$"Can not assign value \"{this.GetString(token)}\" (at position {token.Position}) to target type of {targetType.Name}."
			);
		}

		return ((IConvertible)token.Value.GetBool(this._input)).ToType(underlyingType, CultureInfo.InvariantCulture);
	}

	private object DeserializeTokenFromSimpleType(
		Type targetType,
		PhpDataType dataType,
		string value,
		int tokenPosition
	) {
		if (!targetType.IsPrimitive && targetType.IsNullableReferenceType()) {
			if (value == "" && this._options.EmptyStringToDefault) {
				return null;
			}

			targetType = targetType.GenericTypeArguments[0];
		}

		// Short-circuit strings:
		if (targetType == typeof(string)) {
			return value == "" && this._options.EmptyStringToDefault
				? default
				: value;
		}

		if (targetType.IsEnum) {
			// Enums are converted by name if the token is a string and by underlying value if they are not
			if (value == "" && this._options.EmptyStringToDefault) {
				return Activator.CreateInstance(targetType);
			}

			if (dataType != PhpDataType.String) {
				return Enum.Parse(targetType, value);
			}

			FieldInfo foundFieldInfo = TypeLookup.GetEnumInfo(targetType, value, this._options);

			if (foundFieldInfo == null) {
				throw new DeserializationException(
					$"Exception encountered while trying to assign '{value}' to type '{targetType.Name}'. " +
					$"The value could not be matched to an enum member.");
			}

			return foundFieldInfo.GetRawConstantValue();
		}

		if (targetType.IsIConvertible()) {
			if (value == "" && this._options.EmptyStringToDefault) {
				return Activator.CreateInstance(targetType);
			}

			if (targetType == typeof(bool)) {
				if (this._options.NumberStringToBool && (value == "0" || value == "1")) {
					return value == "1";
				}
			}

			try {
				return ((IConvertible)value).ToType(targetType, CultureInfo.InvariantCulture);
			} catch (Exception exception) {
				throw new DeserializationException(
					$"Exception encountered while trying to assign '{value}' to type {targetType.Name}. See inner exception for details.",
					exception
				);
			}
		}

		if (targetType == typeof(Guid)) {
			return value == "" && this._options.EmptyStringToDefault
				? default
				: new Guid(value);
		}

		if (targetType == typeof(object)) {
			return value == "" && this._options.EmptyStringToDefault
				? default
				: value;
		}

		throw new DeserializationException(
			$"Can not assign value \"{value}\" (at position {tokenPosition}) to target type of {targetType.Name}.");
	}

	private object MakeClass(in PhpToken token) {
		var typeName = this.GetString(token);
		Type targetType = null;
		if (typeName != "stdClass" && this._options.EnableTypeLookup) {
			targetType = TypeLookup.FindTypeInAssymbly(
				typeName,
				this._options.TypeCache.HasFlag(TypeCacheFlag.ClassNames)
			);
		}

		if (targetType == null || typeName == "stdClass") {
			if (this._options.StdClass == StdClassOption.Dynamic) {
				var result = new PhpDynamicObject(token.Length, typeName);
				result.SetClassName(typeName);
				for (int i = 0; i < token.Length; i++) {
					result.TryAdd((string)this.Next(), this.Next());
				}

				return result;
			} else if (this._options.StdClass == StdClassOption.Dictionary) {
				var result = new PhpObjectDictionary(token.Length, typeName);
				result.SetClassName(typeName);
				for (int i = 0; i < token.Length; i++) {
					result.TryAdd((string)this.Next(), this.Next());
				}

				return result;
			} else {
				throw new DeserializationException(
					"Encountered 'stdClass' and the behavior 'Throw' was specified in deserialization options.");
			}
		}

		// go back one because we're basically re-entering the object-token from the top.
		// If we don't decrement the pointer, we'd start with the first child token instead of the object token.
		this._currentToken--;
		var constructedObject = this.Next(targetType);
		if (constructedObject is IPhpObject phpObject and not PhpDateTime) {
			phpObject.SetClassName(typeName);
		}

		return constructedObject;
	}

	private object MakeStruct(Type targetType, in PhpToken token) {
		var result = Activator.CreateInstance(targetType);
		Dictionary<string, FieldInfo> fields = TypeLookup.GetFieldInfos(targetType, this._options);

		for (int i = 0; i < token.Length; i++) {
			var fieldName = this._tokens[this._currentToken++].Value.GetString(this._input, this._options.InputEncoding);
			if (!this._options.CaseSensitiveProperties) {
				fieldName = fieldName.ToLower();
			}

			if (!fields.ContainsKey(fieldName)) {
				if (!this._options.AllowExcessKeys) {
					throw new DeserializationException(
						$"Could not bind the key \"{fieldName}\" to struct of type {targetType.Name}: No such field."
					);
				}

				continue;
			}

			if (fields[fieldName] != null) {
				var field = fields[fieldName];
				try {
					field.SetValue(result, this.Next(field.FieldType));
				} catch (Exception exception) {
					var valueToken = this._tokens[this._currentToken];
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

	private object MakeObject(Type targetType, in PhpToken token) {
		var result = Activator.CreateInstance(targetType);
		Dictionary<object, PropertyInfo> properties = TypeLookup.GetPropertyInfos(targetType, this._options);

		for (int i = 0; i < token.Length; i++) {
			object propertyName;
			var nameToken = this._tokens[this._currentToken++];
			if (nameToken.Type == PhpDataType.String) {
				propertyName = this._options.CaseSensitiveProperties
					? this.GetString(nameToken)
					: this.GetString(nameToken).ToLower();
			} else if (nameToken.Type == PhpDataType.Integer) {
				propertyName = nameToken.Value.GetInt(this._input);
			} else {
				throw new DeserializationException(
					$"Error encountered deserizalizing an object of type '{targetType.FullName}': " +
					$"The key '{this.GetString(nameToken)}' (from the token at position {nameToken.Position}) has an unsupported type of '{nameToken.Type}'."
				);
			}

			if (!properties.TryGetValue(propertyName, out var property)) {
				if (!this._options.AllowExcessKeys) {
					throw new DeserializationException(
						$"Could not bind the key \"{propertyName}\" to object of type {targetType.Name}: No such property."
					);
				}

				this._currentToken++;
				continue;
			}

			if (property != null) {
				// null if PhpIgnore'd
				try {
					property.SetValue(
						result, this.Next(property.PropertyType)
					);
				} catch (Exception exception) {
					var valueToken = this._tokens[this._currentToken-1];
					throw new DeserializationException(
						$"Exception encountered while trying to assign '{this.GetString(valueToken)}' to {targetType.Name}.{property.Name}. See inner exception for details.",
						exception
					);
				}
			} else {
				this._currentToken++;
			}
		}

		return result;
	}

	private object MakeArray(Type targetType, in PhpToken token) {
		var elementType = targetType.GetElementType() ??
		                  throw new InvalidOperationException("targetType.GetElementType() returned null");
		Array result = Array.CreateInstance(elementType, token.Length);

		if (elementType == typeof(object)) {
			for (int i = 0; i < token.Length; i++) {
				this._currentToken++;
				result.SetValue(this.Next(), i);
			}
		} else {
			for (int i = 0; i < token.Length; i++) {
				this._currentToken++;
				result.SetValue(this.Next(elementType), i);
			}
		}

		return result;
	}

	private object MakeList(Type targetType, in PhpToken token) {
		int index = 0;
		int itemPosition = this._currentToken;
		while (index < token.Length) {
			var valueToken = this._tokens[itemPosition +1];
			if (this._tokens[itemPosition].Type != PhpDataType.Integer) {
				var keyToken = this._tokens[itemPosition];
				throw new DeserializationException(
					$"Can not deserialize array at position {token.Position} to list: " +
					$"It has a non-integer key '{this.GetString(keyToken)}' at element {index+1} (position {keyToken.Position})."
				);
			}
			index++;
			if (valueToken.Type == PhpDataType.Array || valueToken.Type == PhpDataType.Object) {
				itemPosition = valueToken.LastValuePosition +1;
			} else {
				itemPosition += 2;
			}
		}

		if (targetType.IsArray) {
			return this.MakeArray(targetType, token);
		}

		if (Activator.CreateInstance(targetType, token.Length) is not IList result) {
			throw new NullReferenceException("Activator.CreateInstance(targetType) returned null");
		}

		Type itemType = targetType.GenericTypeArguments.Length >= 1
			? targetType.GenericTypeArguments[0]
			: typeof(object);

		if (itemType == typeof(object)) {
			for (int i = 0; i < token.Length; i++) {
				this._currentToken++;
				result.Add(this.Next());
			}
		} else {
			for (int i = 0; i < token.Length; i++) {
				this._currentToken++;
				result.Add(this.Next(itemType));
			}
		}
		return result;
	}

	private object MakeDictionary(Type targetType, in PhpToken token) {
		var result = (IDictionary)Activator.CreateInstance(targetType);
		if (result == null) {
			throw new NullReferenceException($"Activator.CreateInstance({targetType.FullName}) returned null");
		}

		if (!targetType.GenericTypeArguments.Any()) {
			for (int i = 0; i < token.Length; i++) {
				result.Add(this.Next(), this.Next());
			}
			return result;
		}

		Type keyType = targetType.GenericTypeArguments[0];
		Type valueType = targetType.GenericTypeArguments[1];

		for (int i = 0; i < token.Length; i++) {
			result.Add(
				keyType == typeof(object)
					? this.Next()
					: this.Next(keyType),
				valueType == typeof(object)
					? this.Next()
					: this.Next(valueType)
			);
		}
		return result;
	}

	private object MakeCollection(in PhpToken token) {
		if (this._options.UseLists == ListOptions.Never) {
			return this.MakeDictionary(typeof(Dictionary<object, object>), token);
		}

		long previousKey = -1;
		bool isList = true;
		bool consecutive = true;
		int index = 0;
		int itemPosition = this._currentToken;
		while (index < token.Length) {
			if (this._tokens[itemPosition].Type != PhpDataType.Integer) {
				isList = false;
				break;
			} else {
				int key = this._tokens[itemPosition].Value.GetInt(this._input);
				if (index == 0 || key == previousKey + 1) {
					previousKey = key;
				} else {
					consecutive = false;
				}
			}
			index++;
			if (this._tokens[itemPosition+1].Type == PhpDataType.Array || this._tokens[itemPosition+1].Type == PhpDataType.Object) {
				itemPosition = this._tokens[itemPosition+1].LastValuePosition +1;
			} else {
				itemPosition += 2;
			}
		}

		if (!isList || (this._options.UseLists == ListOptions.Default && !consecutive)) {
			var result = new Dictionary<object, object>(token.Length);
			for (int i = 0; i < token.Length; i++) {
				result.Add(this.Next(), this.Next());
			}
			return result;
		} else {
			var result = new List<object>(token.Length);
			for (int i = 0; i < token.Length; i++) {
				this._currentToken++;
				result.Add(this.Next());
			}
			return result;
		}
	}

	private string GetString(in PhpToken token) {
		return token.Value.GetString(this._input, this._options.InputEncoding);
	}
}