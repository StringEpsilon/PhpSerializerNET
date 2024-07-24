
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using PhpSerializerNET.Test.DataTypes;
using static PhpSerializerNET.Test.DataTypes.DeserializeObjects;

namespace PhpSerializerNET.Test.Deserialize;

public class DeserializeArraysTest {
	[Fact]
	public void ExplicitToClass() {
		var deserializedObject = PhpSerialization.Deserialize<SimpleClass>(
			"a:5:{s:7:\"AString\";s:22:\"this is a string value\";s:9:\"AnInteger\";i:10;s:7:\"ADouble\";d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}"
		);
		Assert.Equal("this is a string value", deserializedObject.AString);
		Assert.Equal(10, deserializedObject.AnInteger);
		Assert.Equal(1.2345, deserializedObject.ADouble);
		Assert.True(deserializedObject.True);
		Assert.False(deserializedObject.False);
	}

	[Fact]
	public void ExplicitToClassFormatException() {
		var ex = Assert.Throws<DeserializationException>(() =>
			PhpSerialization.Deserialize<SimpleClass>("a:1:{s:9:\"AnInteger\";s:3:\"1b1\";}")
		);
		Assert.Equal(
			"Exception encountered while trying to assign '1b1' to SimpleClass.AnInteger. See inner exception for details.",
			ex.Message
		);
	}

	[Fact]
	public void ExplicitToClassWrongProperty() {
		var ex = Assert.Throws<PhpSerializerNET.DeserializationException>(() =>
			PhpSerialization.Deserialize<SimpleClass>(
				"a:1:{s:7:\"BString\";s:22:\"this is a string value\";}"
			)
		);
		Assert.Equal("Could not bind the key \"BString\" to object of type SimpleClass: No such property.", ex.Message);
	}

	[Fact]
	public void ExplicitToDictionaryOfObject() {
		var result = PhpSerialization.Deserialize<Dictionary<string, object>>(
			"a:5:{s:7:\"AString\";s:22:\"this is a string value\";s:9:\"AnInteger\";i:10;s:7:\"ADouble\";d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}"
		);

		Assert.IsType<Dictionary<string, object>>(result);
		Assert.Equal(5, result.Count);

		Assert.Equal("this is a string value", result["AString"]);
		Assert.Equal((long)10, result["AnInteger"]);
		Assert.Equal(1.2345, result["ADouble"]);
		Assert.Equal(true, result["True"]);
		Assert.Equal(false, result["False"]);
	}

	[Fact]
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

		Assert.Equal(expected.Count, result.Count);

