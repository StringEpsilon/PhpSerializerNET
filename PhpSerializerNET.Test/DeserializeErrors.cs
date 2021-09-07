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
		public void DeserializesMalformedNull()
		{
			var ex = Assert.ThrowsException<DeserializationException>(() =>PhpSerializer.Deserialize("N"));
			Assert.AreEqual("Malformed null at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("N?"));
			Assert.AreEqual("Malformed null at position 0", ex.Message);
		}
		
		[TestMethod]
		public void DeserializesMalformedBool()
		{
			var ex = Assert.ThrowsException<DeserializationException>(() =>PhpSerializer.Deserialize("b"));
			Assert.IsNotNull(ex);
			Assert.AreEqual("Malformed boolean at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() =>PhpSerializer.Deserialize("b?"));
			Assert.IsNotNull(ex);
			Assert.AreEqual("Malformed boolean at position 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() =>PhpSerializer.Deserialize("b:1"));
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
	}
}
