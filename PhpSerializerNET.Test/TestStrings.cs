
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class TestStrings {
		[TestMethod]
		public void SerializeHelloWorld() {
			Assert.AreEqual(
				"s:12:\"Hello World!\";",
				PhpSerializer.Serialize("Hello World!")
			);
		}

		[TestMethod]
		public void SerializeEmptyString() {
			Assert.AreEqual(
				"s:0:\"\";",
				PhpSerializer.Serialize("")
			);
		}
		[TestMethod]
		public void SerializeUmlauts() {
			Assert.AreEqual(
				"s:14:\"äöüßÄÖÜ\";",
				PhpSerializer.Serialize("äöüßÄÖÜ")
			);
		}
		[TestMethod]
		public void SerializeEmoji() {
			Assert.AreEqual(
				"s:4:\"👻\";",
				PhpSerializer.Serialize("👻")
			);
		}


		[TestMethod]
		public void DeserializeHelloWorld() {
			Assert.AreEqual(
				"Hello World!",
				PhpSerializer.Deserialize("s:12:\"Hello World!\";")
			);
		}

		[TestMethod]
		public void DeserializeEmptyString() {
			Assert.AreEqual(
				"",
				PhpSerializer.Deserialize("s:0:\"\";")
			);
		}

		[TestMethod]
		public void DeserializeUmlauts() {
			Assert.AreEqual(
				"äöüßÄÖÜ",
				PhpSerializer.Deserialize("s:14:\"äöüßÄÖÜ\";")
			);
		}

		[TestMethod]
		public void DeserializeEmoji() {
			Assert.AreEqual(
				"👻",
				PhpSerializer.Deserialize("s:4:\"👻\";")
			);
		}

		[TestMethod]
		public void DeserializeHalfnesting() {
			// This is really how the PHP implementation behaves.
			Assert.AreEqual(
				"_\";s:1:\"_",
				PhpSerializer.Deserialize("s:9:\"_\";s:1:\"_\";")
			);
		}

		[TestMethod]
		public void DeserializeStringToBool() {
			var options = new PhpDeserializationOptions() { NumberStringToBool = true };

			var value = PhpSerializer.Deserialize<bool>("s:1:\"1\";", options);
			Assert.AreEqual(true, value);

			value = PhpSerializer.Deserialize<bool>("s:1:\"0\";", options);
			Assert.AreEqual(false, value);

			value = (bool)PhpSerializer.Deserialize("s:1:\"1\";", options);
			Assert.AreEqual(true, value);

			value = (bool)PhpSerializer.Deserialize("s:1:\"0\";", options);
			Assert.AreEqual(false, value);
		}
	}
}