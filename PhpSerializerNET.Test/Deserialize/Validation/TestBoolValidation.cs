
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Deserialize.Validation {
	[TestClass]
	public class TestBoolValidation {
		[TestMethod]
		public void ThrowsOnTruncatedInput() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("b"));
			Assert.AreEqual(
				"Unexpected end of input. Expected ':' at index 1, but input ends at index 0",
				ex.Message
			);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("b:1"));
			Assert.AreEqual(
				"Unexpected end of input. Expected ';' at index 3, but input ends at index 2",
				ex.Message
			);
			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("b:"));
			Assert.AreEqual(
				"Unexpected end of input. Expected '0' or '1' at index 2, but input ends at index 1",
				ex.Message
			);
		}

		[TestMethod]
		public void ThrowsOnInvalidValue() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("b:2;"));
			Assert.AreEqual(
				"Unexpected token in boolean at index 2. Expected either '1' or '0', but found '2' instead.",
				ex.Message
			);
		}
	}
}