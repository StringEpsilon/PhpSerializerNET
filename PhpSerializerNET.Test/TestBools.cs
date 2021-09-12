
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class TestBools {
		[TestMethod]
		public void SerializesTrue() {
			Assert.AreEqual(
				"b:1;",
				PhpSerializer.Serialize(true)
			);
		}

		[TestMethod]
		public void DeserializesTrue() {
			Assert.AreEqual(
				true,
				PhpSerializer.Deserialize("b:1;")
			);
		}

		[TestMethod]
		public void DeserializesTrueExplicit() {

			Assert.AreEqual(
				true,
				PhpSerializer.Deserialize<bool>("b:1;")
			);
		}

		[TestMethod]
		public void SerializesFalse() {
			Assert.AreEqual(
				"b:0;",
				PhpSerializer.Serialize(false)
			);
		}

		[TestMethod]
		public void DeserializesFalse() {
			Assert.AreEqual(
				false,
				PhpSerializer.Deserialize("b:0;")
			);
		}

		[TestMethod]
		public void DeserializesFalseExplicit() {
			Assert.AreEqual(
				false,
				PhpSerializer.Deserialize<bool>("b:0;")
			);
		}
	}
}