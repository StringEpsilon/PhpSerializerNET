/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Serialize;

public class NullSerializationTest {
	[Fact]
	public void SerializesNull() {
		Assert.Equal(
			"N;",
			PhpSerialization.Serialize(null)
		);
	}
}
