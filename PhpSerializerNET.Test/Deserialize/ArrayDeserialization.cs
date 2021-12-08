
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class DeserializeArraysTest {
		[TestMethod]
		public void ExplicitToClass() {
			var deserializedObject = PhpSerialization.Deserialize<SimpleClass>(
				"a:5:{s:7:\"AString\";s:22:\"this is a string value\";s:9:\"AnInteger\";i:10;s:7:\"ADouble\";d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}"
			);
			Assert.AreEqual("this is a string value", deserializedObject.AString);
			Assert.AreEqual(10, deserializedObject.AnInteger);
			Assert.AreEqual(1.2345, deserializedObject.ADouble);
			Assert.AreEqual(true, deserializedObject.True);
			Assert.AreEqual(false, deserializedObject.False);
		}

		[TestMethod]
		public void ExplicitToClassWrongProperty() {
			var ex = Assert.ThrowsException<PhpSerializerNET.DeserializationException>(() =>
				PhpSerialization.Deserialize<SimpleClass>(
					"a:1:{s:7:\"BString\";s:22:\"this is a string value\";}"
				)
			);
			Assert.AreEqual("Could not bind the key \"BString\" to object of type SimpleClass: No such property.", ex.Message);
		}

		[TestMethod]
		public void ExplicitToDictionary() {
			var result = PhpSerialization.Deserialize<Dictionary<string, object>>(
				"a:5:{s:7:\"AString\";s:22:\"this is a string value\";s:9:\"AnInteger\";i:10;s:7:\"ADouble\";d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}"
			);

			Assert.IsInstanceOfType(result, typeof(Dictionary<string, object>));
			Assert.AreEqual(5, result.Count);

			Assert.AreEqual("this is a string value", result["AString"]);
			Assert.AreEqual((long)10, result["AnInteger"]);
			Assert.AreEqual(1.2345, result["ADouble"]);
			Assert.AreEqual(true, result["True"]);
			Assert.AreEqual(false, result["False"]);
		}

		[TestMethod]
		public void ExplicitToHashtable() {
			var result = PhpSerialization.Deserialize<Hashtable>(
				"a:5:{i:0;s:22:\"this is a string value\";i:1;i:10;i:2;d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}"
			);

			Assert.IsInstanceOfType(result, typeof(Hashtable));
			Assert.AreEqual(5, result.Count);
			// the cast to long on the keys is because of the hashtable and C# intrinsics.
			// (int)0 and (long)0 aren't identical enough for the hashtable
			Assert.AreEqual("this is a string value", result[(long)0]);
			Assert.AreEqual((long)10, result[(long)1]);
			Assert.AreEqual(1.2345, result[(long)2]);
			Assert.AreEqual(true, result["True"]);
			Assert.AreEqual(false, result["False"]);
		}

		[TestMethod]
		public void ExplicitToClass_MappingInfo() {
			var deserializedObject = PhpSerialization.Deserialize<MappedClass>(
				"a:3:{s:2:\"en\";s:12:\"Hello World!\";s:2:\"de\";s:11:\"Hallo Welt!\";s:2:\"it\";s:11:\"Ciao mondo!\";}"
			);

			// en and de mapped to differently named property:
			Assert.AreEqual("Hello World!", deserializedObject.English);
			Assert.AreEqual("Hallo Welt!", deserializedObject.German);
			// "it" correctly ignored:
			Assert.AreEqual(null, deserializedObject.it);
		}

		[TestMethod]
		public void ExplicitToStruct() {
			var value = PhpSerialization.Deserialize<AStruct>(
				"a:2:{s:3:\"foo\";s:3:\"Foo\";s:3:\"bar\";s:3:\"Bar\";}"
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
		public void ExplicitToStructWrongField() {
			var ex = Assert.ThrowsException<PhpSerializerNET.DeserializationException>(() =>
				PhpSerialization.Deserialize<AStruct>(
					"a:1:{s:7:\"BString\";s:22:\"this is a string value\";}"
				)
			);
			Assert.AreEqual("Could not bind the key \"BString\" to struct of type AStruct: No such field.", ex.Message);
		}

		[TestMethod]
		public void ExplicitToList() {
			var result = PhpSerialization.Deserialize<List<string>>("a:3:{i:0;s:5:\"Hello\";i:1;s:5:\"World\";i:2;i:12345;}");

			Assert.AreEqual(3, result.Count);
			Assert.AreEqual("Hello", result[0]);
			Assert.AreEqual("World", result[1]);
			Assert.AreEqual("12345", result[2]);
		}
	}
}