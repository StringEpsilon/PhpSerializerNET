
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Deserialize.Validation {
	[TestClass]
	public class TestNullValidation {
		[TestMethod]
		public void ThrowsOnTruncatedInput() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("N"));
			Assert.AreEqual("Unexpected end of input. Expected ';' at index 1, but input ends at index 0", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("N?"));
			Assert.AreEqual("Unexpected token at index 1. Expected ';' but found '?' instead.", ex.Message);
		}

	}
}