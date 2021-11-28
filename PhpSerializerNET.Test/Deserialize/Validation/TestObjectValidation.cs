/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class TestObjectValidation {
		[TestMethod]
		public void ErrorOnInvalidNameLength() {
			var ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize(
					"O:-1:\"stdClass\":1:{s:3:\"Foo\";N;}"
				)
			);

			Assert.AreEqual("Object at position 3 has illegal, missing or malformed length for classname.", ex.Message);

			ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize(
					"O:200:\"stdClass\":1:{s:3:\"Foo\";N;}"
				)
			);

			Assert.AreEqual(
				"Malformed object at position 5: Illegal length of 200. The specified length points to out of bounds index 207.",
				ex.Message
			);
		}

		[TestMethod]
		public void ErrorOnInvalidName() {
			var ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize(
					"O:8:\"stdClass:1:{s:3:\"Foo\";N;}"
				)
			);

			Assert.AreEqual(
				"Malformed object at position 4 has an incorrect length of 8: Expected double quote at position 13, found ':' instead.",
				ex.Message
			);

			ex = Assert.ThrowsException<DeserializationException>(
			   () => PhpSerialization.Deserialize(
				   "O:2:stdClass\":1:{s:3:\"Foo\";N;}"
			   )
		   );

			Assert.AreEqual(
				"Malformed object at position 3: Expected double quote at position 4, found 's' instead.",
				ex.Message
			);
		}

		[TestMethod]
		public void ErrorOnInvalidSyntax() {
			var ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize(
					"O:8:\"stdClass\"1:{s:3:\"Foo\";N;}"
				)
			);

			Assert.AreEqual(
				"Malformed object at position 14: Expected ':', found 1 instead.",
				ex.Message
			);

			ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize(
					"O:8:\"stdClass\":1{s:3:\"Foo\";N;}"
				)
			);

			Assert.AreEqual(
				"Object at position 17 has illegal, missing or malformed length.",
				ex.Message
			);

			ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize(
					"O:8:\"stdClass\":1:s:3:\"Foo\";N;}"
				)
			);

			Assert.AreEqual(
				"Object at position 17: Expected token '{', found 's' instead.",
				ex.Message
			);
		}
	}
}