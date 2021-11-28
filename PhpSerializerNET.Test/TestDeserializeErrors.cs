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
		public void ThrowsOnMalformedBool() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("b"));
			Assert.IsNotNull(ex);
			Assert.AreEqual("Malformed boolean at position 0: Unexpected end of input sequence.", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("b?"));
			Assert.IsNotNull(ex);
			Assert.AreEqual("Malformed boolean at position 0: Unexpected end of input sequence.", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("b:1"));
			Assert.IsNotNull(ex);
			Assert.AreEqual("Malformed boolean at position 0: Unexpected end of input sequence.", ex.Message);
		}

		[TestMethod]
		public void DeserializeMalformedString() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s"));
			Assert.AreEqual("Malformed string at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s_"));
			Assert.AreEqual("Malformed string at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s:1"));
			Assert.AreEqual("Malformed string at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s:1:"));
			Assert.AreEqual("Malformed string at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s:1:\"a "));
			Assert.AreEqual("String at position 4 has an incorrect length of 1: Expected double quote at position 6, found ' ' instead.", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s:1:\"a"));
			Assert.AreEqual("Malformed string at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s:30:\"\";"));
			Assert.AreEqual("Illegal length of 30. The string at position 4 points to out of bounds index 36.", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s:1:\"\";"));
			Assert.AreEqual("String at position 4 has an incorrect length of 1: Expected double quote at position 6, found ';' instead.", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("s:a:\"\";"));
			Assert.AreEqual("String at position 3 has illegal, missing or malformed length.", ex.Message);
		}

		[TestMethod]
		public void ThrowsOnMalformedInteger() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("i"));
			Assert.AreEqual("Malformed integer at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("i?"));
			Assert.AreEqual("Malformed integer at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("i:1"));
			Assert.AreEqual("Malformed integer at position 0", ex.Message);
		}


		[TestMethod]
		public void ThrowsOnMalformedDouble() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("d"));
			Assert.AreEqual("Malformed double at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("d?"));
			Assert.AreEqual("Malformed double at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("d:1"));
			Assert.AreEqual("Malformed double at position 0", ex.Message);
		}

		[TestMethod]
		public void ThrowsOnMalformedArray() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("a"));
			Assert.AreEqual("Malformed array at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("a?"));
			Assert.AreEqual("Malformed array at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("a:1"));
			Assert.AreEqual("Malformed array at position 0", ex.Message);
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