		foreach (var ((expectedKey, expectedValue), (actualKey, actualValue)) in expected.Zip(result)) {
			Assert.Equal(expectedKey, actualKey);
			Assert.Equal(expectedValue.ADouble, actualValue.ADouble);
			Assert.Equal(expectedValue.AString, actualValue.AString);
			Assert.Equal(expectedValue.AnInteger, actualValue.AnInteger);
			Assert.Equal(expectedValue.False, actualValue.False);
			Assert.Equal(expectedValue.True, actualValue.True);
		}
	}

	[Fact]
	public void ExplicitToHashtable() {
		var result = PhpSerialization.Deserialize<Hashtable>(
			"a:5:{i:0;s:22:\"this is a string value\";i:1;i:10;i:2;d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}"
		);

		Assert.IsType<Hashtable>(result);
		Assert.Equal(5, result.Count);
		// the cast to long on the keys is because of the hashtable and C# intrinsics.
		// (int)0 and (long)0 aren't identical enough for the hashtable
		Assert.Equal("this is a string value", result[(long)0]);
		Assert.Equal((long)10, result[(long)1]);
		Assert.Equal(1.2345, result[(long)2]);
		Assert.Equal(true, result["True"]);
		Assert.Equal(false, result["False"]);
	}

	[Fact]
	public void ExplicitToClass_MappingInfo() {
		var deserializedObject = PhpSerialization.Deserialize<MappedClass>(
			"a:3:{s:2:\"en\";s:12:\"Hello World!\";s:2:\"de\";s:11:\"Hallo Welt!\";s:2:\"It\";s:11:\"Ciao mondo!\";}"
		);

		// en and de mapped to differently named property:
		Assert.Equal("Hello World!", deserializedObject.English);
		Assert.Equal("Hallo Welt!", deserializedObject.German);
		// "it" correctly ignored:
		Assert.Null(deserializedObject.It);
	}

	[Fact]
	public void ExplicitToStruct() {
		var value = PhpSerialization.Deserialize<AStruct>(
			"a:2:{s:3:\"foo\";s:3:\"Foo\";s:3:\"bar\";s:3:\"Bar\";}"
		);

		Assert.Equal(
			"Foo",
			value.foo
		);
		Assert.Equal(
			"Bar",
			value.bar
		);
	}

	[Fact]
	public void ExplicitToStructWrongField() {
		var ex = Assert.Throws<PhpSerializerNET.DeserializationException>(() =>
			PhpSerialization.Deserialize<AStruct>(
				"a:1:{s:7:\"BString\";s:22:\"this is a string value\";}"
			)
		);
		Assert.Equal("Could not bind the key \"BString\" to struct of type AStruct: No such field.", ex.Message);
	}

	[Fact]
	public void ExplicitToList() {
		var result = PhpSerialization.Deserialize<List<string>>("a:3:{i:0;s:5:\"Hello\";i:1;s:5:\"World\";i:2;i:12345;}");

		Assert.Equal(3, result.Count);
		Assert.Equal(new List<string>() { "Hello", "World", "12345" }, result);
	}

	[Fact]
	public void ExplicitToArray() {
		var result = PhpSerialization.Deserialize<string[]>("a:3:{i:0;s:5:\"Hello\";i:1;s:5:\"World\";i:2;i:12345;}");

		Assert.Equal(new string[] { "Hello", "World", "12345" }, result);
	}

	[Fact]
	public void ExplicitToListNonIntegerKey() {
		var ex = Assert.Throws<PhpSerializerNET.DeserializationException>(() =>
			PhpSerialization.Deserialize<List<string>>("a:3:{i:0;s:5:\"Hello\";s:1:\"a\";s:5:\"World\";i:2;i:12345;}")
		);

		Assert.Equal("Can not deserialize array at position 0 to list: It has a non-integer key 'a' at element 2 (position 21).", ex.Message);
	}

	[Fact]
	public void ExplicitToEmptyList() {
		var result = PhpSerialization.Deserialize<List<char>>("a:0:{}");
		Assert.Equal(new List<char>(), result);
	}

	[Fact]
	public void ImplicitToDictionary() {
		var result = PhpSerialization.Deserialize(
			"a:5:{s:7:\"AString\";s:22:\"this is a string value\";s:9:\"AnInteger\";i:10;s:7:\"ADouble\";d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}"
		);

		Assert.IsType<Dictionary<object, object>>(result);
		var dictionary = result as Dictionary<object, object>;
		Assert.Equal(5, dictionary.Count);

		Assert.Equal("this is a string value", dictionary["AString"]);
		Assert.Equal((long)10, dictionary["AnInteger"]);
		Assert.Equal(1.2345, dictionary["ADouble"]);
		Assert.Equal(true, dictionary["True"]);
		Assert.Equal(false, dictionary["False"]);
	}

	[Fact]
	public void ExcplicitToNestedObject() {
		var result = PhpSerialization.Deserialize<CircularTest>("a:2:{s:3:\"Foo\";s:5:\"First\";s:3:\"Bar\";a:2:{s:3:\"Foo\";s:6:\"Second\";s:3:\"Bar\";N;}}");

		Assert.Equal("First", result.Foo);
		Assert.NotNull(result.Bar);
		Assert.Equal("Second", result.Bar.Foo);
	}

	[Fact]
	public void Test_Issue11() {
		// See https://github.com/StringEpsilon/PhpSerializerNET/issues/11
		var deserializedObject = PhpSerialization.Deserialize(
			"a:1:{i:0;a:7:{s:1:\"A\";N;s:1:\"B\";N;s:1:\"C\";s:1:\"C\";s:5:\"odSdr\";i:1;s:1:\"D\";d:1;s:1:\"E\";N;s:1:\"F\";a:3:{s:1:\"X\";i:8;s:1:\"Y\";N;s:1:\"Z\";N;}}}"
		);
		Assert.NotNull(deserializedObject);
	}

	[Fact]
	public void Test_Issue12() {
		// See https://github.com/StringEpsilon/PhpSerializerNET/issues/12
		var result = PhpSerialization.Deserialize("a:1:{i:0;a:4:{s:1:\"A\";s:2:\"63\";s:1:\"B\";a:2:{i:558710;s:1:\"2\";i:558709;s:1:\"2\";}s:1:\"C\";s:2:\"71\";s:1:\"G\";a:3:{s:1:\"x\";s:6:\"446368\";s:1:\"y\";s:1:\"0\";s:1:\"z\";s:5:\"1.029\";}}}");
		Assert.NotNull(result);
	}

	[Fact]
	public void MixedKeyArrayIntoObject() {
		var result = PhpSerialization.Deserialize<MixedKeysObject>(
			"a:4:{i:0;s:3:\"Foo\";i:1;s:3:\"Bar\";s:1:\"a\";s:1:\"A\";s:1:\"b\";s:1:\"B\";}"
		);

		Assert.Equal("Foo", result.Foo);
		Assert.Equal("Bar", result.Bar);
		Assert.Equal("A", result.Baz);
		Assert.Equal("B", result.Dummy);
	}
}