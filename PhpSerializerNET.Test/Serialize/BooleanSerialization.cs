
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Serialize {
	[TestClass]
	public class BooleanSerializationTest {
		[TestMethod]
		public void SerializesTrue() {
			Assert.AreEqual(
				"b:1;",
				PhpSerialization.Serialize(true)
			);
		}
		
		[TestMethod]
		public void SerializesFalse() {
			Assert.AreEqual(
				"b:0;",
				PhpSerialization.Serialize(false)
			);
		}
	}
}