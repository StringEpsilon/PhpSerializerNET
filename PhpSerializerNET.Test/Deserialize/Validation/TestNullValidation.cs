
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class TestNullValidation {
		[TestMethod]
		public void ThrowsOnTruncatedInput() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("N"));
			Assert.AreEqual("Malformed null at position 0: Expected ';'", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("N?"));
			Assert.AreEqual("Malformed null at position 0: Expected ';'", ex.Message);
		}

	}
}