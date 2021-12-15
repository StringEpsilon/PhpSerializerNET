/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using System.Dynamic;

namespace PhpSerializerNET {
	public class PhpDynamicObject : DynamicObject, IPhpObject {
		private readonly PhpObjectDictionary _dictionary = new();

		public PhpDynamicObject() { }

		public void SetClassName(string className) => this._dictionary.SetClassName(className);
		public string GetClassName() => this._dictionary.GetClassName();

		internal void TryAdd(string key, object value) => this._dictionary.TryAdd(key, value);

		internal object GetMember(string name) {
			return this._dictionary[name];
		}

		public override IEnumerable<string> GetDynamicMemberNames() {
			return this._dictionary.Keys;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result) {
			return this._dictionary.TryGetValue(binder.Name, out result);
		}
		public override bool TrySetMember(SetMemberBinder binder, object value) {
			this._dictionary[binder.Name] = value;
			return true;
		}
	}
}