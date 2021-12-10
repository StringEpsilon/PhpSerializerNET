/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Deserialize.Validation {
	[TestClass]
	public class TestArrayValidation {

		[TestMethod]
		public void ThrowsOnMalformedArray() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("a"));
			Assert.AreEqual("Unexpected end of input. Expected ':' at index 1, but input ends at index 0", ex.Message);

		}


		[TestMethod]
		public void ThrowsOnInvalidLength() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("a:-1:{};"));
			Assert.AreEqual("Array at position 2 has illegal, missing or malformed length.", ex.Message);
		}


		[TestMethod]
		public void ThrowsOnMissingBracket() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("a:100:};"));
			Assert.AreEqual("Unexpected token at index 6. Expected '{' but found '}' instead.", ex.Message);


		}

		[TestMethod]
		public void ThrowsOnMissingColon() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("a:10000   "));
			Assert.AreEqual("Array at position 7 has illegal, missing or malformed length.", ex.Message);

		}

		[TestMethod]
		public void ThrowsOnAbruptEOF() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("a:10000:"));
			Assert.AreEqual("Unexpected end of input. Expected '{' at index 8, but input ends at index 7", ex.Message);


			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("a:1000000"));
			Assert.AreEqual("Unexpected token at index 8. Expected ':' but found '0' instead.", ex.Message);

		}

		[TestMethod]
		public void ThrowsOnFalseLength() {
			var exception = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize("a:2:{i:0;i:0;i:1;i:1;i:2;i:2;}")
			);

			Assert.AreEqual("Array at position 0 should be of length 2, but actual length is 3.", exception.Message);
		}
	}
}