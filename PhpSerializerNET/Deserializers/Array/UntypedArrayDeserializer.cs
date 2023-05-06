/*!
   This Source Code Form is subject to the terms of the Mozilla Public
   License, v. 2.0. If a copy of the MPL was not distributed with this
   file, You can obtain one at http://mozilla.org/MPL/2.0/.
!*/

using System;
using System.Collections.Generic;

namespace PhpSerializerNET;

internal class UntypedArrayDeserializer : ArrayDeserializer {
	public UntypedArrayDeserializer(PhpDeserializationOptions options) : base(options) {
	}

	internal override object Deserialize(PhpSerializeToken token) {
		switch (token.Type) {
			case PhpSerializerType.Array:
				return this.DeserializeArray(token);
			case PhpSerializerType.Object:
				return this.ObjectDeserializer.Deserialize(token);
			default:
				return this.PrimitiveDeserializer.Deserialize(token);
		}
	}

	public object DeserializeArray(PhpSerializeToken token) {
		if (this._options.UseLists == ListOptions.Never) {
			var result = new Dictionary<object, object>();
			for (int i = 0; i < token.Children.Length; i += 2) {
				result.Add(
					this.Deserialize(token.Children[i]),
					this.Deserialize(token.Children[i + 1])
				);
			}
			return result;
		}
		long previousKey = -1;
		bool isList = true;
		bool consecutive = true;
		for (int i = 0; i < token.Children.Length; i += 2) {
			if (token.Children[i].Type != PhpSerializerType.Integer) {
				isList = false;
				break;
			} else {
				var key = token.Children[i].Value.PhpToLong();
				if (i == 0 || key == previousKey + 1) {
					previousKey = key;
				} else {
					consecutive = false;
				}
			}
		}
		if (!isList || (this._options.UseLists == ListOptions.Default && consecutive == false)) {
			var result = new Dictionary<object, object>();
			for (int i = 0; i < token.Children.Length; i += 2) {
				result.Add(
					this.Deserialize(token.Children[i]),
					this.Deserialize(token.Children[i + 1])
				);
			}
			return result;
		} else {
			var result = new List<object>();
			for (int i = 1; i < token.Children.Length; i += 2) {
				result.Add(this.Deserialize(token.Children[i]));
			}
			return result;
		}
	}

	internal override object Deserialize(PhpSerializeToken token, Type targetType) {
		throw new NotImplementedException();
	}
}
