
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class IntegerDeserializationTest {
		[TestMethod]
		public void DeserializeZero() {
			Assert.AreEqual(
				0,
				PhpSerialization.Deserialize<int>("i:0;")
			);
		}

		[TestMethod]
		public void DeserializeOne() {
			Assert.AreEqual(
				1,
				PhpSerialization.Deserialize<int>("i:1;")
			);
		}

		[TestMethod]
		public void DeserializeIntMaxValue() {
			Assert.AreEqual(
				int.MaxValue,
				PhpSerialization.Deserialize<int>("i:2147483647;")
			);
		}

		[TestMethod]
		public void DeserializeIntMinValue() {
			Assert.AreEqual(
				int.MinValue,
				PhpSerialization.Deserialize<int>("i:-2147483648;")
			);
		}
	}
}