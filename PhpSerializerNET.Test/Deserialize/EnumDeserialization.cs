
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Deserialize;

public class EnumDeserializationTest {

	[Fact]
	public void DeserializeLongBasedEnum() {
		Assert.Equal(
			IntEnum.A,
			PhpSerialization.Deserialize<IntEnum>("i:1;")
		);
	}

	[Fact]
	public void DeserializeIntBasedEnum() {
		Assert.Equal(
			LongEnum.A,
			PhpSerialization.Deserialize<LongEnum>("i:1;")
		);
	}

	[Fact]
	public void DeserializeFromString() {
		Assert.Equal(
			LongEnum.A,
			PhpSerialization.Deserialize<LongEnum>("s:1:\"A\";")
		);
	}

	[Fact]
	public void DeserializeFromStringWithPropertyName() {
		Assert.Equal(
			IntEnumWithPropertyName.A,
			PhpSerialization.Deserialize<IntEnumWithPropertyName>("s:1:\"a\";")
		);

		Assert.Equal(
			IntEnumWithPropertyName.B,
			PhpSerialization.Deserialize<IntEnumWithPropertyName>("s:1:\"c\";")
		);

		Assert.Equal(
			IntEnumWithPropertyName.C,
			PhpSerialization.Deserialize<IntEnumWithPropertyName>("s:1:\"C\";")
		);
	}

	[Fact]
	public void DeserializeToNullable() {
		LongEnum? result = PhpSerialization.Deserialize<LongEnum?>("i:1;");
		Assert.Equal(
			LongEnum.A,
			result
		);
	}
}