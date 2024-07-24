
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Serialize;

public class BooleanSerializationTest {
	[Theory]
	[InlineData(true, "b:1;")]
	[InlineData(false, "b:0;")]
	public void Serializes(bool input, string output) {
		Assert.Equal(output, PhpSerialization.Serialize(input));
	}
}
