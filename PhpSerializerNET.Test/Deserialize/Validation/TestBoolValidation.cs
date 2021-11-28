
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class TestBoolValidation {
		[TestMethod]
		public void ThrowsOnTruncatedInput() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("b"));
			Assert.AreEqual(
				"Malformed boolean at position 0: Unexpected end of input sequence.",
				ex.Message
			);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("b:1"));
			Assert.AreEqual(
				"Malformed boolean at position 0: Unexpected end of input sequence.",
				ex.Message
			);
		}

		[TestMethod]
		public void ThrowsOnInvalidValue() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("b:2;"));
			Assert.AreEqual(
				"Malformed boolean at position 0: Only '1' and '0' are allowed, found '2' instead.",
				ex.Message
			);
		}
	}
}