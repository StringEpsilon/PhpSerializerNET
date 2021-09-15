/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class TestObjects {
		[PhpClass]
		public class MyPhpObject{
			public double John {get;set;}
			public double Jane {get;set;}
		}

		[TestMethod]
		public void SerializesToObject(){
			var testObject = new MyPhpObject(){
				John = 3.14,
				Jane = 2.718,
			};
			Assert.AreEqual(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				PhpSerializer.Serialize(testObject)
			);
		}

		[TestMethod]
		public void DeserializesStdClassToDynamic(){
			var anonymous = new { foo= "foo"};
			System.Console.WriteLine(anonymous.GetType().Name);
			var result = (dynamic)PhpSerializer.Deserialize(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}"
			);

			Assert.AreEqual(3.14, result["John"]);
			Assert.AreEqual(2.718, result["Jane"]);
		}

		[TestMethod]
		public void DeserializesStdClassToDictionary(){
			var anonymous = new { foo= "foo"};
			System.Console.WriteLine(anonymous.GetType().Name);
			var result = (dynamic)PhpSerializer.Deserialize(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				new PhpDeserializationOptions() {StdClass = StdClassOption.Dynamic}
			);

			Assert.AreEqual(3.14, result.John);
			Assert.AreEqual(2.718, result.Jane);
		}
	}
}