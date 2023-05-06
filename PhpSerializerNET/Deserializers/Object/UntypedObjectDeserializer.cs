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
		return token.Type switch {
			PhpSerializerType.Array => this.ArrayDeserializer.Deserialize(token),
			PhpSerializerType.Object => this.CreateObject(token),
			_ => this.PrimitiveDeserializer.Deserialize(token),
		};
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
		foreach(var item in token.Children) {
			result.TryAdd(
				item.Key.Value,
				this.Deserialize(item.Value)
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
