/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Serialize;

public class LongSerializationTest {
	[Fact]
	public void SerializeIntMaxValue() {
		Assert.Equal(
			"i:9223372036854775807;",
			PhpSerialization.Serialize(long.MaxValue)
		);
	}
	[Fact]
	public void SerializeMinValue() {
		Assert.Equal(
			"i:-9223372036854775808;",
			PhpSerialization.Serialize(long.MinValue)
		);
	}
}
