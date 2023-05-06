/*!
   This Source Code Form is subject to the terms of the Mozilla Public
   License, v. 2.0. If a copy of the MPL was not distributed with this
   file, You can obtain one at http://mozilla.org/MPL/2.0/.
!*/

using System;

namespace PhpSerializerNET;

internal class UntypedObjectDeserializer : ObjectDeserializer {
	public UntypedObjectDeserializer(PhpDeserializationOptions options) : base(options) {
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

	private object CreateObject(PhpSerializeToken token) {
		var typeName = token.Value;
		object constructedObject;

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

		if (constructedObject is IPhpObject phpObject and not PhpDateTime) {
			phpObject.SetClassName(typeName);
		}
		return constructedObject;
	}

	internal override object Deserialize(PhpSerializeToken token, Type targetType) {
		throw new NotImplementedException();
	}
}
