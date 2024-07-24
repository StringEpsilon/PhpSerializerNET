
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Deserialize;

public class IntegerDeserializationTest {
	[Theory]
	[InlineData("i:0;", 0)]
	[InlineData("i:1;", 1)]
	[InlineData("i:2147483647;", int.MaxValue)]
	[InlineData("i:-2147483648;", int.MinValue)]
	public void DeserializeZero(string input, int expected) {
		Assert.Equal(expected, PhpSerialization.Deserialize<int>(input));
	}

	[Fact]
	public void DeserializeToNullable() {
		var result = PhpSerialization.Deserialize<int?>("i:1;");
		Assert.Equal(1, result.Value);
	}

	[Fact]
	public void DeserializeIntToDouble() {
		double number = PhpSerialization.Deserialize<double>("i:10;");
		Assert.Equal(10.00, number);
	}

	[Fact]
	public void ExplictCastFormatException() {
		var ex = Assert.Throws<DeserializationException>(() =>
			PhpSerialization.Deserialize<int>(
				"s:3:\"1b1\";"
			)
		);
		Assert.IsType<System.FormatException>(ex.InnerException);
		Assert.Equal("Exception encountered while trying to assign '1b1' to type Int32. See inner exception for details.", ex.Message);
	}
}
