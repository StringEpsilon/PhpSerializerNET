/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Deserialize.Options {
	[TestClass]
	public class StdClassTest {
		public struct MyStruct {
			public double John;
			public double Jane;
		}

		[TestMethod]
		public void Option_Throw() {
			var ex = Assert.ThrowsException<DeserializationException>( 
				() => PhpSerialization.Deserialize(
					"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
					new PhpDeserializationOptions() { StdClass = StdClassOption.Throw }
				)
			);

			Assert.AreEqual(
				"Encountered 'stdClass' and the behavior 'Throw' was specified in deserialization options.",
				ex.Message
			);
		}

		[TestMethod]
		public void Option_Dynamic() {
			dynamic result = (PhpDynamicObject)PhpSerialization.Deserialize(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				new PhpDeserializationOptions() { StdClass = StdClassOption.Dynamic }
			);

			Assert.AreEqual(3.14, result.John);
			Assert.AreEqual(2.718, result.Jane);
			Assert.AreEqual("stdClass", result.GetClassName());
		}

		[TestMethod]
		public void Option_Dictionary() {
			var result = (IDictionary)PhpSerialization.Deserialize(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				new PhpDeserializationOptions() { StdClass = StdClassOption.Dictionary }
			);

			Assert.AreEqual(3.14, result["John"]);
			Assert.AreEqual(2.718, result["Jane"]);
		}

		[TestMethod]
		public void Overridden_By_Class() {
			var anonymous = new { foo = "foo" };
			var result = PhpSerialization.Deserialize<NamedClass>(
				"O:8:\"stdClass\":2:{s:3:\"Foo\";d:3.14;s:3:\"Bar\";d:2.718;}",
				new PhpDeserializationOptions() { StdClass = StdClassOption.Dynamic }
			);

			Assert.IsInstanceOfType(result, typeof(NamedClass));
			Assert.AreEqual(3.14, result.Foo);
			Assert.AreEqual(2.718, result.Bar);
		}

		[TestMethod]
		public void Overridden_By_Struct() {
			var result = PhpSerialization.Deserialize<MyStruct>(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				new PhpDeserializationOptions() { StdClass = StdClassOption.Dynamic }
			);

			Assert.IsInstanceOfType(result, typeof(MyStruct));
			Assert.AreEqual(3.14, result.John);
			Assert.AreEqual(2.718, result.Jane);
		}

		[TestMethod]
		public void Overridden_By_Dictionary() {
			var result = PhpSerialization.Deserialize<Dictionary<string, object>>(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				new PhpDeserializationOptions() { StdClass = StdClassOption.Dynamic }
			);

			Assert.AreEqual(3.14, result["John"]);
			Assert.AreEqual(2.718, result["Jane"]);
		}
	}
}