/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Deserialize.Options {
	[TestClass]
	public class AllowExcessKeysTest {
		const string StructTestInput = "a:3:{s:3:\"foo\";s:3:\"Foo\";s:3:\"bar\";s:3:\"Bar\";s:6:\"foobar\";s:6:\"FooBar\";}";
		const string ObjectTestInput = "a:2:{s:7:\"AString\";s:3:\"foo\";s:7:\"BString\";s:3:\"bar\";}";

		[TestMethod]
		public void Struct_DeserializesWithOptionEnabled() {
			var value = PhpSerialization.Deserialize<AStruct>(
				StructTestInput,
				new PhpDeserializationOptions() { AllowExcessKeys = true }
			);

			Assert.AreEqual(
				"Foo",
				value.foo
			);
			Assert.AreEqual(
				"Bar",
				value.bar
			);
		}

		[TestMethod]
		public void Struct_ThrowsWithOptionDisabled() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize<AStruct>(
				StructTestInput,
				new PhpDeserializationOptions() { AllowExcessKeys = false }
			));
			Assert.AreEqual("Could not bind the key \"foobar\" to struct of type AStruct: No such field.", ex.Message);
		}

		[TestMethod]
		public void Object_DeserializesWithOptionEnabled() {
			var deserializedObject = PhpSerialization.Deserialize<SimpleClass>(
				ObjectTestInput,
				new PhpDeserializationOptions() { AllowExcessKeys = true }
			);
			Assert.IsNotNull(deserializedObject);
		}

		[TestMethod]
		public void Object_ThrowsWithOptionDisabled() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize<SimpleClass>(
				ObjectTestInput,
				new PhpDeserializationOptions() { AllowExcessKeys = false }
			));
			Assert.AreEqual("Could not bind the key \"BString\" to object of type SimpleClass: No such property.", ex.Message);
		}
	}
}