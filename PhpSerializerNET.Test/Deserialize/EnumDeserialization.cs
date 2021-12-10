
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Deserialize {

	[TestClass]
	public class EnumDeserializationTest{

		[TestMethod]
		public void DeserializeLongBasedEnum() {
			Assert.AreEqual(
				IntEnum.A,
				PhpSerialization.Deserialize<IntEnum>("i:1;")
			);
		}
		
		[TestMethod]
		public void DeserializeIntBasedEnum() {
			Assert.AreEqual(
				LongEnum.A,
				PhpSerialization.Deserialize<LongEnum>("i:1;")
			);
		}

		[TestMethod]
		public void DeserializeFromString() {
			Assert.AreEqual(
				LongEnum.A,
				PhpSerialization.Deserialize<LongEnum>("s:1:\"A\";")
			);
		}
	}
}