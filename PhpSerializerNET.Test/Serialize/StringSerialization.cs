
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Serialize {
	[TestClass]
	public class StringSerializationTest {
		[TestMethod]
		public void SerializeHelloWorld() {
			Assert.AreEqual(
				"s:12:\"Hello World!\";",
				PhpSerialization.Serialize("Hello World!")
			);
		}

		[TestMethod]
		public void SerializeEmptyString() {
			Assert.AreEqual(
				"s:0:\"\";",
				PhpSerialization.Serialize("")
			);
		}

		[TestMethod]
		public void SerializeUmlauts() {
			Assert.AreEqual(
				"s:14:\"äöüßÄÖÜ\";",
				PhpSerialization.Serialize("äöüßÄÖÜ")
			);
		}

		[TestMethod]
		public void SerializeEmoji() {
			Assert.AreEqual(
				"s:4:\"👻\";",
				PhpSerialization.Serialize("👻")
			);
		}

	}
}