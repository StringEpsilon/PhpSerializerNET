/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Serialize {
	[TestClass]
	public class ListSerializationTest {
		[TestMethod]
		public void SerializeListOfStrings() {
			Assert.AreEqual( // strings:
				"a:2:{i:0;s:5:\"Hello\";i:1;s:5:\"World\";}",
				PhpSerialization.Serialize(new List<string>() { "Hello", "World" })
			);
		}

		[TestMethod]
		public void SerializeListOfBools() {
			Assert.AreEqual( // booleans:
				"a:2:{i:0;b:1;i:1;b:0;}",
				PhpSerialization.Serialize(new List<object>() { true, false })
			);
		}

		[TestMethod]
		public void SerializeMixedList() {
			Assert.AreEqual( // mixed types:
				"a:5:{i:0;b:1;i:1;i:1;i:2;d:1.23;i:3;s:3:\"end\";i:4;N;}",
				PhpSerialization.Serialize(new List<object>() { true, 1, 1.23, "end", null })
			);
		}
	}
}
