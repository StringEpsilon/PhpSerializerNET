/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Serialize;

public class DoubleSerializationTest {
	[Fact]
	public void SerializesDecimalValue() {
		Assert.Equal(
			"d:1.23456789;",
			PhpSerialization.Serialize(1.23456789)
		);
	}

	[Fact]
	public void SerializesOne() {
		Assert.Equal(
			"d:1;",
			PhpSerialization.Serialize((double)1)
		);
	}

	[Fact]
	public void SerializesMinValue() {
		Assert.Equal(
			"d:-1.7976931348623157E+308;",
			PhpSerialization.Serialize(double.MinValue)
		);
	}

	[Fact]
	public void SerializesMaxValue() {
		Assert.Equal(
			"d:1.7976931348623157E+308;",
			PhpSerialization.Serialize(double.MaxValue)
		);
	}

	[Fact]
	public void SerializesInfinity() {
		Assert.Equal(
			"d:INF;",
			PhpSerialization.Serialize(double.PositiveInfinity)
		);
	}

	[Fact]
	public void SerializesNegativeInfinity() {
		Assert.Equal(
			"d:-INF;",
			PhpSerialization.Serialize(double.NegativeInfinity)
		);
	}

	[Fact]
	public void SerializesNaN() {
		Assert.Equal(
			"d:NAN;",
			PhpSerialization.Serialize(double.NaN)
		);
	}
}