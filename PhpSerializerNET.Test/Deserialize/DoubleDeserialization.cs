

/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Deserialize {
	[TestClass]
	public class DoubleDeserializationTest {

		[TestMethod]
		public void DeserializesNormalValue() {
			Assert.AreEqual(
				1.23456789,
				PhpSerialization.Deserialize<double>("d:1.23456789;")
			);
			Assert.AreEqual(
				1.23456789,
				PhpSerialization.Deserialize("d:1.23456789;")
			);
		}

		[TestMethod]
		public void DeserializesOne() {
			Assert.AreEqual(
				(double)1,
				PhpSerialization.Deserialize("d:1;")
			);
		}

		[TestMethod]
		public void DeserializesMinValue() {
			Assert.AreEqual(
				double.MinValue,
				PhpSerialization.Deserialize("d:-1.7976931348623157E+308;")
			);
		}

		[TestMethod]
		public void DeserializesMaxValue() {
			Assert.AreEqual(
				double.MaxValue,
				PhpSerialization.Deserialize("d:1.7976931348623157E+308;")
			);
		}

		[TestMethod]
		public void DeserializesInfinity() {
			Assert.AreEqual(
				double.PositiveInfinity,
				PhpSerialization.Deserialize("d:INF;")
			);
		}

		[TestMethod]
		public void DeserializesNegativeInfinity() {
			Assert.AreEqual(
				double.NegativeInfinity,
				PhpSerialization.Deserialize("d:-INF;")
			);
		}

		[TestMethod]
		public void DeserializesNotANumber() {
			Assert.AreEqual(
				double.NaN,
				PhpSerialization.Deserialize("d:NAN;")
			);
		}

		[TestMethod]
		public void Explicit_DeserializesInfinity() {
			Assert.AreEqual(
				double.PositiveInfinity,
				PhpSerialization.Deserialize<double>("d:INF;")
			);
		}

		[TestMethod]
		public void Explicit_DeserializesNegativeInfinity() {
			Assert.AreEqual(
				double.NegativeInfinity,
				PhpSerialization.Deserialize<double>("d:-INF;")
			);
		}

		[TestMethod]
		public void Explicit_DeserializesNotANumber() {
			Assert.AreEqual(
				double.NaN,
				PhpSerialization.Deserialize<double>("d:NAN;")
			);
		}

		[TestMethod]
		public void Explicit_Nullable_DeserializesInfinity() {
			Assert.AreEqual(
				double.PositiveInfinity,
				PhpSerialization.Deserialize<double?>("d:INF;")
			);
		}

		[TestMethod]
		public void Explicit_Nullable_DeserializesNegativeInfinity() {
			Assert.AreEqual(
				double.NegativeInfinity,
				PhpSerialization.Deserialize<double?>("d:-INF;")
			);
		}

		[TestMethod]
		public void Explicit_Nullable_DeserializesNotANumber() {
			Assert.AreEqual(
				double.NaN,
				PhpSerialization.Deserialize<double?>("d:NAN;")
			);
		}

		[TestMethod]
		public void DeserializesToNullable() {
			Assert.AreEqual(
				3.1415,
				PhpSerialization.Deserialize<double?>("d:3.1415;")
			);
		}

		[TestMethod]
		public void DeserializeDoubleToInt() {
			double number = PhpSerialization.Deserialize<double>("d:10;");
			Assert.AreEqual((long)10, number);
		}

	}
}