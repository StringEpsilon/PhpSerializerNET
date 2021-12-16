/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Serialize {
	[TestClass]
	public class StructSerializationTest {
		[TestMethod]
		public void SerializeStruct() {
			Assert.AreEqual(
				"a:2:{s:3:\"foo\";s:3:\"Foo\";s:3:\"bar\";s:3:\"Bar\";}",
				PhpSerialization.Serialize(
					new AStruct() { foo = "Foo", bar = "Bar" }
				)
			);
		}

		[TestMethod]
		public void SerializeStructWithIgnore() {
			Assert.AreEqual(
				"a:1:{s:3:\"foo\";s:3:\"Foo\";}",
				PhpSerialization.Serialize(
					new AStructWithIgnore() { foo = "Foo", bar = "Bar" }
				)
			);
		}
	}
}