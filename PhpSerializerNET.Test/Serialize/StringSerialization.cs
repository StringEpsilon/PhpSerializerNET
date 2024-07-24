
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Serialize;
public class StringSerializationTest {
	[Fact]
	public void SerializeHelloWorld() {
		Assert.Equal(
			"s:12:\"Hello World!\";",
			PhpSerialization.Serialize("Hello World!")
		);
	}

	[Fact]
	public void SerializeEmptyString() {
		Assert.Equal(
			"s:0:\"\";",
			PhpSerialization.Serialize("")
		);
	}

	[Fact]
	public void SerializeUmlauts() {
		Assert.Equal(
			"s:14:\"äöüßÄÖÜ\";",
			PhpSerialization.Serialize("äöüßÄÖÜ")
		);
	}

	[Fact]
	public void SerializeEmoji() {
		Assert.Equal(
			"s:4:\"👻\";",
			PhpSerialization.Serialize("👻")
		);
	}
}
