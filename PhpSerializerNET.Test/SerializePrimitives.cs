/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpSerializerNET;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class SerializePrimitives {
		[TestMethod]
		public void SerializesNull() {
			Assert.AreEqual(
				"N;",
				PhpSerializer.Serialize(null)
			);
		}

		[TestMethod]
		public void SerializesBool() {
			Assert.AreEqual(
				"b:1;",
				PhpSerializer.Serialize(true)
			);

			Assert.AreEqual(
				"b:0;",
				PhpSerializer.Serialize(false)
			);
		}

		[TestMethod]
		public void StringNReturnsNull() {
			var result = PhpSerializer.Deserialize<string>("N;");

			Assert.IsNull(result);
		}

		[TestMethod]
		public void LongNReturnsZero() {
			var result = PhpSerializer.Deserialize<long>("N;");

			Assert.AreEqual(0, result);
		}

		[TestMethod]
		public void ConvertsProperly() {
			var result = PhpSerializer.Deserialize<long>("b:0;");

			Assert.AreEqual(0, result);

			result = PhpSerializer.Deserialize<long>("b:1;");

			Assert.AreEqual(1, result);


			double number = PhpSerializer.Deserialize<double>("i:10;");

			Assert.AreEqual(10.00, number);
		}

		[TestMethod]
		public void SerializeInteger() {
			Assert.AreEqual(
				"i:0;",
				PhpSerializer.Serialize(0)
			);
			Assert.AreEqual(
				"i:1;",
				PhpSerializer.Serialize(1)
			);
			Assert.AreEqual(
				"i:2147483647;",
				PhpSerializer.Serialize(int.MaxValue)
			);
			Assert.AreEqual(
				"i:-2147483648;",
				PhpSerializer.Serialize(int.MinValue)
			);
		}

		[TestMethod]
		public void SerializeLong() {
			Assert.AreEqual(
				"i:9223372036854775807;",
				PhpSerializer.Serialize(long.MaxValue)
			);
			Assert.AreEqual(
				"i:-9223372036854775808;", // Note: PHP 8 serializes this to a double, but it works fine on deserialization.
				PhpSerializer.Serialize(long.MinValue)
			);
		}

		[TestMethod]
		public void SerializesDouble() {
			Assert.AreEqual(
				"d:1.23456789;",
				PhpSerializer.Serialize(1.23456789)
			);
			Assert.AreEqual(
				"d:1;",
				PhpSerializer.Serialize((double)1)
			);
			Assert.AreEqual(
				"d:-1.7976931348623157E+308;",
				PhpSerializer.Serialize(double.MinValue)
			);
			Assert.AreEqual(
				"d:1.7976931348623157E+308;",
				PhpSerializer.Serialize(double.MaxValue)
			);
			Assert.AreEqual(
				"d:INF;",
				PhpSerializer.Serialize(double.PositiveInfinity)
			);
			Assert.AreEqual(
				"d:-INF;",
				PhpSerializer.Serialize(double.NegativeInfinity)
			);
			Assert.AreEqual(
				"d:NAN;",
				PhpSerializer.Serialize(double.NaN)
			);
		}

	}
}
