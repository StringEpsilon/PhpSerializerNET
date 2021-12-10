/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Serialize {
	[TestClass]
	public class NullSerializationTest {
		[TestMethod]
		public void SerializesNull() {
			Assert.AreEqual(
				"N;",
				PhpSerialization.Serialize(null)
			);
		}
	}
}