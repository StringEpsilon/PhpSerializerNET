/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class TestArrayValidation {
		
		[TestMethod]
		public void ThrowsOnMalformedArray() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("a"));
			Assert.AreEqual("Malformed array at position 0", ex.Message);
		}

		
		[TestMethod]
		public void ThrowsOnInvalidLength() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("a:-1:{};"));
			Assert.AreEqual("Array at position 3 has illegal, missing or malformed length.", ex.Message);
		}


		[TestMethod]
		public void ThrowsOnMissingBracket() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("a:100:};"));
			Assert.AreEqual("Array at position 5: Expected token '{', found '}' instead.", ex.Message);
		}

		[TestMethod]
		public void ThrowsOnMissingColon() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("a:10000   "));
			Assert.AreEqual("Array at position 8 has illegal, missing or malformed length.", ex.Message);
		}

		[TestMethod]
		public void ThrowsOnAbruptEOF() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("a:10000:"));
			Assert.AreEqual("Array at position 7: Unexpected end of input sequence.", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("a:1000000"));
			Assert.AreEqual("Array at position 8: Unexpected end of input sequence.", ex.Message);
		}
	}
}