/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]

	public class TestDoubles {
		[TestMethod]
		public void SerializesDecimalValue() {
			Assert.AreEqual(
				"d:1.23456789;",
				PhpSerialization.Serialize(1.23456789)
			);
		}

		[TestMethod]
		public void SerializesOne() {
			Assert.AreEqual(
				"d:1;",
				PhpSerialization.Serialize((double)1)
			);
		}

		[TestMethod]
		public void SerializesMinValue() {
			Assert.AreEqual(
				"d:-1.7976931348623157E+308;",
				PhpSerialization.Serialize(double.MinValue)
			);
		}

		[TestMethod]
		public void SerializesMaxValue() {
			Assert.AreEqual(
				"d:1.7976931348623157E+308;",
				PhpSerialization.Serialize(double.MaxValue)
			);
		}

		[TestMethod]
		public void SerializesInfinity() {
			Assert.AreEqual(
				"d:INF;",
				PhpSerialization.Serialize(double.PositiveInfinity)
			);
		}

		[TestMethod]
		public void SerializesNegativeInfinity() {
			Assert.AreEqual(
				"d:-INF;",
				PhpSerialization.Serialize(double.NegativeInfinity)
			);
		}

		[TestMethod]
		public void SerializesNaN() {
			Assert.AreEqual(
				"d:NAN;",
				PhpSerialization.Serialize(double.NaN)
			);
		}

		[TestMethod]
		public void ThrowsOnMissingSemicolon() {
			var ex = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize("d:100")
			);
			Assert.AreEqual("Malformed double at position 4: Expected token ';', found '0' instead.", ex.Message);
		}
	}
}