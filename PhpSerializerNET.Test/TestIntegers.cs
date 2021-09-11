
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class TestIntegers {
		[TestMethod]
		public void SerializeZero() {
			Assert.AreEqual(
				"i:0;",
				PhpSerializer.Serialize(0)
			);
		}

		[TestMethod]
		public void DeserializeZero() {
			Assert.AreEqual(
				0,
				PhpSerializer.Deserialize<int>("i:0;")
			);
		}

		[TestMethod]
		public void SerializeOne() {
			Assert.AreEqual(
				"i:1;",
				PhpSerializer.Serialize(1)
			);
		}

		[TestMethod]
		public void DeserializeOne() {
			Assert.AreEqual(
				1,
				PhpSerializer.Deserialize<int>("i:1;")
			);
		}

		[TestMethod]
		public void SerializeIntMaxValue() {
			Assert.AreEqual(
				"i:2147483647;",
				PhpSerializer.Serialize(int.MaxValue)
			);
		}

		[TestMethod]
		public void DeserializeIntMaxValue() {
			Assert.AreEqual(
				int.MaxValue,
				PhpSerializer.Deserialize<int>("i:2147483647;")
			);
		}

		[TestMethod]
		public void SerializeIntMinValue() {
			Assert.AreEqual(
				"i:-2147483648;",
				PhpSerializer.Serialize(int.MinValue)
			);
		}

		[TestMethod]
		public void DeserializeIntMinValue() {
			Assert.AreEqual(
				int.MinValue,
				PhpSerializer.Deserialize<int>("i:-2147483648;")
			);
		}
	}
}