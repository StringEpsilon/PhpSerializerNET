
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Serialize;

public class IntegerSerializationTest {
	[Fact]
	public void SerializeZero() {
		Assert.Equal(
			"i:0;",
			PhpSerialization.Serialize(0)
		);
	}

	[Fact]
	public void SerializeOne() {
		Assert.Equal(
			"i:1;",
			PhpSerialization.Serialize(1)
		);
	}

	[Fact]
	public void SerializeIntMaxValue() {
		Assert.Equal(
			"i:2147483647;",
			PhpSerialization.Serialize(int.MaxValue)
		);
	}

	[Fact]
	public void SerializeIntMinValue() {
		Assert.Equal(
			"i:-2147483648;",
			PhpSerialization.Serialize(int.MinValue)
		);
	}
}