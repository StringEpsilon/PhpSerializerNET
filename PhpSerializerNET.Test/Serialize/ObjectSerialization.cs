
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Serialize {
	[TestClass]
	public class ObjectSerializationTest {
		[TestMethod]
		public void SerializesToStdClass() {
			var testObject = new UnnamedClass() {
				Foo = 3.14,
				Bar = 2.718,
			};
			Assert.AreEqual(
				"O:8:\"stdClass\":2:{s:3:\"Foo\";d:3.14;s:3:\"Bar\";d:2.718;}",
				PhpSerialization.Serialize(testObject)
			);
		}

		[TestMethod]
		public void SerializesToSpecificClass() {
			var testObject = new NamedClass() {
				Foo = 3.14,
				Bar = 2.718,
			};
			Assert.AreEqual(
				"O:7:\"myClass\":2:{s:3:\"Foo\";d:3.14;s:3:\"Bar\";d:2.718;}",
				PhpSerialization.Serialize(testObject)
			);
		}

		[TestMethod]
		public void SerializeObjectToArray() {
			var testObject = new MappedClass() {
				English = "Hello world!",
				German = "Hallo Welt!",
				It = "Ciao mondo!"
			};

			Assert.AreEqual(
				"a:3:{s:2:\"en\";s:12:\"Hello world!\";s:2:\"de\";s:11:\"Hallo Welt!\";s:4:\"Guid\";a:1:{s:5:\"Empty\";N;}}",
				PhpSerialization.Serialize(testObject)
			);
		}

		[TestMethod]
		public void SerializeObjectToObject() {
			var testObject = new UnnamedClass() {
				Foo = 1,
				Bar = 2,
			};

			Assert.AreEqual(
				"O:8:\"stdClass\":2:{s:3:\"Foo\";d:1;s:3:\"Bar\";d:2;}",
				PhpSerialization.Serialize(testObject)
			);
		}

		[TestMethod]
		public void ObjectIntoMixedKeyArray() {
			var data = new MixedKeysPhpClass() {
				Foo = "Foo",
				Bar = "Bar",
				Baz = "A",
				Dummy = "B",
			};

			Assert.AreEqual(
				"O:8:\"stdClass\":4:{i:0;s:3:\"Foo\";i:1;s:3:\"Bar\";s:1:\"a\";s:1:\"A\";s:1:\"b\";s:1:\"B\";}",
				PhpSerialization.Serialize(data)
			);
		}
	}
}