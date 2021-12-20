
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Deserialize {
	[TestClass]
	public class LongDeserializationTest {

		[TestMethod]
		public void DeserializeZero() {
			Assert.AreEqual(
				(long)0,
				PhpSerialization.Deserialize("i:0;")
			);
		}

		[TestMethod]
		public void DeserializeOne() {
			Assert.AreEqual(
				(long)1,
				PhpSerialization.Deserialize("i:1;")
			);
		}


		[TestMethod]
		public void DeserializeMaxValue() {
			Assert.AreEqual(
				long.MaxValue,
				PhpSerialization.Deserialize("i:9223372036854775807;")
			);
		}

		[TestMethod]
		public void DeserializeMinValue() {
			Assert.AreEqual(
				long.MinValue,
				PhpSerialization.Deserialize("i:-9223372036854775808;")
			);
		}

		[TestMethod]
		public void DeserializesNullToZero() {
			var result = PhpSerialization.Deserialize<long>("N;");

			Assert.AreEqual(0, result);
		}

		[TestMethod]
		public void DeserializesToNullable() {
			var result = PhpSerialization.Deserialize<long?>("N;");

			Assert.AreEqual(null, result);

			result = PhpSerialization.Deserialize<long?>("i:1;");

			Assert.AreEqual(1, result);
		}

		[TestMethod]
		public void SupportsOtherNumberTypes() {
			Assert.AreEqual(
				short.MinValue,
				PhpSerialization.Deserialize<short>($"i:{short.MinValue};")
			);
			Assert.AreEqual(
				short.MaxValue,
				PhpSerialization.Deserialize<short>($"i:{short.MaxValue};")
			);

			Assert.AreEqual(
				ushort.MinValue,
				PhpSerialization.Deserialize<ushort>($"i:{ushort.MinValue};")
			);
			Assert.AreEqual(
				ushort.MaxValue,
				PhpSerialization.Deserialize<ushort>($"i:{ushort.MaxValue};")
			);

			Assert.AreEqual(
				uint.MinValue,
				PhpSerialization.Deserialize<uint>($"i:{uint.MinValue};")
			);
			Assert.AreEqual(
				uint.MaxValue,
				PhpSerialization.Deserialize<uint>($"i:{uint.MaxValue};")
			);

			Assert.AreEqual(
				ulong.MinValue,
				PhpSerialization.Deserialize<ulong>($"i:{ulong.MinValue};")
			);
			Assert.AreEqual(
				ulong.MaxValue,
				PhpSerialization.Deserialize<ulong>($"i:{ulong.MaxValue};")
			);

			Assert.AreEqual(
				sbyte.MinValue,
				PhpSerialization.Deserialize<sbyte>($"i:{sbyte.MinValue};")
			);
			Assert.AreEqual(
				sbyte.MaxValue,
				PhpSerialization.Deserialize<sbyte>($"i:{sbyte.MaxValue};")
			);
		}
	}
}