/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Deserialize.Validation {
	[TestClass]
	public class TestStringValidation {
		[TestMethod]
		public void ThrowsOnTruncatedInput() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s"));
			Assert.AreEqual("Unexpected end of input. Expected ':' at index 1, but input ends at index 0", ex.Message);
		}

		[TestMethod]
		public void ThrowsOnMissingStartQuote() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s:3:abc\";"));
			Assert.AreEqual(
				"Unexpected token at index 4. Expected '\"' but found 'a' instead.",
				ex.Message
			);
		}

		[TestMethod]
		public void ThrowsOnMissingEndQuote() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s:3:\"abc;"));
			Assert.AreEqual(
				"Unexpected token at index 8. Expected '\"' but found ';' instead.",
				ex.Message
			);
		}

		[TestMethod]
		public void ThrowsOnInvalidLength() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s:3\"abc\";"));
			Assert.AreEqual(
				"String at position 3 has illegal, missing or malformed length.",
				ex.Message
			);
		}

		[TestMethod]
		public void ThrowsOnOutOfBoundsLength() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s:10:\"abc\";"));
			Assert.AreEqual(
				"Illegal length of 10. The string at position 6 points to out of bounds index 16.",
				ex.Message
			);
		}

		[TestMethod]
		public void ThrowsOnMissingSemicolon() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s:3:\"abc\""));
			Assert.AreEqual("Unexpected end of input. Expected ';' at index 9, but input ends at index 8", ex.Message);
		}
	}
}