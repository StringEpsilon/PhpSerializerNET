/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class TestDoubleValidation {
		[TestMethod]
		public void ThrowsOnTruncatedInput() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("d"));
			Assert.AreEqual("Malformed double at position 0", ex.Message);
		}

		[TestMethod]
		public void ThrowsOnMissingColon() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("d     "));
			Assert.AreEqual("Malformed double at position 0", ex.Message);
		}

		[TestMethod]
		public void ThrowsOnMissingSemicolon() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("d:111111"));
			Assert.AreEqual("Malformed double at position 7: Expected token ';', found '1' instead.", ex.Message);
		}

		[TestMethod]
		public void ThrowsOnInvalidCharacter() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("d:bgg5;"));
			Assert.AreEqual("Malformed double at position 2, unexpected token 'b'", ex.Message);
		}
	}
}