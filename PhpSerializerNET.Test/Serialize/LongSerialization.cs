
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class LongSerializationTest {
		[TestMethod]
		public void SerializeIntMaxValue() {
			Assert.AreEqual(
				"i:9223372036854775807;",
				PhpSerialization.Serialize(long.MaxValue)
			);
		}
		[TestMethod]
		public void SerializeMinValue() {
			Assert.AreEqual(
				"i:-9223372036854775808;",
				PhpSerialization.Serialize(long.MinValue)
			);
		}
	}
}