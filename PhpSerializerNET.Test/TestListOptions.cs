/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class DeserializeListOptions {

		[TestMethod]
		public void ListOptionNever() {
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
		public void ListOptionDefault() {
			var test = PhpSerialization.Deserialize(
				"a:2:{i:0;s:1:\"a\";i:1;s:1:\"b\";}",
				new PhpDeserializationOptions() {
					UseLists = ListOptions.Default
				}
			);

			var list = test as List<object>;
			Assert.IsNotNull(list);
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("a", list[0]);
			Assert.AreEqual("b", list[1]);

			// Same option, non-consecutive integer keys:
			test = PhpSerialization.Deserialize(
				"a:2:{i:2;s:1:\"a\";i:4;s:1:\"b\";}",
				new PhpDeserializationOptions() {
					UseLists = ListOptions.Default
				}
			);

			var dictionary = test as Dictionary<object, object>;
			Assert.IsNotNull(dictionary);
			Assert.AreEqual(2, dictionary.Count);
			Assert.AreEqual("a", dictionary[(long)2]);
			Assert.AreEqual("b", dictionary[(long)4]);
		}

		[TestMethod]
		public void ListOptionAllIntegers() {
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

			// Same option, non-consecutive integer keys:
			test = PhpSerialization.Deserialize(
				"a:2:{i:2;s:1:\"a\";i:4;s:1:\"b\";}",
				new PhpDeserializationOptions() {
					UseLists = ListOptions.OnAllIntegerKeys
				}
			);

			list = test as List<object>;
			Assert.IsNotNull(list);
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("a", list[0]);
			Assert.AreEqual("b", list[1]);
		}
	}
}
