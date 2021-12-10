
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Deserialize {
	[TestClass]
	public class DeserializeBooleansTest {
		[TestMethod]
		public void DeserializesTrue() {
			Assert.AreEqual(
				true,
				PhpSerialization.Deserialize("b:1;")
			);
		}

		[TestMethod]
		public void DeserializesTrueExplicit() {

			Assert.AreEqual(
				true,
				PhpSerialization.Deserialize<bool>("b:1;")
			);
		}

		[TestMethod]
		public void DeserializesFalse() {
			Assert.AreEqual(
				false,
				PhpSerialization.Deserialize("b:0;")
			);
		}

		[TestMethod]
		public void DeserializesFalseExplicit() {
			Assert.AreEqual(
				false,
				PhpSerialization.Deserialize<bool>("b:0;")
			);
		}

		[TestMethod]
		public void DeserializesToLong() {
			var result = PhpSerialization.Deserialize<long>("b:0;");

			Assert.AreEqual(0, result);

			result = PhpSerialization.Deserialize<long>("b:1;");

			Assert.AreEqual(1, result);
		}

		[TestMethod]
		public void DeserializesToString() {
			var result = PhpSerialization.Deserialize<string>("b:0;");

			Assert.AreEqual("False", result);

			result = PhpSerialization.Deserialize<string>("b:1;");

			Assert.AreEqual("True", result);
		}
	}
}