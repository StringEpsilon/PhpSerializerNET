
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class ObjectSerializationTest {
		[TestMethod]
		public void SerializesToStdClass() {
			var testObject = new UnnamedClass() {
				Foo = 3.14,
				Bar = 2.718,
			};
			Assert.AreEqual(
				"O:8:\"stdClass\":2:{s:3:\"Foo\";d:3.14;s:3:\"Bar\";d:2.718;}",
				PhpSerialization.Serialize(testObject)
			);
		}

		[TestMethod]
		public void SerializesToSpecificClass() {
			var testObject = new NamedClass() {
				Foo = 3.14,
				Bar = 2.718,
			};
			Assert.AreEqual(
				"O:7:\"myClass\":2:{s:3:\"Foo\";d:3.14;s:3:\"Bar\";d:2.718;}",
				PhpSerialization.Serialize(testObject)
			);
		}
	}
}