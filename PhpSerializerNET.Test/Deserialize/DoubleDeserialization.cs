

/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Deserialize;

public class DoubleDeserializationTest {

	[Fact]
	public void DeserializesNormalValue() {
		Assert.Equal(
			1.23456789,
			PhpSerialization.Deserialize<double>("d:1.23456789;")
		);
		Assert.Equal(
			1.23456789,
			PhpSerialization.Deserialize("d:1.23456789;")
		);
	}

	[Fact]
	public void DeserializesOne() {
		Assert.Equal(
			(double)1,
			PhpSerialization.Deserialize("d:1;")
		);
	}

	[Fact]
	public void DeserializesMinValue() {
		Assert.Equal(
			double.MinValue,
			PhpSerialization.Deserialize("d:-1.7976931348623157E+308;")
		);
	}

	[Fact]
	public void DeserializesMaxValue() {
		Assert.Equal(
			double.MaxValue,
			PhpSerialization.Deserialize("d:1.7976931348623157E+308;")
		);
	}

	[Fact]
	public void DeserializesInfinity() {
		Assert.Equal(
			double.PositiveInfinity,
			PhpSerialization.Deserialize("d:INF;")
		);
	}

	[Fact]
	public void DeserializesNegativeInfinity() {
		Assert.Equal(
			double.NegativeInfinity,
			PhpSerialization.Deserialize("d:-INF;")
		);
	}

	[Fact]
	public void DeserializesNotANumber() {
		Assert.Equal(
			double.NaN,
			PhpSerialization.Deserialize("d:NAN;")
		);
	}

	[Fact]
	public void Explicit_DeserializesInfinity() {
		Assert.Equal(
			double.PositiveInfinity,
			PhpSerialization.Deserialize<double>("d:INF;")
		);
	}

	[Fact]
	public void Explicit_DeserializesNegativeInfinity() {
		Assert.Equal(
			double.NegativeInfinity,
			PhpSerialization.Deserialize<double>("d:-INF;")
		);
	}

	[Fact]
	public void Explicit_DeserializesNotANumber() {
		Assert.Equal(
			double.NaN,
			PhpSerialization.Deserialize<double>("d:NAN;")
		);
	}

	[Fact]
	public void Explicit_Nullable_DeserializesInfinity() {
		Assert.Equal(
			double.PositiveInfinity,
			PhpSerialization.Deserialize<double?>("d:INF;")
		);
	}

	[Fact]
	public void Explicit_Nullable_DeserializesNegativeInfinity() {
		Assert.Equal(
			double.NegativeInfinity,
			PhpSerialization.Deserialize<double?>("d:-INF;")
		);
	}

	[Fact]
	public void Explicit_Nullable_DeserializesNotANumber() {
		Assert.Equal(
			double.NaN,
			PhpSerialization.Deserialize<double?>("d:NAN;")
		);
	}

	[Fact]
	public void DeserializesToNullable() {
		Assert.Equal(
			3.1415,
			PhpSerialization.Deserialize<double?>("d:3.1415;")
		);
	}

	[Fact]
	public void DeserializeDoubleToInt() {
		double number = PhpSerialization.Deserialize<double>("d:10;");
		Assert.Equal(10, number);
	}

}