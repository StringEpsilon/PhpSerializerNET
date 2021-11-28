
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class DeserializeStringsTest {
		[TestMethod]
		public void SerializeHelloWorld() {
			Assert.AreEqual(
				"s:12:\"Hello World!\";",
				PhpSerialization.Serialize("Hello World!")
			);
		}
		
		[TestMethod]
		public void DeserializeEmptyStringExplicit() {
			Assert.AreEqual(
				"",
				PhpSerialization.Deserialize<string>("s:0:\"\";")
			);
		}

		[TestMethod]
		public void DeserializeHelloWorld() {
			Assert.AreEqual(
				"Hello World!",
				PhpSerialization.Deserialize("s:12:\"Hello World!\";")
			);
		}

		[TestMethod]
		public void DeserializeEmptyString() {
			Assert.AreEqual(
				"",
				PhpSerialization.Deserialize("s:0:\"\";")
			);
		}

		[TestMethod]
		public void DeserializeUmlauts() {
			Assert.AreEqual(
				"äöüßÄÖÜ",
				PhpSerialization.Deserialize("s:14:\"äöüßÄÖÜ\";")
			);
		}

		[TestMethod]
		public void DeserializeEmoji() {
			Assert.AreEqual(
				"👻",
				PhpSerialization.Deserialize("s:4:\"👻\";")
			);
		}

		[TestMethod]
		public void DeserializeHalfnesting() {
			// This is really how the PHP implementation behaves.
			Assert.AreEqual(
				"_\";s:1:\"_",
				PhpSerialization.Deserialize("s:9:\"_\";s:1:\"_\";")
			);
		}

		[TestMethod]
		public void DeserializeStringToBool() {
			var options = new PhpDeserializationOptions() { NumberStringToBool = true };

			var value = PhpSerialization.Deserialize<bool>("s:1:\"1\";", options);
			Assert.AreEqual(true, value);

			value = PhpSerialization.Deserialize<bool>("s:1:\"0\";", options);
			Assert.AreEqual(false, value);

			value = (bool)PhpSerialization.Deserialize("s:1:\"1\";", options);
			Assert.AreEqual(true, value);

			value = (bool)PhpSerialization.Deserialize("s:1:\"0\";", options);
			Assert.AreEqual(false, value);
		}
	}
}