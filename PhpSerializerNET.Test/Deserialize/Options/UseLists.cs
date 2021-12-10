/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Deserialize.Options {
	[TestClass]
	public class UseListsTest {
		[TestMethod]
		public void Option_Never() {
			var test = PhpSerialization.Deserialize(
				"a:2:{i:0;s:1:\"a\";i:1;s:1:\"b\";}",
				new PhpDeserializationOptions() {
					UseLists = ListOptions.Never
				}
			);

			var dictionary = test as Dictionary<object, object>;
			Assert.IsNotNull(dictionary);
			Assert.AreEqual(2, dictionary.Count);
			Assert.AreEqual("a", dictionary[(long)0]);
			Assert.AreEqual("b", dictionary[(long)1]);
		}

		[TestMethod]
		public void Option_Default() {
			var result = PhpSerialization.Deserialize(
				"a:2:{i:0;s:1:\"a\";i:1;s:1:\"b\";}",
				new PhpDeserializationOptions() {
					UseLists = ListOptions.Default
				}
			);

			var list = result as List<object>;
			Assert.IsNotNull(list);
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("a", list[0]);
			Assert.AreEqual("b", list[1]);
		}
		
		[TestMethod]
		public void Option_Default_NonConsequetive() {
			// Same option, non-consecutive integer keys:
			var result = PhpSerialization.Deserialize(
				"a:2:{i:2;s:1:\"a\";i:4;s:1:\"b\";}",
				new PhpDeserializationOptions() {
					UseLists = ListOptions.Default
				}
			);

			var dictionary = result as Dictionary<object, object>;
			Assert.IsNotNull(dictionary);
			Assert.AreEqual(2, dictionary.Count);
			Assert.AreEqual("a", dictionary[(long)2]);
			Assert.AreEqual("b", dictionary[(long)4]);
		}

		[TestMethod]
		public void Option_OnAllIntegerKeys() {
			var test = PhpSerialization.Deserialize(
				"a:2:{i:0;s:1:\"a\";i:1;s:1:\"b\";}",
				new PhpDeserializationOptions() {
					UseLists = ListOptions.OnAllIntegerKeys
				}
			);

			var list = test as List<object>;
			Assert.IsNotNull(list);
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("a", list[0]);
			Assert.AreEqual("b", list[1]);

		}

		[TestMethod]
		public void Option_OnAllIntegerKeys_NonConsequetive() {
			// Same option, non-consecutive integer keys:
			var result = PhpSerialization.Deserialize(
				"a:2:{i:2;s:1:\"a\";i:4;s:1:\"b\";}",
				new PhpDeserializationOptions() {
					UseLists = ListOptions.OnAllIntegerKeys
				}
			);

			var list = result as List<object>;
			Assert.IsNotNull(list);
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("a", list[0]);
			Assert.AreEqual("b", list[1]);
		}
	}
}