
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class TestLongs {

		[TestMethod]
		public void DeserializeZero() {
			Assert.AreEqual(
				(long)0,
				PhpSerializer.Deserialize("i:0;")
			);
		}

		[TestMethod]
		public void DeserializeOne() {
			Assert.AreEqual(
				(long)1,
				PhpSerializer.Deserialize("i:1;")
			);
		}

		[TestMethod]
		public void SerializeIntMaxValue() {
			Assert.AreEqual(
				"i:9223372036854775807;",
				PhpSerializer.Serialize(long.MaxValue)
			);
		}

		[TestMethod]
		public void DeserializeIntMaxValue() {
			Assert.AreEqual(
				long.MaxValue,
				PhpSerializer.Deserialize("i:9223372036854775807;")
			);
		}

		[TestMethod]
		public void SerializeIntMinValue() {
			Assert.AreEqual(
				"i:-9223372036854775808;",
				PhpSerializer.Serialize(long.MinValue)
			);
		}

		[TestMethod]
		public void DeserializeIntMinValue() {
			Assert.AreEqual(
				long.MinValue,
				PhpSerializer.Deserialize("i:-9223372036854775808;")
			);
		}

		[TestMethod]
		public void DeserializesNullToZero() {
			var result = PhpSerializer.Deserialize<long>("N;");

			Assert.AreEqual(0, result);
		}
	}
}