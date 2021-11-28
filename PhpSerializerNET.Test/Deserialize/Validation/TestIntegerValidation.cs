
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class TestIntegerValidation {
		[TestMethod]
		public void ThrowsOnTruncatedInput() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("i"));
			Assert.AreEqual(
				"Malformed integer at position 0: Unexpected end of input sequence.",
				ex.Message
			);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("i:1"));
			Assert.AreEqual(
				"Malformed integer at position 0: Unexpected end of input sequence.",
				ex.Message
			);
		}

		[TestMethod]
		public void ThrowsOnInvalidValue() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("i:12345b;"));
			Assert.AreEqual(
				"Malformed integer at position 7. Unexpected token 'b'.",
				ex.Message
			);
		}

		[TestMethod]
		public void ThrowOnMissingSemicolon() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("i:12345"));
			Assert.AreEqual(
				"Malformed integer at position 6: Expected token ';', found '5' instead.",
				ex.Message
			);
		}
	}
}