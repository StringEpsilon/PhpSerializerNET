
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Deserialize.Validation {
	[TestClass]
	public class TestIntegerValidation {
		[TestMethod]
		public void ThrowsOnTruncatedInput() {
			// var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("i"));
			// Assert.AreEqual(
			// 	"Unexpected end of input. Expected ':' at index 1, but input ends at index 0",
			// 	ex.Message
			// );

			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("i:1"));
			Assert.AreEqual(
				"Unexpected end of input. Expected ':' at index 2, but input ends at index 2",
				ex.Message
			);
		}

		[TestMethod]
		public void ThrowsOnInvalidValue() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("i:12345b;"));
			Assert.AreEqual(
				"Unexpected token at index 7. 'b' is not a valid part of a number.",
				ex.Message
			);
		}

		[TestMethod]
		public void ThrowOnMissingSemicolon() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("i:12345"));
			Assert.AreEqual(
				"Unexpected end of input. Expected ':' at index 6, but input ends at index 6",
				ex.Message
			);
		}
	}
}