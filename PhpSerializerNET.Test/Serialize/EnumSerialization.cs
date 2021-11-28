
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test {

	[TestClass]
	public class EnumSerializationTest {
		[TestMethod]
		public void SerializeOne() {
			Assert.AreEqual(
				"i:1;",
				PhpSerialization.Serialize(IntEnum.A)
			);
		}

		[TestMethod]
		public void SerializeToString() {
			Assert.AreEqual(
				"s:1:\"A\";",
				PhpSerialization.Serialize(IntEnum.A, new PhpSerializiationOptions{NumericEnums = false})
			);
		}
	}
}