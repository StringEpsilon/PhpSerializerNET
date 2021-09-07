/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test
{
	[TestClass]
	public class SerializeObjects
	{
		public class CircularTest
		{
			public string Foo { get; set; }
			public CircularTest Bar { get; set; }
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
		public void SerializesObject()
		{
			var testObject = new
			{
				AString = "this is a string value",
				AnInteger = 10,
				ADouble = 1.2345,
				True = true,
				False = false,
			};

			Assert.AreEqual(
				"a:5:{s:7:\"AString\";s:22:\"this is a string value\";s:9:\"AnInteger\";i:10;s:7:\"ADouble\";d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}",
				PhpSerializer.Serialize(testObject)
			);
		}


		[TestMethod]
		public void DeserializesObjectWithMappingInfo()
		{
			var testObject = new MappedClass(){
				English = "Hello world!",
				German = "Hallo Welt!",
				it = "Ciao mondo!"
			};

			Assert.AreEqual(
				"a:2:{s:2:\"en\";s:12:\"Hello world!\";s:2:\"de\";s:11:\"Hallo Welt!\";}",
				PhpSerializer.Serialize(testObject)
			);
		}

		[TestMethod]
		public void SerializeCircularObject()
		{
			var testObject = new CircularTest()
			{
				Foo = "First"
			};
			testObject.Bar = new CircularTest()
			{
				Foo = "Second",
				Bar = testObject
			};

			Assert.AreEqual(
				"a:2:{s:3:\"Foo\";s:5:\"First\";s:3:\"Bar\";a:2:{s:3:\"Foo\";s:6:\"Second\";s:3:\"Bar\";N;}}",
				PhpSerializer.Serialize(testObject)
			);
		}

		[TestMethod]
		public void SerializeList()
		{

			Assert.AreEqual( // strings:
				"a:2:{i:0;s:5:\"Hello\";i:1;s:5:\"World\";}",
				PhpSerializer.Serialize(new List<string>() { "Hello", "World" })
			);
			Assert.AreEqual( // booleans:
				"a:2:{i:0;b:1;i:1;b:0;}",
				PhpSerializer.Serialize(new List<object>() { true, false })
			);
			Assert.AreEqual( // mixed types:
				"a:5:{i:0;b:1;i:1;i:1;i:2;d:1.23;i:3;s:3:\"end\";i:4;N;}",
				PhpSerializer.Serialize(new List<object>() { true, 1, 1.23, "end", null })
			);
		}
	}
}
