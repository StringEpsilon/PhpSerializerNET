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
using System.Text;

namespace PhpSerializerNET;

internal ref struct PhpDeserializer {
	private readonly PhpDeserializationOptions _options;
	private Encoding _inputEncoding;
	private readonly Span<PhpToken> _tokens;
	private readonly ReadOnlySpan<byte> _input;
	private int _currentToken = 0;

	internal PhpDeserializer(Span<PhpToken> tokens, ReadOnlySpan<byte> input, PhpDeserializationOptions options) {
		_options = options;
		_input = input;
		_tokens = tokens;
		_inputEncoding = _options.InputEncoding;
	}

	internal object Deserialize() {
		return this.DeserializeToken();
	}

	internal object Deserialize(Type targetType) {
		return this.DeserializeToken(targetType);
	}

	internal T Deserialize<T>() {
		return (T)this.Deserialize(typeof(T));
	}

	private object DeserializeToken() {
		var token = this._tokens[this._currentToken];
		this._currentToken++;
		switch (token.Type) {
			case PhpDataType.Boolean:
				return token.Value.GetBool(this._input);
			case PhpDataType.Integer:
				return token.Value.GetLong(this._input);
			case PhpDataType.Floating:
				return token.Value.GetDouble(this._input);
			case PhpDataType.String:
				if (this._options.NumberStringToBool) {
					if (_input[token.Value.Start] == (byte)'1' || _input[token.Value.Start] == (byte)'0') {
						return token.Value.GetBool(this._input);
					}
				}
				return this.GetString(token);
			case PhpDataType.Array:
				return MakeCollection(token);
			case PhpDataType.Object:
				return MakeClass(token);
			case PhpDataType.Null:
			default:
				return null;
		}
	}



	private object DeserializeToken(Type targetType) {
		if (targetType == null) {
			throw new ArgumentNullException(nameof(targetType));
		}
		var token = this._tokens[this._currentToken];
		this._currentToken++;
		switch (token.Type) {
			case PhpDataType.Boolean:
				return DeserializeBoolean(targetType, token);
			case PhpDataType.Integer:
				return DeserializeInteger(targetType, token);
			case PhpDataType.Floating:
				return DeserializeDouble(targetType, token);
			case PhpDataType.String:
				return DeserializeTokenFromSimpleType(
					targetType,
					token.Type,
					this.GetString(token),
					token.Position
				);
			case PhpDataType.Object: {
					object result;
					if (typeof(IDictionary).IsAssignableFrom(targetType)) {
						result = MakeDictionary(targetType, token);
					} else if (targetType.IsClass) {
						result = MakeObject(targetType, token);
					} else {
						result = MakeStruct(targetType, token);
					}
					if (result is IPhpObject phpObject and not PhpDateTime) {
						phpObject.SetClassName(this.GetString(token));
					}
					return result;
				}
			case PhpDataType.Array: {
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
			case PhpDataType.Null:
			default:
				if (targetType.IsValueType) {
					return Activator.CreateInstance(targetType);
				} else {
					return null;
				}
		}
	}

	private object DeserializeInteger(Type targetType, in PhpToken token) {
		return Type.GetTypeCode(targetType) switch {
			TypeCode.Int16 => short.Parse(token.Value.GetSlice(_input), CultureInfo.InvariantCulture),
			TypeCode.Int32 => int.Parse(token.Value.GetSlice(_input), CultureInfo.InvariantCulture),
			TypeCode.Int64 => long.Parse(token.Value.GetSlice(_input), CultureInfo.InvariantCulture),
			TypeCode.UInt16 => ushort.Parse(token.Value.GetSlice(_input), CultureInfo.InvariantCulture),
			TypeCode.UInt32 => uint.Parse(token.Value.GetSlice(_input), CultureInfo.InvariantCulture),
			TypeCode.UInt64 => ulong.Parse(token.Value.GetSlice(_input), CultureInfo.InvariantCulture),
			TypeCode.SByte => sbyte.Parse(token.Value.GetSlice(_input), CultureInfo.InvariantCulture),
			_ => this.DeserializeTokenFromSimpleType(
				targetType,
				token.Type,
				this.GetString(token),
				token.Position
			),
		};
	}

	private object DeserializeDouble(Type targetType, in PhpToken token) {
		if (targetType == typeof(double) || targetType == typeof(float)) {
			return token.Value.GetDouble(_input);
		}

		string value = this.GetString(token);
		if (value == "INF") {
			value =  double.PositiveInfinity.ToString(CultureInfo.InvariantCulture);
		} else if (value == "-INF") {
			value = double.NegativeInfinity.ToString(CultureInfo.InvariantCulture);
		}
		return this.DeserializeTokenFromSimpleType(targetType, token.Type, value, token.Position);
	}

	private object DeserializeBoolean(Type targetType, in PhpToken token) {
		if (targetType == typeof(bool) || targetType == typeof(bool?)) {
			return token.Value.GetBool(_input);
		}
		Type underlyingType = targetType;
		if (targetType.IsNullableReferenceType()) {
			underlyingType = targetType.GenericTypeArguments[0];
		}

		if (underlyingType.IsIConvertible()) {
			return ((IConvertible)token.Value.GetBool(_input)).ToType(underlyingType, CultureInfo.InvariantCulture);
		} else {
			throw new DeserializationException(
				$"Can not assign value \"{this.GetString(token)}\" (at position {token.Position}) to target type of {targetType.Name}."
			);
		}
	}

	private object DeserializeTokenFromSimpleType(
		Type targetType,
		PhpDataType dataType,
		string value,
		int tokenPosition
	) {
		if (!targetType.IsPrimitive && targetType.IsNullableReferenceType()) {
			if (value == "" && _options.EmptyStringToDefault) {
				return null;
			}

			targetType = targetType.GenericTypeArguments[0];
			if (targetType == null) {
				throw new NullReferenceException("Could not get underlying type for nullable reference type " + targetType);
			}
		}

		// Short-circuit strings:
		if (targetType == typeof(string)) {
			return value == "" && _options.EmptyStringToDefault
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
			if (value == "" && _options.EmptyStringToDefault) {
				return Activator.CreateInstance(targetType);
			}

			if (targetType == typeof(bool)) {
				if (_options.NumberStringToBool && value is "0" or "1") {
					return value.PhpToBool();
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
			return value == "" && _options.EmptyStringToDefault
				? default
				: new Guid(value);
		}

		if (targetType == typeof(object)) {
			return value == "" && _options.EmptyStringToDefault
				? default
				: value;
		}

		throw new DeserializationException($"Can not assign value \"{value}\" (at position {tokenPosition}) to target type of {targetType.Name}.");
	}

	private object MakeClass(in PhpToken token) {
		var typeName = this.GetString(token);
		object constructedObject;
		Type targetType = null;
		if (typeName != "stdClass" && this._options.EnableTypeLookup) {
			targetType = TypeLookup.FindTypeInAssymbly(typeName, this._options.TypeCache.HasFlag(TypeCacheFlag.ClassNames));
		}
		if (targetType != null && typeName != "stdClass") {
			_currentToken--; // go back one because we're basically re-entering the object-token from the top.
			// If we don't decrement the pointer, we'd start with the first child token instead of the object token.
			constructedObject = this.DeserializeToken(targetType);
		} else {
			dynamic result;
			if (_options.StdClass == StdClassOption.Dynamic) {
				result = new PhpDynamicObject();
			} else if (this._options.StdClass == StdClassOption.Dictionary) {
				result = new PhpObjectDictionary();
			} else {
				throw new DeserializationException("Encountered 'stdClass' and the behavior 'Throw' was specified in deserialization options.");
			}
			for (int i = 0; i < token.Length; i++) {
				var key = this.DeserializeToken();
				var value = this.DeserializeToken();
				result.TryAdd(
					(string)key,
					value
				);
			}
			constructedObject = result;
		}
		if (constructedObject is IPhpObject phpObject and not PhpDateTime) {
			phpObject.SetClassName(typeName);
		}
		return constructedObject;
	}

	private object MakeStruct(Type targetType, in PhpToken token) {
		var result = Activator.CreateInstance(targetType);
		Dictionary<string, FieldInfo> fields = TypeLookup.GetFieldInfos(targetType, this._options);

		for (int i = 0; i < token.Length; i++) {
			var fieldName = this._tokens[this._currentToken++].Value.GetString(_input, _options.InputEncoding);
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
					field.SetValue(result, DeserializeToken(field.FieldType));
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
			var nameToken = this._tokens[_currentToken++];
			if (nameToken.Type == PhpDataType.String) {
				propertyName = this._options.CaseSensitiveProperties
					? this.GetString(nameToken)
					: this.GetString(nameToken).ToLower();
			} else if (nameToken.Type == PhpDataType.Integer) {
				propertyName = nameToken.Value.GetLong(_input);
			} else {
				throw new DeserializationException(
					$"Error encountered deserizalizing an object of type '{targetType.FullName}': " +
					$"The key '{this.GetString(nameToken)}' (from the token at position {nameToken.Position}) has an unsupported type of '{nameToken.Type}'."
				);
			}
			if (!properties.ContainsKey(propertyName)) {
				if (!this._options.AllowExcessKeys) {
					throw new DeserializationException(
						$"Could not bind the key \"{propertyName}\" to object of type {targetType.Name}: No such property."
					);
				}
				_currentToken++;
				continue;
			}
			var property = properties[propertyName];
			if (property != null) { // null if PhpIgnore'd
				try {
					property.SetValue(
						result,
						DeserializeToken(property.PropertyType)
					);
				} catch (Exception exception) {
					var valueToken = _tokens[_currentToken-1];
					throw new DeserializationException(
						$"Exception encountered while trying to assign '{this.GetString(valueToken)}' to {targetType.Name}.{property.Name}. See inner exception for details.",
						exception
					);
				}
			} else {
				_currentToken++;
			}
		}
		return result;
	}

	private object MakeArray(Type targetType, in PhpToken token) {
		var elementType = targetType.GetElementType() ?? throw new InvalidOperationException("targetType.GetElementType() returned null");
		Array result = Array.CreateInstance(elementType, token.Length);

		var arrayIndex = 0;
		for (int i = 0; i < token.Length; i++) {
			_currentToken++;
			result.SetValue(
				elementType == typeof(object)
					? DeserializeToken()
					: DeserializeToken(elementType),
				arrayIndex
			);
			arrayIndex++;
		}
		return result;
	}

	private object MakeList(Type targetType, in PhpToken token) {
		for (int i = 0; i < token.Length * 2; i+=2) {
			if (this._tokens[_currentToken+i].Type != PhpDataType.Integer) {
				var badToken = this._tokens[_currentToken+i];
				throw new DeserializationException(
					$"Can not deserialize array at position {token.Position} to list: " +
					$"It has a non-integer key '{this.GetString(badToken)}' at element {i} (position {badToken.Position})."
				);
			}
		}

		if (targetType.IsArray) {
			return MakeArray(targetType, token);
		}
		var result = (IList)Activator.CreateInstance(targetType, token.Length);
		if (result == null) {
			throw new NullReferenceException("Activator.CreateInstance(targetType) returned null");
		}
		Type itemType;
		if (targetType.GenericTypeArguments.Length >= 1) {
			itemType = targetType.GenericTypeArguments[0];
		} else {
			itemType = typeof(object);
		}

		for (int i = 0; i < token.Length; i++) {
			_currentToken++;
			result.Add(
				itemType == typeof(object)
					? DeserializeToken()
					: DeserializeToken(itemType)
			);
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
				result.Add(
					DeserializeToken(),
					DeserializeToken()
				);
			}
			return result;
		}
		Type keyType = targetType.GenericTypeArguments[0];
		Type valueType = targetType.GenericTypeArguments[1];

		for (int i = 0; i < token.Length; i++) {
			result.Add(
				keyType == typeof(object)
					? DeserializeToken()
					: DeserializeToken(keyType),
				valueType == typeof(object)
					? DeserializeToken()
					: DeserializeToken(valueType)
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
		for (int i = 0; i < token.Length*2; i+=2) {
			if (this._tokens[_currentToken+i].Type != PhpDataType.Integer) {
				isList = false;
				break;
			} else {
				var key = this._tokens[_currentToken+i].Value.GetLong(_input);
				if (i == 0 || key == previousKey + 1) {
					previousKey = key;
				} else {
					consecutive = false;
				}
			}
		}
		if (!isList || (this._options.UseLists == ListOptions.Default && consecutive == false)) {
			var result = new Dictionary<object, object>(token.Length);
			for (int i = 0; i < token.Length; i++) {
				result.Add(
					this.DeserializeToken(),
					this.DeserializeToken()
				);
			}
			return result;
		} else {
			var result = new List<object>(token.Length);
			for (int i = 0; i < token.Length; i++) {
				_currentToken++;
				result.Add(this.DeserializeToken());
			}
			return result;
		}
	}

	private string GetString(in PhpToken token) {
		return token.Value.GetString(this._input, this._options.InputEncoding);
	}
}