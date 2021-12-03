
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class TestDictionaries {
		[TestMethod]
		public void DeserializesDictionary() {
			var result = PhpSerialization.Deserialize(
				"a:5:{s:7:\"AString\";s:22:\"this is a string value\";s:9:\"AnInteger\";i:10;s:7:\"ADouble\";d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}"
			);

			Assert.IsInstanceOfType(result, typeof(Dictionary<object, object>));
			var dictionary = result as Dictionary<object, object>;
			Assert.AreEqual(5, dictionary.Count);

			Assert.AreEqual("this is a string value", dictionary["AString"]);
			Assert.AreEqual((long)10, dictionary["AnInteger"]);
			Assert.AreEqual(1.2345, dictionary["ADouble"]);
			Assert.AreEqual(true, dictionary["True"]);
			Assert.AreEqual(false, dictionary["False"]);
		}

		[TestMethod]
		public void SerializesDictionary() {
			var dictionary = new Dictionary<object, object>(){
				{ "AString", "this is a string value" },
				{ "AnInteger", (long)10 },
				{ "ADouble", 1.2345 },
				{ "True", true },
				{ "False", false }
			};

			var result = PhpSerialization.Serialize(
				dictionary
			);
			Assert.AreEqual(
				"a:5:{s:7:\"AString\";s:22:\"this is a string value\";s:9:\"AnInteger\";i:10;s:7:\"ADouble\";d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}",
				result
			);
		}

		[TestMethod]
		public void SerializesBoolDictionary() {
			var dictionary = new Dictionary<bool, object>(){
				{true, "True"},
				{false, "False"}
			};

			var result = PhpSerialization.Serialize(
				dictionary
			);

			Assert.AreEqual(
				"a:2:{b:1;s:4:\"True\";b:0;s:5:\"False\";}",
				result
			);
		}
	}
}
