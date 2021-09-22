/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Dynamic;

namespace PhpSerializerNET {
	public class PhpDynamicObject : DynamicObject, IPhpObject {
		private PhpObjectDictionary _dictionary = new();

		public PhpDynamicObject() { }

		public void SetClassName(string className)  => _dictionary.SetClassName(className);
		public string GetClassName()  => _dictionary.GetClassName();

		internal void TryAdd(string key, object value) => _dictionary.TryAdd(key, value);

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return this._dictionary.TryGetValue(binder.Name, out result);
		}
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			this._dictionary[binder.Name] = value;
			return true;
		}
	}
}