/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class TestCircularReferences {
		private class CircularClass {
			public string Foo { get; set; }
			public CircularClass Bar { get; set; }
		}

		[TestMethod]
		public void SerializeCircularObject() {
			var testObject = new CircularClass() {
				Foo = "First"
			};
			testObject.Bar = new CircularClass() {
				Foo = "Second",
				Bar = testObject
			};

			Assert.AreEqual(
				"a:2:{s:3:\"Foo\";s:5:\"First\";s:3:\"Bar\";a:2:{s:3:\"Foo\";s:6:\"Second\";s:3:\"Bar\";N;}}",
				PhpSerialization.Serialize(testObject)
			);
		}

		[TestMethod]
		public void SerializeCircularList() {
			List<object> listA = new() { "A", "B" };
			List<object> listB = new() { "C", "D", listA };
			listA.Add(listB);

			Assert.AreEqual( // strings:
				"a:3:{i:0;s:1:\"A\";i:1;s:1:\"B\";i:2;a:3:{i:0;s:1:\"C\";i:1;s:1:\"D\";i:2;N;}}",
				PhpSerialization.Serialize(listA)
			);
		}
	}
}