/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class TestObjects {
		[PhpClass]
		public class MyPhpObject {
			public double John { get; set; }
			public double Jane { get; set; }
		}

		[PhpClass("myClass")]
		public class NamedClass {
			public double John { get; set; }
			public double Jane { get; set; }
		}

		public struct MyStruct {
			public double John;
			public double Jane;
		}

		[TestMethod]
		public void SerializesToObject() {
			var testObject = new MyPhpObject() {
				John = 3.14,
				Jane = 2.718,
			};
			Assert.AreEqual(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				PhpSerialization.Serialize(testObject)
			);
		}

		[TestMethod]
		public void SerializesNamedClassToObject() {
			var testObject = new NamedClass() {
				John = 3.14,
				Jane = 2.718,
			};
			Assert.AreEqual(
				"O:7:\"myClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				PhpSerialization.Serialize(testObject)
			);
		}


		[TestMethod]
		public void DeserializesClass() {
			var result = (NamedClass)PhpSerialization.Deserialize(
				"O:10:\"NamedClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}"
			);
			Assert.AreEqual(3.14, result.John);
			Assert.AreEqual(2.718, result.Jane);
		}

		[TestMethod]
		public void DeserializesNamedClass() {
			var result = (NamedClass)PhpSerialization.Deserialize(
				"O:7:\"myClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}"
			);
			Assert.AreEqual(3.14, result.John);
			Assert.AreEqual(2.718, result.Jane);
		}

		[TestMethod]
		public void DeserializesStdClassToDynamic() {
			var anonymous = new { foo = "foo" };

			var result = (dynamic)PhpSerialization.Deserialize(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}"
			);

			Assert.AreEqual(3.14, result["John"]);
			Assert.AreEqual(2.718, result["Jane"]);
		}

		[TestMethod]
		public void DeserializesStdClassToDictionary() {
			var anonymous = new { foo = "foo" };
			dynamic result = (PhpDynamicObject)PhpSerialization.Deserialize(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				new PhpDeserializationOptions() { StdClass = StdClassOption.Dynamic }
			);

			Assert.AreEqual(3.14, result.John);
			Assert.AreEqual(2.718, result.Jane);
			Assert.AreEqual("stdClass", result.GetClassName());
		}

		[TestMethod]
		public void DeserializesToSpecificClass() {
			var anonymous = new { foo = "foo" };
			var result = PhpSerialization.Deserialize<NamedClass>(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				new PhpDeserializationOptions() { StdClass = StdClassOption.Dynamic }
			);

			Assert.AreEqual(3.14, result.John);
			Assert.AreEqual(2.718, result.Jane);
		}

		[TestMethod]
		public void DeserializesToSpecificStruct() {
			var anonymous = new { foo = "foo" };
			var result = PhpSerialization.Deserialize<MyStruct>(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				new PhpDeserializationOptions() { StdClass = StdClassOption.Dynamic }
			);

			Assert.AreEqual(3.14, result.John);
			Assert.AreEqual(2.718, result.Jane);
		}

		[TestMethod]
		public void DeserializesToSpecifiedDictionary() {
			var anonymous = new { foo = "foo" };
			var result = PhpSerialization.Deserialize<Dictionary<string, object>>(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				new PhpDeserializationOptions() { StdClass = StdClassOption.Dynamic }
			);

			Assert.AreEqual(3.14, result["John"]);
			Assert.AreEqual(2.718, result["Jane"]);
		}

		[TestMethod]
		public void DeserializeNesting() {
			var result = (List<object>)PhpSerialization.Deserialize(
				"a:1:{i:0;O:14:\"ABC\\Epsilon\\42\":1:{s:6:\"People\";O:6:\"people\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}}}"
			);
			var firstEntry = (PhpObjectDictionary)result[0];
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("ABC\\Epsilon\\42", firstEntry.GetClassName());
			Assert.AreEqual(
				3.14,
				((PhpObjectDictionary)firstEntry["People"])["John"]
			);
			Assert.AreEqual(
				2.718,
				((PhpObjectDictionary)firstEntry["People"])["Jane"]
			);

			Assert.AreEqual(
				"a:1:{i:0;O:14:\"ABC\\Epsilon\\42\":1:{s:6:\"People\";O:6:\"people\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}}}",
				PhpSerialization.Serialize(result)
			);
		}

		[TestMethod]
		public void ErrorOnInvalidNameLength() {
			var ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize(
					"O:-1:\"stdClass\":1:{s:3:\"Foo\";N;}"
				)
			);

			Assert.AreEqual("Object at position 3 has illegal, missing or malformed length for classname.", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize(
					"O:200:\"stdClass\":1:{s:3:\"Foo\";N;}"
				)
			);

			Assert.AreEqual(
				"Malformed object at position 5: Illegal length of 200. The specified length points to out of bounds index 207.",
				ex.Message
			);
		}

		[TestMethod]
		public void ErrorOnInvalidName() {
			var ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize(
					"O:8:\"stdClass:1:{s:3:\"Foo\";N;}"
				)
			);

			Assert.AreEqual(
				"Malformed object at position 4 has an incorrect length of 8: Expected double quote at position 13, found ':' instead.",
				ex.Message
			);

			ex = Assert.ThrowsException<DeserializationException>(
			   () => PhpSerialization.Deserialize(
				   "O:2:stdClass\":1:{s:3:\"Foo\";N;}"
			   )
		   );

			Assert.AreEqual(
				"Malformed object at position 3: Expected double quote at position 4, found 's' instead.",
				ex.Message
			);
		}

		[TestMethod]
		public void ErrorOnInvalidSyntax() {
			var ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize(
					"O:8:\"stdClass\"1:{s:3:\"Foo\";N;}"
				)
			);

			Assert.AreEqual(
				"Malformed object at position 14: Expected ':', found 1 instead.",
				ex.Message
			);

			ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize(
					"O:8:\"stdClass\":1{s:3:\"Foo\";N;}"
				)
			);

			Assert.AreEqual(
				"Object at position 17 has illegal, missing or malformed length.",
				ex.Message
			);

			ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize(
					"O:8:\"stdClass\":1:s:3:\"Foo\";N;}"
				)
			);

			Assert.AreEqual(
				"Object at position 17: Expected token '{', found 's' instead.",
				ex.Message
			);
		}
	}
}