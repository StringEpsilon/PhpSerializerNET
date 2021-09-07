/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test
{
	[TestClass]
	public class DeserializeErrors
	{
		[TestMethod]
		public void ThrowsOnMalformedNull()
		{
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("N"));
			Assert.AreEqual("Malformed null at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("N?"));
			Assert.AreEqual("Malformed null at position 0", ex.Message);
		}

		[TestMethod]
		public void ThrowsOnMalformedBool()
		{
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("b"));
			Assert.IsNotNull(ex);
			Assert.AreEqual("Malformed boolean at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("b?"));
			Assert.IsNotNull(ex);
			Assert.AreEqual("Malformed boolean at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("b:1"));
			Assert.IsNotNull(ex);
			Assert.AreEqual("Malformed boolean at position 0", ex.Message);
		}

		[TestMethod]
		public void DeserializeMalformedString()
		{
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("s"));
			Assert.AreEqual("Malformed string at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("s_"));
			Assert.AreEqual("Malformed string at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("s:1"));
			Assert.AreEqual("Malformed string at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("s:1:"));
			Assert.AreEqual("Malformed string at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("s:1:\"a "));
			Assert.AreEqual("Malformed string at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("s:1:\"a"));
			Assert.AreEqual("Malformed string at position 0", ex.Message);
		}

		[TestMethod]
		public void ThrowsOnMalformedInteger()
		{
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("i"));
			Assert.AreEqual("Malformed integer at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("i?"));
			Assert.AreEqual("Malformed integer at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("i:1"));
			Assert.AreEqual("Malformed integer at position 0", ex.Message);
		}


		[TestMethod]
		public void ThrowsOnMalformedDouble()
		{
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("d"));
			Assert.AreEqual("Malformed double at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("d?"));
			Assert.AreEqual("Malformed double at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("d:1"));
			Assert.AreEqual("Malformed double at position 0", ex.Message);
		}

		[TestMethod]
		public void ThrowsOnMalformedArray()
		{
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("a"));
			Assert.AreEqual("Malformed array at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("a?"));
			Assert.AreEqual("Malformed array at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("a:1"));
			Assert.AreEqual("Malformed array at position 0", ex.Message);
		}


		[TestMethod]
		public void ThrowsOnUnexpectedToken()
		{
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("_"));
			Assert.AreEqual("Unexpected token '_' at position 0.", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("i:42;_"));
			Assert.AreEqual("Unexpected token '_' at position 5.", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("_i:42;"));
			Assert.AreEqual("Unexpected token '_' at position 0.", ex.Message);
		}
	}
}
