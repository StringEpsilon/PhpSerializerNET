/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class DeserializePrimitives {
		[TestMethod]
		public void DeserializesNull() {
			Assert.AreEqual(
				null,
				PhpSerialization.Deserialize("N;")
			);
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

		[TestMethod]
		public void DeserializeInteger() {
			Assert.AreEqual(
				0,
				PhpSerialization.Deserialize<int>("i:0;")
			);
			Assert.AreEqual(
				1,
				PhpSerialization.Deserialize<int>("i:1;")
			);
			Assert.AreEqual(
				2147483647,
				PhpSerialization.Deserialize<int>("i:2147483647;")
			);
			Assert.AreEqual(
				-2147483648,
				PhpSerialization.Deserialize<int>("i:-2147483648;")
			);
		}

		[TestMethod]
		public void DeserializeLong() {
			Assert.AreEqual(
				123456789,
				PhpSerialization.Deserialize<long>("i:123456789;")
			);
			Assert.AreEqual(
				long.MaxValue,
				PhpSerialization.Deserialize("i:9223372036854775807;")
			);
			Assert.AreEqual(
				long.MinValue,
				PhpSerialization.Deserialize("i:-9223372036854775808;")
			);
		}

		[TestMethod]
		public void DeserializeGUID() {
			Guid guid = PhpSerialization.Deserialize<Guid>("s:36:\"82e2ebf0-43e6-4c10-82cf-57d60383a6be\";");
			Assert.AreEqual("82e2ebf0-43e6-4c10-82cf-57d60383a6be", guid.ToString());
		}

		[TestMethod]
		public void DeserializesDouble() {
			Assert.AreEqual(
				1.23456789,
				PhpSerialization.Deserialize<double>("d:1.23456789;")
			);
			Assert.AreEqual(
				1.23456789,
				PhpSerialization.Deserialize("d:1.23456789;")
			);
			Assert.AreEqual(
				(double)1,
				PhpSerialization.Deserialize("d:1;")
			);
			Assert.AreEqual(
				double.MinValue,
				PhpSerialization.Deserialize("d:-1.7976931348623157E+308;")
			);
			Assert.AreEqual(
				double.MaxValue,
				PhpSerialization.Deserialize("d:1.7976931348623157E+308;")
			);
			Assert.AreEqual(
				double.PositiveInfinity,
				PhpSerialization.Deserialize("d:INF;")
			);
			Assert.AreEqual(
				double.NegativeInfinity,
				PhpSerialization.Deserialize("d:-INF;")
			);
			Assert.AreEqual(
				double.NaN,
				PhpSerialization.Deserialize("d:NAN;")
			);
		}
	}
}
