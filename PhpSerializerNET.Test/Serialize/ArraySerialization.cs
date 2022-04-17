/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Serialize {
	[TestClass]
	public class ArraySerialization {
		[TestMethod]
		public void StringArraySerializaton() {
			string[] data = new string[3] { "a", "b", "c" };

			Assert.AreEqual(
				"a:3:{i:0;s:1:\"a\";i:1;s:1:\"b\";i:2;s:1:\"c\";}",
				PhpSerialization.Serialize(data)
			);
		}

		// TODO: Move to separate file.
		public class MixedKeysObject {
			[PhpProperty(0)]
			public string Foo { get; set; }
			[PhpProperty(1)]
			public string Bar { get; set; }
			[PhpProperty("a")]
			public string Baz { get; set; }
			[PhpProperty("b")]
			public string Dummy { get; set; }
		}

		[TestMethod]
		public void ObjectIntoMixedKeyArray() {
			var data = new MixedKeysObject() {
				Foo = "Foo",
				Bar = "Bar",
				Baz = "A",
				Dummy = "B",
			};

			Assert.AreEqual(
				"a:4:{i:0;s:3:\"Foo\";i:1;s:3:\"Bar\";s:1:\"a\";s:1:\"A\";s:1:\"b\";s:1:\"B\";}",
				PhpSerialization.Serialize(data)
			);
		}

		[TestMethod]
		public void MixedKeyArrayIntoObject() {
			var expected = new MixedKeysObject() {
				Foo = "Foo",
				Bar = "Bar",
				Baz = "A",
				Dummy = "B",
			};

			var result = PhpSerialization.Deserialize<MixedKeysObject>(
				"a:4:{i:0;s:3:\"Foo\";i:1;s:3:\"Bar\";s:1:\"a\";s:1:\"A\";s:1:\"b\";s:1:\"B\";}"
			);

			Assert.AreEqual("Foo", result.Foo);
			Assert.AreEqual("Bar", result.Bar);
			Assert.AreEqual("A", result.Baz);
			Assert.AreEqual("B", result.Dummy);
		}
	}
}