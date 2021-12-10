/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Deserialize.Validation {
	[TestClass]
	public class TestDoubleValidation {
		[TestMethod]
		public void ThrowsOnTruncatedInput() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("d"));
			Assert.AreEqual("Unexpected end of input. Expected ':' at index 1, but input ends at index 0", ex.Message);

		}

		[TestMethod]
		public void ThrowsOnMissingColon() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("d     "));
			Assert.AreEqual("Unexpected token at index 1. Expected ':' but found ' ' instead.", ex.Message);

		}

		[TestMethod]
		public void ThrowsOnMissingSemicolon() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("d:111111"));
			Assert.AreEqual("Unexpected end of input. Expected ':' at index 7, but input ends at index 7", ex.Message);
		}


		[TestMethod]
		public void ThrowsOnInvalidCharacter() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("d:bgg5;"));
			Assert.AreEqual(
				"Unexpected token at index 2. 'b' is not a valid part of a floating point number.",
				ex.Message
			);
		}
	}
}