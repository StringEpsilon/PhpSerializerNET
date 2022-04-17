
/**
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Deserialize {
	[TestClass]
	public class ObjectDeserializationTest {
		[TestMethod]
		public void IntegerKeysClass() {
			var result = PhpSerialization.Deserialize<MixedKeysPhpClass>(
				"O:8:\"stdClass\":4:{i:0;s:3:\"Foo\";i:1;s:3:\"Bar\";s:1:\"a\";s:1:\"A\";s:1:\"b\";s:1:\"B\";}"
			);

			Assert.IsNotNull(result);
			Assert.AreEqual("Foo", result.Foo);
			Assert.AreEqual("Bar", result.Bar);
			Assert.AreEqual("A", result.Baz);
			Assert.AreEqual("B", result.Dummy);
		}
	}
}
