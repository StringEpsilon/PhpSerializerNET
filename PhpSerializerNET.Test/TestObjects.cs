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
				PhpSerializer.Serialize(testObject)
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
				PhpSerializer.Serialize(testObject)
			);
		}


		[TestMethod]
		public void DeserializesClass() {
			var result = (NamedClass)PhpSerializer.Deserialize(
				"O:10:\"NamedClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}"
			);
			Assert.AreEqual(3.14, result.John);
			Assert.AreEqual(2.718, result.Jane);
		}

		[TestMethod]
		public void DeserializesNamedClass() {
			var result = (NamedClass)PhpSerializer.Deserialize(
				"O:7:\"myClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}"
			);
			Assert.AreEqual(3.14, result.John);
			Assert.AreEqual(2.718, result.Jane);
		}

		[TestMethod]
		public void DeserializesStdClassToDynamic() {
			var anonymous = new { foo = "foo" };
			System.Console.WriteLine(anonymous.GetType().Name);
			var result = (dynamic)PhpSerializer.Deserialize(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}"
			);

			Assert.AreEqual(3.14, result["John"]);
			Assert.AreEqual(2.718, result["Jane"]);
		}

		[TestMethod]
		public void DeserializesStdClassToDictionary() {
			var anonymous = new { foo = "foo" };
			System.Console.WriteLine(anonymous.GetType().Name);
			var result = (dynamic)PhpSerializer.Deserialize(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				new PhpDeserializationOptions() { StdClass = StdClassOption.Dynamic }
			);

			Assert.AreEqual(3.14, result.John);
			Assert.AreEqual(2.718, result.Jane);
		}

		[TestMethod]
		public void DeserializesToSpecificClass() {
			var anonymous = new { foo = "foo" };
			System.Console.WriteLine(anonymous.GetType().Name);
			var result = PhpSerializer.Deserialize<NamedClass>(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				new PhpDeserializationOptions() { StdClass = StdClassOption.Dynamic }
			);

			Assert.AreEqual(3.14, result.John);
			Assert.AreEqual(2.718, result.Jane);
		}

		[TestMethod]
		public void DeserializesToSpecificStruct() {
			var anonymous = new { foo = "foo" };
			System.Console.WriteLine(anonymous.GetType().Name);
			var result = PhpSerializer.Deserialize<MyStruct>(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				new PhpDeserializationOptions() { StdClass = StdClassOption.Dynamic }
			);

			Assert.AreEqual(3.14, result.John);
			Assert.AreEqual(2.718, result.Jane);
		}

		[TestMethod]
		public void DeserializesToSpecifiedDictionary() {
			var anonymous = new { foo = "foo" };
			System.Console.WriteLine(anonymous.GetType().Name);
			var result = PhpSerializer.Deserialize<Dictionary<string, object>>(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				new PhpDeserializationOptions() { StdClass = StdClassOption.Dynamic }
			);

			Assert.AreEqual(3.14, result["John"]);
			Assert.AreEqual(2.718, result["Jane"]);
		}
	}
}