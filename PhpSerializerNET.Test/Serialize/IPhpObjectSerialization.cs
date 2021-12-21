/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class IPhpObjectSerializationTest {

		[TestMethod]
		public void SerializeIPhpObject() {
			var data = new MyPhpObject() { Foo = "" };
			data.SetClassName("MyPhpObject");
			Assert.AreEqual( // strings:
				"O:11:\"MyPhpObject\":1:{s:3:\"Foo\";s:0:\"\";}",
				PhpSerialization.Serialize(data)
			);
		}

		[TestMethod]
		public void SerializePhpObjectDictionary() {
			var data = new PhpObjectDictionary() { { "Foo", "" } };
			data.SetClassName("MyPhpObject");
			Assert.AreEqual( // strings:
				"O:11:\"MyPhpObject\":1:{s:3:\"Foo\";s:0:\"\";}",
				PhpSerialization.Serialize(data)
			);
		}
	}
}