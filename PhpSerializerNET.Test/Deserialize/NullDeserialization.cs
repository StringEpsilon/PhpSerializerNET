

/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Deserialize;

public class NullDeserializationTest {
	[Fact]
	public void DeserializesNull() {
		var result = PhpSerialization.Deserialize("N;");

		Assert.Null(result);
	}

	[Fact]
	public void DeserializesExplicitNull() {
		var result = PhpSerialization.Deserialize<SimpleClass>("N;");
		Assert.Null(result);
	}

	[Fact]
	public void DeserializesToNullableStruct() {
		var result = PhpSerialization.Deserialize<AStruct?>("N;");
		Assert.Null(result);
	}

	[Fact]
	public void ExplicitToPrimitiveDefaultValues() {
		Assert.False(PhpSerialization.Deserialize<bool>("N;"));
		Assert.Equal(0, PhpSerialization.Deserialize<double>("N;"));
		Assert.Null(PhpSerialization.Deserialize<string>("N;"));
	}
}