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
			System.Console.WriteLine(anonymous.GetType().Name);
			var result = (dynamic)PhpSerialization.Deserialize(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}"
			);

			Assert.AreEqual(3.14, result["John"]);
			Assert.AreEqual(2.718, result["Jane"]);
		}

		[TestMethod]
		public void DeserializesStdClassToDictionary() {
			var anonymous = new { foo = "foo" };
			System.Console.WriteLine(anonymous.GetType().Name);
			var result = (dynamic)PhpSerialization.Deserialize(
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
			System.Console.WriteLine(anonymous.GetType().Name);
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
			System.Console.WriteLine(anonymous.GetType().Name);
			var result = PhpSerialization.Deserialize<Dictionary<string, object>>(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				new PhpDeserializationOptions() { StdClass = StdClassOption.Dynamic }
			);

			Assert.AreEqual(3.14, result["John"]);
			Assert.AreEqual(2.718, result["Jane"]);
		}

		

		[TestMethod]
		public void DeserializeNesting(){
			var result = (List<dynamic>)PhpSerialization.Deserialize(
				"a:1:{i:0;O:14:\"ABC\\Epsilon\\42\":3:{s:4:\"date\";O:8:\"DateTime\":3:{s:4:\"date\";s:26:\"2021-08-18 09:10:23.441055\";s:13:\"timezone_type\";i:3;s:8:\"timezone\";s:3:\"UTC\";}}}",
				new PhpDeserializationOptions() { EnableTypeLookup = false , StdClass = StdClassOption.Dynamic}
			);
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("2021-08-18 09:10:23.441055", result[0].date.date);
		}
	}
}