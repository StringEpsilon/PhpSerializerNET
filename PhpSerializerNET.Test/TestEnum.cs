
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	public enum IntEnum : int {
		A = 1,
		B,
	}
	public enum LongEnum : long {
		A = 1,
		B,
	}

	[TestClass]
	public class TestEnum {

		[TestMethod]
		public void DeserializeEnums() {
			Assert.AreEqual(
				IntEnum.A,
				PhpSerialization.Deserialize<IntEnum>("i:1;")
			);

			Assert.AreEqual(
				LongEnum.A,
				PhpSerialization.Deserialize<LongEnum>("i:1;")
			);

			Assert.AreEqual(
				LongEnum.A,
				PhpSerialization.Deserialize<LongEnum>("s:1:\"A\";")
			);
		}

		[TestMethod]
		public void SerializeOne() {
			Assert.AreEqual(
				"i:1;",
				PhpSerialization.Serialize(IntEnum.A)
			);
		}
	}
}