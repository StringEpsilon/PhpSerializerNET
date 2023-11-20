/*!
   This Source Code Form is subject to the terms of the Mozilla Public
   License, v. 2.0. If a copy of the MPL was not distributed with this
   file, You can obtain one at http://mozilla.org/MPL/2.0/.
!*/

using System;

namespace PhpSerializerNET;

internal abstract class ObjectDeserializer {
	protected readonly PhpDeserializationOptions _options;
	internal PrimitiveDeserializer PrimitiveDeserializer { get; set; }
	internal ArrayDeserializer ArrayDeserializer { get; set; }

	internal ObjectDeserializer(PhpDeserializationOptions options) {
		this._options = options;
	}

	internal abstract object Deserialize(PhpSerializeToken token);
	internal abstract object Deserialize(PhpSerializeToken token, Type targetType);
}