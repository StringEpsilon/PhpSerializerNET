
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET.Test.DataTypes;
using static PhpSerializerNET.Test.DataTypes.DeserializeObjects;

namespace PhpSerializerNET.Test.Deserialize {
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
		public void ExplicitToClassFormatException() {
			var ex = Assert.ThrowsException<DeserializationException>(() =>
			   PhpSerialization.Deserialize<SimpleClass>("a:1:{s:9:\"AnInteger\";s:3:\"1b1\";}")
			);
			Assert.AreEqual(
				"Exception encountered while trying to assign '1b1' to SimpleClass.AnInteger. See inner exception for details.",
				ex.Message
			);
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
		public void ExplicitToDictionaryOfObject() {
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
		public void ExplicitToDictionaryOfComplexType() {
			var result = PhpSerialization.Deserialize<Dictionary<string, SimpleClass>>(
				"a:1:{s:4:\"AKey\";a:5:{s:7:\"AString\";s:22:\"this is a string value\";s:9:\"AnInteger\";i:10;s:7:\"ADouble\";d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}}"
			);

			var expected = new Dictionary<string, SimpleClass>
			{
				{
					"AKey",
					new SimpleClass
					{
						ADouble = 1.2345d,
						AString = "this is a string value",
						AnInteger = 10,
						False = false,
						True = true
					}

				}
			};

			// No easy way to assert dicts in MsTest :/

			Assert.AreEqual(expected.Count, result.Count);

			foreach (var ((expectedKey, expectedValue), (actualKey, actualValue)) in expected.Zip(result)) {
				Assert.AreEqual(expectedKey, actualKey);
				Assert.AreEqual(expectedValue.ADouble, actualValue.ADouble);
				Assert.AreEqual(expectedValue.AString, actualValue.AString);
				Assert.AreEqual(expectedValue.AnInteger, actualValue.AnInteger);
				Assert.AreEqual(expectedValue.False, actualValue.False);
				Assert.AreEqual(expectedValue.True, actualValue.True);
			}
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
				"a:3:{s:2:\"en\";s:12:\"Hello World!\";s:2:\"de\";s:11:\"Hallo Welt!\";s:2:\"It\";s:11:\"Ciao mondo!\";}"
			);

			// en and de mapped to differently named property:
			Assert.AreEqual("Hello World!", deserializedObject.English);
			Assert.AreEqual("Hallo Welt!", deserializedObject.German);
			// "it" correctly ignored:
			Assert.AreEqual(null, deserializedObject.It);
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
			CollectionAssert.AreEqual(new List<string>() { "Hello", "World", "12345" }, result);
		}

		[TestMethod]
		public void ExplicitToArray() {
			var result = PhpSerialization.Deserialize<string[]>("a:3:{i:0;s:5:\"Hello\";i:1;s:5:\"World\";i:2;i:12345;}");

			CollectionAssert.AreEqual(new string[] { "Hello", "World", "12345" }, result);
		}

		[TestMethod]
		public void ExplicitToListNonIntegerKey() {
			var ex = Assert.ThrowsException<PhpSerializerNET.DeserializationException>(() =>
				PhpSerialization.Deserialize<List<string>>("a:3:{i:0;s:5:\"Hello\";s:1:\"a\";s:5:\"World\";i:2;i:12345;}")
			);

			Assert.AreEqual("Can not deserialize array at position 0 to list: It has a non-integer key 'a' at element 2 (position 21).", ex.Message);
		}

		[TestMethod]
		public void ExplicitToEmptyList() {
			var result = PhpSerialization.Deserialize<List<char>>("a:0:{}");
			CollectionAssert.AreEqual(new List<char>(), result);
		}

		[TestMethod]
		public void ImplicitToDictionary() {
			var result = PhpSerialization.Deserialize(
				"a:5:{s:7:\"AString\";s:22:\"this is a string value\";s:9:\"AnInteger\";i:10;s:7:\"ADouble\";d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}"
			);

			Assert.IsInstanceOfType(result, typeof(Dictionary<object, object>));
			var dictionary = result as Dictionary<object, object>;
			Assert.AreEqual(5, dictionary.Count);

			Assert.AreEqual("this is a string value", dictionary["AString"]);
			Assert.AreEqual((long)10, dictionary["AnInteger"]);
			Assert.AreEqual(1.2345, dictionary["ADouble"]);
			Assert.AreEqual(true, dictionary["True"]);
			Assert.AreEqual(false, dictionary["False"]);
		}

		[TestMethod]
		public void ExcplicitToNestedObject() {
			var result = PhpSerialization.Deserialize<CircularTest>("a:2:{s:3:\"Foo\";s:5:\"First\";s:3:\"Bar\";a:2:{s:3:\"Foo\";s:6:\"Second\";s:3:\"Bar\";N;}}");

			Assert.AreEqual(
				"First",
				result.Foo
			);
			Assert.IsNotNull(
				result.Bar
			);
			Assert.AreEqual(
				"Second",
				result.Bar.Foo
			);
		}

		[TestMethod]
		public void Test_Issue11() {
			// See https://github.com/StringEpsilon/PhpSerializerNET/issues/11
			var deserializedObject = PhpSerialization.Deserialize(
				"a:1:{i:0;a:7:{s:1:\"A\";N;s:1:\"B\";N;s:1:\"C\";s:1:\"C\";s:5:\"odSdr\";i:1;s:1:\"D\";d:1;s:1:\"E\";N;s:1:\"F\";a:3:{s:1:\"X\";i:8;s:1:\"Y\";N;s:1:\"Z\";N;}}}"
			);
			Assert.IsNotNull(deserializedObject);
		}

		[TestMethod]
		public void Test_Issue12() {
			// See https://github.com/StringEpsilon/PhpSerializerNET/issues/12
			var result = PhpSerialization.Deserialize("a:1:{i:0;a:4:{s:1:\"A\";s:2:\"63\";s:1:\"B\";a:2:{i:558710;s:1:\"2\";i:558709;s:1:\"2\";}s:1:\"C\";s:2:\"71\";s:1:\"G\";a:3:{s:1:\"x\";s:6:\"446368\";s:1:\"y\";s:1:\"0\";s:1:\"z\";s:5:\"1.029\";}}}");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void MixedKeyArrayIntoObject() {
			var result = PhpSerialization.Deserialize<MixedKeysObject>(
				"a:4:{i:0;s:3:\"Foo\";i:1;s:3:\"Bar\";s:1:\"a\";s:1:\"A\";s:1:\"b\";s:1:\"B\";}"
			);

			Assert.AreEqual("Foo", result.Foo);
			Assert.AreEqual("Bar", result.Bar);
			Assert.AreEqual("A", result.Baz);
			Assert.AreEqual("B", result.Dummy);
		}
	}
}