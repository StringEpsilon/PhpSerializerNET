/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET.Test.DataTypes;

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
			var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(
				() => PhpSerialization.Deserialize("")
			);

			const string expected = "PhpSerialization.Deserialize(): Parameter 'input' must not be null or empty. (Parameter 'input')";
			Assert.AreEqual(expected, ex.Message);

			ex = Assert.ThrowsException<ArgumentOutOfRangeException>(
				() => PhpSerialization.Deserialize<string>("")
			);

			Assert.AreEqual(expected, ex.Message);

			ex = Assert.ThrowsException<ArgumentOutOfRangeException>(
				() => PhpSerialization.Deserialize("", typeof(string))
			);

			Assert.AreEqual(expected, ex.Message);
		}


		[TestMethod]
		public void ThrowOnIllegalKeyType() {
			var ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize<MyPhpObject>("O:8:\"stdClass\":1:{b:1;s:4:\"true\";}")
			);
			Assert.AreEqual(
				"Error encountered deserizalizing an object of type 'PhpSerializerNET.Test.DataTypes.MyPhpObject': " +
				"The key '1' (from the token at position 18) has an unsupported type of 'Boolean'.",
				ex.Message
			);
		}

		[TestMethod]
		public void ThrowOnIntegerKeyPhpObject() {
			var ex = Assert.ThrowsException<ArgumentException>(
				() => PhpSerialization.Deserialize<PhpObjectDictionary>("O:8:\"stdClass\":1:{i:0;s:4:\"true\";}")
			);
		}
	}
}
