
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Serialize;

public class EnumSerializationTest {
	[Fact]
	public void SerializeOne() {
		Assert.Equal(
			"i:1;",
			PhpSerialization.Serialize(IntEnum.A)
		);
	}

	[Fact]
	public void SerializeToString() {
		Assert.Equal(
			"s:1:\"A\";",
			PhpSerialization.Serialize(IntEnum.A, new PhpSerializiationOptions { NumericEnums = false })
		);
	}
}
