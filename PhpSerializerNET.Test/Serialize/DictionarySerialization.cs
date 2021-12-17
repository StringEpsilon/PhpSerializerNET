/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Serialize {
	[TestClass]
	public class DictionarySerializationTest {
		[TestMethod]
		public void SerializesMixedData() {
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
		public void SerializesWithDoubleKeys() {
			var dictionary = new Dictionary<object, object>(){
				{1.1, "a"},
				{1.2, "b"},
				{1.3, "c"}
			};

			Assert.AreEqual(
				"a:3:{d:1.1;s:1:\"a\";d:1.2;s:1:\"b\";d:1.3;s:1:\"c\";}",
				PhpSerialization.Serialize(dictionary)
			);
		}

		[TestMethod]
		public void SerializesWithBooleanKeys() {
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

		[TestMethod]
		public void TerminatesCircularReference() {
			var dictionary = new Dictionary<object, object>(){
				{"1", "a"}
			};
			dictionary.Add("2", dictionary);

			var result = PhpSerialization.Serialize(dictionary);

			Assert.AreEqual(
				"a:2:{s:1:\"1\";s:1:\"a\";s:1:\"2\";N;}",
				result
			);
		}
	}
}