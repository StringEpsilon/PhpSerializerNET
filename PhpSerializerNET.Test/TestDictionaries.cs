
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class TestDictionaries {
		[TestMethod]
		public void DeserializesDistionary() {
			var result = PhpSerializer.Deserialize(
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

		public void SerializesDistionary() {
			var dictionary = new Dictionary<object, object>(){
				{"this is a string value", "AString"},
				{(long)10, "AnInteger"},
				{1.2345, "ADouble"},
				{true, "True"},
				{false, "False"}
			};

			var result = PhpSerializer.Serialize(
				dictionary
			);

			Assert.IsInstanceOfType(result, typeof(Dictionary<object, object>));
			Assert.AreEqual(
				"a:5:{s:7:\"AString\";s:22:\"this is a string value\";s:9:\"AnInteger\";i:10;s:7:\"ADouble\";d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}",
				result
			);
		}

		[TestMethod]
		public void DeserializesDictionaryExplicitly() {
			var result = PhpSerializer.Deserialize<Dictionary<string, object>>(
				"a:5:{s:7:\"AString\";s:22:\"this is a string value\";s:9:\"AnInteger\";i:10;s:7:\"ADouble\";d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}"
			);

			Assert.IsInstanceOfType(result, typeof(Dictionary<string, object>));
			Assert.AreEqual(5, result.Count);

			Assert.AreEqual("this is a string value", result["AString"]);
			Assert.AreEqual((long)10, result["AnInteger"]);
			Assert.AreEqual(1.2345, result["ADouble"]);
			Assert.AreEqual(true, result["True"]);
			Assert.AreEqual(false, result["False"]);
		}

		[TestMethod]
		public void DeserializesDictionaryStringKey() {
			var result = PhpSerializer.Deserialize<Dictionary<string, object>>(
				"a:5:{i:0;s:22:\"this is a string value\";i:1;i:10;i:2;d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}"
			);

			Assert.IsInstanceOfType(result, typeof(Dictionary<string, object>));
			Assert.AreEqual(5, result.Count);

			Assert.AreEqual("this is a string value", result["0"]);
			Assert.AreEqual((long)10, result["1"]);
			Assert.AreEqual(1.2345, result["2"]);
			Assert.AreEqual(true, result["True"]);
			Assert.AreEqual(false, result["False"]);
		}

		[TestMethod]
		public void DeserializeHashtable() {
			var result = PhpSerializer.Deserialize<Hashtable>(
				"a:5:{i:0;s:22:\"this is a string value\";i:1;i:10;i:2;d:1.2345;s:4:\"True\";b:1;s:5:\"False\";b:0;}"
			);

			Assert.IsInstanceOfType(result, typeof(Hashtable));
			Assert.AreEqual(5, result.Count);
			// the cast to long on the keys is because of the hashtable and C# intrinsics.
			// (int)0 and (long)0 aren't identical enough for the hashtable
			Assert.AreEqual("this is a string value", result[(long)0]);
			Assert.AreEqual((long)10, result[(long)1]);
			Assert.AreEqual(1.2345, result[(long)2]);
			Assert.AreEqual(true, result["True"]);
			Assert.AreEqual(false, result["False"]);
		}

	}
}
