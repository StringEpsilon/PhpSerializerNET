/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Deserialize {

	[TestClass]
	public class DeserializeStructsTest {
		[TestMethod]
		public void DeserializeArrayToStruct() {
			var value = PhpSerialization.Deserialize<AStruct>(
				"a:2:{s:3:\"foo\";s:3:\"Foo\";s:3:\"bar\";s:3:\"Bar\";}"
			);
			Assert.AreEqual("Foo", value.foo);
			Assert.AreEqual("Bar", value.bar);
		}

		[TestMethod]
		public void DeserializeObjectToStruct() {
			var value = PhpSerialization.Deserialize<AStruct>(
				"O:8:\"sdtClass\":2:{s:3:\"foo\";s:3:\"Foo\";s:3:\"bar\";s:3:\"Bar\";}"
			);
			Assert.AreEqual("Foo", value.foo);
			Assert.AreEqual("Bar", value.bar);
		}


		[TestMethod]
		public void DeserializeWithIgnoredField() {
			var value = PhpSerialization.Deserialize<AStructWithIgnore>(
				"a:2:{s:3:\"foo\";s:3:\"Foo\";s:3:\"bar\";s:3:\"Bar\";}"
			);
			Assert.AreEqual("Foo", value.foo);
			Assert.AreEqual(null, value.bar);
		}

		[TestMethod]
		public void DeserializePropertyName() {
			var value = PhpSerialization.Deserialize<AStructWithRename>(
				"a:2:{s:3:\"foo\";s:3:\"Foo\";s:6:\"foobar\";s:3:\"Bar\";}"
			);
			Assert.AreEqual("Foo", value.foo);
			Assert.AreEqual("Bar", value.bar);
		}

		[TestMethod]
		public void DeserializeBoolToStruct() {
			var ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize<AStruct>(
					"b:1;"
				)
			);

			Assert.AreEqual(
				"Can not assign value \"1\" (at position 0) to target type of AStruct.",
				ex.Message
			);
		}

		[TestMethod]
		public void DeserializeStringToStruct() {
			var ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize<AStruct>(
					"s:3:\"foo\";"
				)
			);

			Assert.AreEqual(
				"Can not assign value \"foo\" (at position 0) to target type of AStruct.",
				ex.Message
			);
		}

		[TestMethod]
		public void DeserializeNullToStruct() {
			Assert.AreEqual(
				default,
				PhpSerialization.Deserialize<AStruct>(
					"N;"
				)
			);
		}
	}
}