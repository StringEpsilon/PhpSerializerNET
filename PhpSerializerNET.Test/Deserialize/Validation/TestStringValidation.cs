/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class TestStringValidation {
		[TestMethod]
		public void ThrowsOnTruncatedInput() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s"));
			Assert.AreEqual("Malformed string at position 0", ex.Message);
		}

		[TestMethod]
		public void ThrowsOnMissingStartQuote() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s:3:abc\";"));
			Assert.AreEqual(
				"String at position 3 has an incorrect length of 3: Expected double quote at position 4, found 'a' instead.", 
				ex.Message
			);
		}

		[TestMethod]
		public void ThrowsOnMissingEndQuote() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s:3:\"abc;"));
			Assert.AreEqual(
				"String at position 4 has an incorrect length of 3: Expected double quote at position 8, found ';' instead.",
				ex.Message
			);
		}

		[TestMethod]
		public void ThrowsOnInvalidLength() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s:3\"abc\";"));
			Assert.AreEqual(
				"String at position 4 has illegal, missing or malformed length.",
				ex.Message
			);
		}

		[TestMethod]
		public void ThrowsOnOutOfBoundsLength() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s:10:\"abc\";"));
			Assert.AreEqual(
				"Illegal length of 10. The string at position 4 points to out of bounds index 16.",
				ex.Message
			);
		}

		[TestMethod]
		public void ThrowsOnMissingSemicolon() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s:3:\"abc\""));
			Assert.AreEqual("Malformed string at position 9: Expected semicolon.", ex.Message);
		}
	}
}