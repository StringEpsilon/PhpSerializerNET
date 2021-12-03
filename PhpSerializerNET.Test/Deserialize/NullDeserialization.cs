		
		
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class NullDeserializationTest {
		[TestMethod]
		public void DeserializesNull() {
			var result = PhpSerialization.Deserialize("N;");

			Assert.IsNull(result);
		}

		[TestMethod]
		public void DeserializesExplicitNull() {
			var result = PhpSerialization.Deserialize<SimpleClass>("N;");

			Assert.IsNull(result);
		}
	
		[TestMethod]
		public void ExplicitToPrimitiveDefaultValues() {
			Assert.AreEqual(
				0,
				PhpSerialization.Deserialize<long>("N;")
			);
			Assert.AreEqual(
				false,
				PhpSerialization.Deserialize<bool>("N;")
			);
			Assert.AreEqual(
				0,
				PhpSerialization.Deserialize<double>("N;")
			);
			Assert.AreEqual(
				null,
				PhpSerialization.Deserialize<string>("N;")
			);
		}
	}
}