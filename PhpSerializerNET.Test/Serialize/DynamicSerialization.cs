
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Serialize {
	[TestClass]
	public class DynamicSerializationTest {
		[TestMethod]
		public void SerializesPhpDynamicObject() {
			dynamic data = new PhpDynamicObject();
			data.Foo = "a";
			data.Bar = 3.1415;

			Assert.AreEqual(
				"O:8:\"stdClass\":2:{s:3:\"Foo\";s:1:\"a\";s:3:\"Bar\";d:3.1415;}",
				PhpSerialization.Serialize(data)
			);
		}

		[TestMethod]
		public void SerializesPhpDynamicObjectWithClassname() {
			dynamic data = new PhpDynamicObject();
			data.SetClassName("phpDynamicObject");
			data.Foo = "a";
			data.Bar = 3.1415;
			System.Console.WriteLine(data.Bar);
			Assert.AreEqual(
				"O:16:\"phpDynamicObject\":2:{s:3:\"Foo\";s:1:\"a\";s:3:\"Bar\";d:3.1415;}",
				PhpSerialization.Serialize(data)
			);
		}

		[TestMethod]
		public void SerializesExpandoObject() {
			dynamic data = new ExpandoObject();
			data.Foo = "a";
			data.Bar = 3.1415;

			Assert.AreEqual(
				"O:8:\"stdClass\":2:{s:3:\"Foo\";s:1:\"a\";s:3:\"Bar\";d:3.1415;}",
				PhpSerialization.Serialize(data)
			);
		}

	}
}