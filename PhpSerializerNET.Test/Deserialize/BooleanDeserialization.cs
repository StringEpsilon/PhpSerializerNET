
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Deserialize;

public class DeserializeBooleansTest {
	[Fact]
	public void DeserializesTrue() {
		Assert.Equal(true, PhpSerialization.Deserialize("b:1;"));
	}

	[Fact]
	public void DeserializesTrueExplicit() {
		Assert.True(PhpSerialization.Deserialize<bool>("b:1;"));
	}

	[Fact]
	public void DeserializesFalse() {
		Assert.Equal(false, PhpSerialization.Deserialize("b:0;"));
	}

	[Fact]
	public void DeserializesFalseExplicit() {
		Assert.False(PhpSerialization.Deserialize<bool>("b:0;"));
	}

	[Fact]
	public void DeserializesToLong() {
		var result = PhpSerialization.Deserialize<long>("b:0;");

		Assert.Equal(0, result);

		result = PhpSerialization.Deserialize<long>("b:1;");

		Assert.Equal(1, result);
	}

	[Fact]
	public void DeserializesToString() {
		var result = PhpSerialization.Deserialize<string>("b:0;");

		Assert.Equal("False", result);

		result = PhpSerialization.Deserialize<string>("b:1;");

		Assert.Equal("True", result);
	}

	[Fact]
	public void DeserializeToNullable() {
		Assert.Equal(
			false,
			PhpSerialization.Deserialize<bool?>("b:0;")
		);
	}
}