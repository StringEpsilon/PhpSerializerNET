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
			Assert.AreEqual("N", ex.Input);
			Assert.AreEqual("Unexpected end of data.", ex.Message);
			Assert.AreEqual(1, ex.Position);
			

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerializer.Deserialize("N?"));
			Assert.AreEqual("N?", ex.Input);
			Assert.AreEqual("Expected ';' at position 1.", ex.Message);
			Assert.AreEqual(1, ex.Position);
		}
		
		[TestMethod]
		public void DeserializesMalformedBool()
		{
			var ex = Assert.ThrowsException<DeserializationException>(() =>PhpSerializer.Deserialize("b"));
			Assert.IsNotNull(ex);
			Assert.AreEqual("b", ex.Input);
			Assert.AreEqual("Unexpected end of data.", ex.Message);
			Assert.AreEqual(1, ex.Position);

			ex = Assert.ThrowsException<DeserializationException>(() =>PhpSerializer.Deserialize("b?"));
			Assert.IsNotNull(ex);
			Assert.AreEqual("Expected ':' at position 1.", ex.Message);
			Assert.AreEqual(1, ex.Position);

			ex = Assert.ThrowsException<DeserializationException>(() =>PhpSerializer.Deserialize("b:1"));
			Assert.IsNotNull(ex);
			Assert.AreEqual("Expected ';' around position 2.", ex.Message);
			Assert.AreEqual(2, ex.Position);
		}

		[TestMethod]
		public void DeserializeMalformedString()
		{
			DeserializationException exception = null;
			try {
				PhpSerializer.Deserialize("s");
			}catch(DeserializationException ex){
				exception = ex;
			}
			Assert.IsNotNull(exception);
			Assert.AreEqual("Unexpected end of data.", exception.Message);
			Assert.AreEqual(1, exception.Position);

			try {
				PhpSerializer.Deserialize("s_");
			}catch(DeserializationException ex){
				exception = ex;
			}
			Assert.IsNotNull(exception);
			Assert.AreEqual("Expected ':' at position 1.", exception.Message);
			Assert.AreEqual(1, exception.Position);

			try {
				PhpSerializer.Deserialize("s:1");
			}catch(DeserializationException ex){
				exception = ex;
			}
			Assert.IsNotNull(exception);
			Assert.AreEqual("Expected ':' around position 3.", exception.Message);
			Assert.AreEqual(3, exception.Position);

			try {
				PhpSerializer.Deserialize("s:1:");
			}catch(DeserializationException ex){
				exception = ex;
			}
			Assert.IsNotNull(exception);
			Assert.AreEqual("Expected opening '\"' around position 4.", exception.Message);
			Assert.AreEqual(4, exception.Position);

			try {
				PhpSerializer.Deserialize("s:1:\"a ");
			}catch(DeserializationException ex){
				exception = ex;
			}
			Assert.IsNotNull(exception);
			Assert.AreEqual("Expected closing '\"' at position 6.", exception.Message);
			Assert.AreEqual(6, exception.Position);

			try {
				PhpSerializer.Deserialize("s:1:\"a");
			}catch(DeserializationException ex){
				exception = ex;
			}
			Assert.IsNotNull(exception);
			Assert.AreEqual("Unexpected end of data.", exception.Message);
			Assert.AreEqual(5, exception.Position);
		}
	}
}
