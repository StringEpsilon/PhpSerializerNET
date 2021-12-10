/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Deserialize.Validation {
	[TestClass]
	public class TestOtherErrors {
		[TestMethod]
		public void ThrowsOnUnexpectedToken() {
			var ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("_"));
			Assert.AreEqual("Unexpected token '_' at position 0.", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("i:42;_"));
			Assert.AreEqual("Unexpected token '_' at position 5.", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(() => PhpSerialization.Deserialize("_i:42;"));
			Assert.AreEqual("Unexpected token '_' at position 0.", ex.Message);
		}

		[TestMethod]
		public void ErrorOnTuple() {
			var ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize("s:7:\"AString\";s:7:\"AString\";")
			);

			Assert.AreEqual("Unexpected token 's' at position 14.", ex.Message);
		}

		[TestMethod]
		public void ErrorOnEmptyInput() {
			var ex = Assert.ThrowsException<ArgumentException>(
				() => PhpSerialization.Deserialize("")
			);

			Assert.AreEqual("PhpSerialization.Deserialize(): Parameter 'input' must not be null or empty.", ex.Message);

			ex = Assert.ThrowsException<ArgumentException>(
				() => PhpSerialization.Deserialize<string>("")
			);

			Assert.AreEqual("PhpSerialization.Deserialize(): Parameter 'input' must not be null or empty.", ex.Message);
		}
	}
}
