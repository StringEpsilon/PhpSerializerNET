/*!
   This Source Code Form is subject to the terms of the Mozilla Public
   License, v. 2.0. If a copy of the MPL was not distributed with this
   file, You can obtain one at http://mozilla.org/MPL/2.0/.
!*/

using System;

namespace PhpSerializerNET;

internal class DefaultPrimitiveDeserializer : PrimitiveDeserializer {
	public DefaultPrimitiveDeserializer(PhpDeserializationOptions options) : base(options) {
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
				throw new ArgumentException("The token given to DefaultPrimitiveDeserializer must be primitive.");
			case PhpSerializerType.Null:
			default:
				return null;
		}
	}

	internal override object Deserialize(PhpSerializeToken token, Type targetType) {
		throw new NotImplementedException("DefaultPrimitiveDeserializer does not implement target type deserialization");
	}
}
