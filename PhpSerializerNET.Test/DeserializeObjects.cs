/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET;

namespace PhpSerializerNET.Test
{
	[TestClass]
	public class DeserializeObjects
	{
		public class CircularTest
		{
			public string Foo { get; set; }
			public CircularTest Bar { get; set; }
		}

		public class SimpleClass
		{
			public string AString { get; set; }
			public int AnInteger { get; set; }
			public double ADouble { get; set; }
			public bool True { get; set; }
			public bool False { get; set; }
		}

		public class MappedClass {
			[PhpProperty("en")]
			public string English {get;set;}

			[PhpProperty("de")]
			public string German {get;set;}

			[PhpIgnore]
			public string it {get;set;}
		}

		[TestMethod]
		public void DeserializesObject()
		{
			var deserializedObject = PhpSerializer.Deserialize<SimpleClass>(
				"a:5:{s:7:\"AString\";s:22:\"this is a string value\";s:9:\"AnInteger\";i:10;s:7:\"ADouble\";d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}"
			);
			Assert.AreEqual("this is a string value", deserializedObject.AString);
			Assert.AreEqual(10, deserializedObject.AnInteger);
			Assert.AreEqual(1.2345, deserializedObject.ADouble);
			Assert.AreEqual(true, deserializedObject.True);
			Assert.AreEqual(false, deserializedObject.False);
		}

		[TestMethod]
		public void DeserializesObjectWithMappingInfo()
		{
			var deserializedObject = PhpSerializer.Deserialize<MappedClass>(
				"a:3:{s:2:\"en\";s:12:\"Hello World!\";s:2:\"de\";s:11:\"Hallo Welt!\";s:2:\"it\";s:11:\"Ciao mondo!\";}"
			);

			// en and de mapped to differently named property:
			Assert.AreEqual("Hello World!", deserializedObject.English);
			Assert.AreEqual("Hallo Welt!", deserializedObject.German);
			// "it" correctly ignored:
			Assert.AreEqual(null, deserializedObject.it);
		}

		[TestMethod]
		public void DeserializesBracketJunk()
		{
			var deserializedObject = PhpSerializer.Deserialize<SimpleClass>(
				"a:2:{s:7:\"AString\";s:12:\"\"\"\"\"}}}}{{{{\";s:9:\"AnInteger\";i:10;}"
			);
			Assert.AreEqual("\"\"\"\"}}}}{{{{", deserializedObject.AString);
			Assert.AreEqual(10, deserializedObject.AnInteger);

			deserializedObject = PhpSerializer.Deserialize<SimpleClass>(
				"a:2:{s:7:\"AString\";s:12:\";;};};};::::\";s:9:\"AnInteger\";i:10;}"
			);
			Assert.AreEqual(";;};};};::::", deserializedObject.AString);
			Assert.AreEqual(10, deserializedObject.AnInteger);
		}


		[TestMethod]
		public void DeserializesDistionary()
		{
			var result = PhpSerializer.Deserialize(
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
		public void DeserializesDictionaryExplicitly()
		{
			var result = PhpSerializer.Deserialize<Dictionary<string, object>>(
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
		public void DeserializesDictionaryStringKey()
		{
			var result = PhpSerializer.Deserialize<Dictionary<string, object>>(
				"a:5:{i:0;s:22:\"this is a string value\";i:1;i:10;i:2;d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}"
			);

			Assert.IsInstanceOfType(result, typeof(Dictionary<string, object>));
			Assert.AreEqual(5, result.Count);

			Assert.AreEqual("this is a string value", result["0"]);
			Assert.AreEqual((long)10, result["1"]);
			Assert.AreEqual(1.2345, result["2"]);
			Assert.AreEqual(true, result["True"]);
			Assert.AreEqual(false, result["False"]);
		}

		[TestMethod]
		public void DeserializeHashtable()
		{
			var result = PhpSerializer.Deserialize<Hashtable>(
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
		public void DeserializeList()
		{
			var result = PhpSerializer.Deserialize<List<String>>("a:3:{i:0;s:5:\"Hello\";i:1;s:5:\"World\";i:2;i:12345;}");

			Assert.AreEqual(3, result.Count);
			Assert.AreEqual("Hello", result[0]);
			Assert.AreEqual("World", result[1]);
			Assert.AreEqual("12345", result[2]);
		}

		[TestMethod]
		public void DeserializeListInvalidLength()
		{
			var exception = Assert.ThrowsException<DeserializationException>(
				() => PhpSerializer.Deserialize<List<String>>("a:2:{i:0;s:5:\"Hello\";i:1;s:5:\"World\";i:2;i:12345;}")
			);
			
			Assert.AreEqual("Array at position 5 should be of length 2, but actual length is 3.", exception.Message);
		}

		[TestMethod]
		public void DeserializeNestedObject(){
			var result = PhpSerializer.Deserialize<CircularTest>("a:2:{s:3:\"Foo\";s:5:\"First\";s:3:\"Bar\";a:2:{s:3:\"Foo\";s:6:\"Second\";s:3:\"Bar\";N;}}");

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
	}
}
