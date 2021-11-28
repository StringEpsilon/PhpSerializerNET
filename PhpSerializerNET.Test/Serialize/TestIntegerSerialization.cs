
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class TestIntegerSerialization {
		[TestMethod]
		public void SerializeZero() {
			Assert.AreEqual(
				"i:0;",
				PhpSerialization.Serialize(0)
			);
		}

		[TestMethod]
		public void SerializeOne() {
			Assert.AreEqual(
				"i:1;",
				PhpSerialization.Serialize(1)
			);
		}

		[TestMethod]
		public void SerializeIntMaxValue() {
			Assert.AreEqual(
				"i:2147483647;",
				PhpSerialization.Serialize(int.MaxValue)
			);
		}

		[TestMethod]
		public void SerializeIntMinValue() {
			Assert.AreEqual(
				"i:-2147483648;",
				PhpSerialization.Serialize(int.MinValue)
			);
		}
	}
}