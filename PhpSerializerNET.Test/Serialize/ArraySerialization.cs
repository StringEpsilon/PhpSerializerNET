/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Serialize {
	[TestClass]
	public class ArraySerialization {
		[TestMethod]
		public void StringArraySerializaton() {
			string[] data = new string[3] { "a", "b", "c" };

			Assert.AreEqual(
				"a:3:{i:0;s:1:\"a\";i:1;s:1:\"b\";i:2;s:1:\"c\";}",
				PhpSerialization.Serialize(data)
			);
		}

		[TestMethod]
		public void ObjectIntoMixedKeyArray() {
			var data = new MixedKeysObject() {
				Foo = "Foo",
				Bar = "Bar",
				Baz = "A",
				Dummy = "B",
			};

			Assert.AreEqual(
				"a:4:{i:0;s:3:\"Foo\";i:1;s:3:\"Bar\";s:1:\"a\";s:1:\"A\";s:1:\"b\";s:1:\"B\";}",
				PhpSerialization.Serialize(data)
			);
		}
	}
}