/*!
   This Source Code Form is subject to the terms of the Mozilla Public
   License, v. 2.0. If a copy of the MPL was not distributed with this
   file, You can obtain one at http://mozilla.org/MPL/2.0/.
!*/

using System;
using System.Globalization;
using System.Reflection;

namespace PhpSerializerNET;

internal class TypedPrimitiveDeserializer : PrimitiveDeserializer {
	public TypedPrimitiveDeserializer(PhpDeserializationOptions options) : base(options) {
	}

	internal override object Deserialize(PhpSerializeToken token) {
		switch (token.Type) {
			case PhpSerializerType.Boolean:
				return token.Value.PhpToBool();
			case PhpSerializerType.Integer:
				return token.Value.PhpToLong();
			case PhpSerializerType.Floating:
				return token.Value.PhpToDouble();
			case PhpSerializerType.String:
				if (this._options.NumberStringToBool && (token.Value == "0" || token.Value == "1")) {
					return token.Value.PhpToBool();
				}
				return token.Value;
			case PhpSerializerType.Array:
			case PhpSerializerType.Object:
				throw new ArgumentException("The token given to PrimitiveDeserializer must be primitive.");
			case PhpSerializerType.Null:
			default:
				return null;
		}
	}

	internal override object Deserialize(PhpSerializeToken token, Type targetType) {
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
			case PhpSerializerType.Object:
			case PhpSerializerType.Array: {
					throw new ArgumentException("The token given to PrimitiveDeserializer must be primitive.");
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
			return token.Value.PhpToDouble();
		}

		token.Value = token.Value switch {
			"INF" => double.PositiveInfinity.ToString(CultureInfo.InvariantCulture),
			"-INF" => double.NegativeInfinity.ToString(CultureInfo.InvariantCulture),
			_ => token.Value,
		};
		return this.DeserializeTokenFromSimpleType(targetType, token);
	}

	private static object DeserializeBoolean(Type targetType, PhpSerializeToken token) {
		if (targetType == typeof(bool) || targetType == typeof(bool?)) {
			return token.Value.PhpToBool();
		}
		Type underlyingType = targetType;
		if (targetType.IsNullableReferenceType()) {
			underlyingType = targetType.GenericTypeArguments[0];
		}

		if (underlyingType.IsIConvertible()) {
			return ((IConvertible)token.Value.PhpToBool()).ToType(underlyingType, CultureInfo.InvariantCulture);
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
			if (token.Value == "" && this._options.EmptyStringToDefault) {
				return Activator.CreateInstance(targetType);
			}

			if (token.Type != PhpSerializerType.String) {
				return Enum.Parse(targetType, token.Value);
			}

			FieldInfo foundFieldInfo = TypeLookup.GetEnumInfo(targetType, token.Value, this._options);

			if (foundFieldInfo == null) {
				throw new DeserializationException(
					$"Exception encountered while trying to assign '{token.Value}' to type '{targetType.Name}'. " +
					$"The value could not be matched to an enum member.");
			}

			return foundFieldInfo.GetRawConstantValue();
		}

		if (targetType.IsIConvertible()) {
			if (token.Value == "" && _options.EmptyStringToDefault) {
				return Activator.CreateInstance(targetType);
			}

			if (targetType == typeof(bool)) {
				if (_options.NumberStringToBool && token.Value is "0" or "1") {
					return token.Value.PhpToBool();
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
}