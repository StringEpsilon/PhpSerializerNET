
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET;

namespace PhpSerializerNET.Test {
	public struct MyStruct {
		public string foo;
		public string bar;
	}

	[TestClass]
	public class TestStructs {

		[TestMethod]
		public void DeserializeArrayToStruct() {
			var ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerializer.Deserialize<MyStruct>(
					"a:2:{s:3:\"foo\";s:3:\"Foo\";s:3:\"bar\";s:3:\"Boo\";}"
				)
			);

			Assert.AreEqual(
				"Can not assign array (at position 5) to target type of MyStruct.",
				ex.Message
			);
		}

		[TestMethod]
		public void DeserializeStringToStruct() {
			var ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerializer.Deserialize<MyStruct>(
					"s:3:\"foo\";"
				)
			);

			Assert.AreEqual(
				"Can not assign value \"foo\" (at position 0) to target type of MyStruct.",
				ex.Message
			);
		}

		[TestMethod]
		public void DeserializeNullToStruct() {
			Assert.AreEqual(
				default(MyStruct),
				PhpSerializer.Deserialize<MyStruct>(
					"N;"
				)
			);
		}
	}
}