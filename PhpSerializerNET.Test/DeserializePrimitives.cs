/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test
{
	[TestClass]
	public class DeserializePrimitives
	{
		[TestMethod]
		public void DeserializesNull()
		{
			Assert.AreEqual(
				null,
				PhpSerializer.Deserialize("N;")
			);
		}

		[TestMethod]
		public void DeserializesBool()
		{
			Assert.AreEqual(
				true,
				PhpSerializer.Deserialize("b:1;")
			);

			Assert.AreEqual(
				false,
				PhpSerializer.Deserialize("b:0;")
			);

			Assert.AreEqual(
				true,
				PhpSerializer.Deserialize<bool>("b:1;")
			);

			Assert.AreEqual(
				false,
				PhpSerializer.Deserialize<bool>("b:0;")
			);
		}


		[TestMethod]
		public void DeserializeInteger()
		{
			Assert.AreEqual(
				0,
				PhpSerializer.Deserialize<int>("i:0;")
			);
			Assert.AreEqual(
				1,
				PhpSerializer.Deserialize<int>("i:1;")
			);
			Assert.AreEqual(
				2147483647,
				PhpSerializer.Deserialize<int>("i:2147483647;")
			);
			Assert.AreEqual(
				-2147483648,
				PhpSerializer.Deserialize<int>("i:-2147483648;")
			);
		}

		[TestMethod]
		public void DeserializeLong()
		{
			Assert.AreEqual(
				123456789,
				PhpSerializer.Deserialize<long>("i:123456789;")
			);
			Assert.AreEqual(
				long.MaxValue,
				PhpSerializer.Deserialize("i:9223372036854775807;")
			);
			Assert.AreEqual(
				long.MinValue,
				PhpSerializer.Deserialize("i:-9223372036854775808;")
			);
		}

		[TestMethod]
		public void DeserializesDouble()
		{
			Assert.AreEqual(
				1.23456789,
				PhpSerializer.Deserialize<double>("d:1.23456789;")
			);
			Assert.AreEqual(
				1.23456789,
				PhpSerializer.Deserialize("d:1.23456789;")
			);
			Assert.AreEqual(
				(double)1,
				PhpSerializer.Deserialize("d:1;")
			);
			Assert.AreEqual(
				double.MinValue,
				PhpSerializer.Deserialize("d:-1.7976931348623157E+308;")
			);
			Assert.AreEqual(
				double.MaxValue,
				PhpSerializer.Deserialize("d:1.7976931348623157E+308;")
			);
			Assert.AreEqual(
				double.PositiveInfinity,
				PhpSerializer.Deserialize("d:INF;")
			);
			Assert.AreEqual(
				double.NegativeInfinity,
				PhpSerializer.Deserialize("d:-INF;")
			);
			Assert.AreEqual(
				double.NaN,
				PhpSerializer.Deserialize("d:NAN;")
			);
		}

		[TestMethod]
		public void DeserializesString()
		{
			var greeting = PhpSerializer.Deserialize<string>("s:12:\"Hello World!\";");
			Assert.AreEqual("Hello World!", greeting);

			Assert.AreEqual(
				"Hello World!",
				PhpSerializer.Deserialize("s:12:\"Hello World!\";")
			);
			Assert.AreEqual(
				"",
				PhpSerializer.Deserialize("s:0:\"\";")
			);
			Assert.AreEqual(
				"äöüßÄÖÜ",
				PhpSerializer.Deserialize("s:14:\"äöüßÄÖÜ\";")
			);
			Assert.AreEqual(
				"👻",
				PhpSerializer.Deserialize("s:4:\"👻\";")
			);
		}
	}
}
