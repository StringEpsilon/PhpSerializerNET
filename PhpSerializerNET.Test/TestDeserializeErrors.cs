/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class DeserializeErrors {
		[TestMethod]
		public void ThrowsOnMalformedNull() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("N"));
			Assert.AreEqual("Malformed null at position 0: Expected ';'", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("N?"));
			Assert.AreEqual("Malformed null at position 0: Expected ';'", ex.Message);
		}

		
		[TestMethod]
		public void ThrowsOnUnexpectedToken() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("_"));
			Assert.AreEqual("Unexpected token '_' at position 0.", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("i:42;_"));
			Assert.AreEqual("Unexpected token '_' at position 5.", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("_i:42;"));
			Assert.AreEqual("Unexpected token '_' at position 0.", ex.Message);
		}
	}
}
