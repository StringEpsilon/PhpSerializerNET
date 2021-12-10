/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Deserialize.Options {
	[TestClass]
	public class CaseSensitivePropertiesTest {
		[TestMethod]
		public void Disabled_Array_Deserializes() {
			var result = PhpSerialization.Deserialize<AStruct>(
				"a:2:{s:3:\"FOO\";s:3:\"Foo\";s:3:\"BAR\";s:3:\"Bar\";}",
				new PhpDeserializationOptions() { CaseSensitiveProperties = false }
			);

			Assert.AreEqual("Foo", result.foo);
			Assert.AreEqual("Bar", result.bar);
		}

		[TestMethod]
		public void Disabled_Object_Deserializes() {
			var result = PhpSerialization.Deserialize<AStruct>(
					"O:8:\"stdClass\":2:{s:3:\"FOO\";s:3:\"Foo\";s:3:\"BAR\";s:3:\"Bar\";}",
					new PhpDeserializationOptions() { CaseSensitiveProperties = false }
			);

			Assert.AreEqual("Foo", result.foo);
			Assert.AreEqual("Bar", result.bar);
		}


		[TestMethod]
		public void Enabled_Array_Throws() {
			var exception = Assert.ThrowsException<DeserializationException>(
			 	() => PhpSerialization.Deserialize<AStruct>(
					"a:2:{s:3:\"FOO\";s:3:\"Foo\";s:3:\"BAR\";s:3:\"Bar\";}",
					new PhpDeserializationOptions() { CaseSensitiveProperties = true }
				)
			);

			Assert.AreEqual(
				"Could not bind the key \"FOO\" to struct of type AStruct: No such field.",
				exception.Message
			);
		}

		[TestMethod]
		public void Enabled_Object_Throws() {
			var exception = Assert.ThrowsException<DeserializationException>(
			 	() => PhpSerialization.Deserialize<AStruct>(
					"O:8:\"stdClass\":2:{s:3:\"FOO\";s:3:\"Foo\";s:3:\"BAR\";s:3:\"Bar\";}",
					new PhpDeserializationOptions() { CaseSensitiveProperties = true }
				)
			);

			Assert.AreEqual(
				"Could not bind the key \"FOO\" to struct of type AStruct: No such field.",
				exception.Message
			);
		}

		[TestMethod]
		public void Disabled_Object_WorksWithMapping() {
			var deserializedObject = PhpSerialization.Deserialize<MappedClass>(
				"O:8:\"stdClass\":2:{s:2:\"EN\";s:12:\"Hello World!\";s:2:\"DE\";s:11:\"Hallo Welt!\";}",
				new PhpDeserializationOptions() { CaseSensitiveProperties = false }
			);

			// en and de mapped to differently named property:
			Assert.AreEqual("Hello World!", deserializedObject.English);
			Assert.AreEqual("Hallo Welt!", deserializedObject.German);
		}

		[TestMethod]
		public void Disabled_Array_WorksWithMapping() {
			var deserializedObject = PhpSerialization.Deserialize<MappedClass>(
				"a:2:{s:2:\"EN\";s:12:\"Hello World!\";s:2:\"DE\";s:11:\"Hallo Welt!\";}",
				new PhpDeserializationOptions() { CaseSensitiveProperties = false }
			);

			// en and de mapped to differently named property:
			Assert.AreEqual("Hello World!", deserializedObject.English);
			Assert.AreEqual("Hallo Welt!", deserializedObject.German);
		}
	}
}