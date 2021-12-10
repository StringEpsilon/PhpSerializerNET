/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Deserialize.Options {
	[TestClass]
	public class EnableTypeLookupTest {
		[TestMethod]
		public void Enabled_Finds_Class() {
			var result = PhpSerialization.Deserialize(
				"O:11:\"MappedClass\":2:{s:2:\"en\";s:12:\"Hello World!\";s:2:\"de\";s:11:\"Hallo Welt!\";}",
				new PhpDeserializationOptions() { EnableTypeLookup = true }
			);

			Assert.IsInstanceOfType(result, typeof(MappedClass));

			// Check that everything was deserialized onto the properties:
			var mappedClass = result as MappedClass;
			Assert.AreEqual("Hello World!", mappedClass.English);
			Assert.AreEqual("Hallo Welt!", mappedClass.German);
		}

		[TestMethod]
		public void Disabled_UseStdClass() {
			var result = PhpSerialization.Deserialize(
				"O:11:\"MappedClass\":2:{s:2:\"en\";s:12:\"Hello World!\";s:2:\"de\";s:11:\"Hallo Welt!\";}",
				new PhpDeserializationOptions() {
					EnableTypeLookup = false,
					StdClass = StdClassOption.Dictionary,
				}
			);

			Assert.IsInstanceOfType(result, typeof(PhpSerializerNET.PhpObjectDictionary));

			// Check that everything was deserialized onto the properties:
			var dictionary = result as PhpObjectDictionary;
			Assert.AreEqual("Hello World!", dictionary["en"]);
			Assert.AreEqual("Hallo Welt!", dictionary["de"]);
		}

	}
}