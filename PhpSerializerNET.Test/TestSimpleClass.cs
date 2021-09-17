
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class TestSimpleClass {
		public class SimpleClass {
			public string AString { get; set; }
			public int AnInteger { get; set; }
			public double ADouble { get; set; }
			public bool True { get; set; }
			public bool False { get; set; }
		}

		[TestMethod]
		public void DeserializeSimpleClass() {
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
		public void SerializeSimpleClass() {
			var deserializedObject = PhpSerialization.Deserialize<SimpleClass>(
				"a:5:{s:7:\"AString\";s:22:\"this is a string value\";s:9:\"AnInteger\";i:10;s:7:\"ADouble\";d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}"
			);
			var value = new SimpleClass() {
				AString = "this is a string value",
				AnInteger = 10,
				ADouble = 1.2345,
				True = true,
				False = false
			};
			Assert.AreEqual(
				"a:5:{s:7:\"AString\";s:22:\"this is a string value\";s:9:\"AnInteger\";i:10;s:7:\"ADouble\";d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}",
				PhpSerialization.Serialize(value)
			);
		}

		[TestMethod]
		public void ErrorOnFlatValue() {
			var ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize<SimpleClass>("s:7:\"AString\";s:7:\"AString\";")
			);

			Assert.AreEqual("Can not deserialize loose collection of values into object", ex.Message);
		}

		[TestMethod]
		public void ReturnsNull() {
			var result = PhpSerialization.Deserialize<SimpleClass>("N;");

			Assert.IsNull(result);
		}

		[TestMethod]
		public void DeserializesBracketJunk() {
			var deserializedObject = PhpSerialization.Deserialize<SimpleClass>(
				"a:2:{s:7:\"AString\";s:12:\"\"\"\"\"}}}}{{{{\";s:9:\"AnInteger\";i:10;}"
			);
			Assert.AreEqual("\"\"\"\"}}}}{{{{", deserializedObject.AString);
			Assert.AreEqual(10, deserializedObject.AnInteger);

			deserializedObject = PhpSerialization.Deserialize<SimpleClass>(
				"a:2:{s:7:\"AString\";s:12:\";;};};};::::\";s:9:\"AnInteger\";i:10;}"
			);
			Assert.AreEqual(";;};};};::::", deserializedObject.AString);
			Assert.AreEqual(10, deserializedObject.AnInteger);
		}
	}
}