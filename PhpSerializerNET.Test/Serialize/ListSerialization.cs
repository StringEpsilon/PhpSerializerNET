/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using Xunit;

namespace PhpSerializerNET.Test.Serialize;

public class ListSerializationTest {
	[Fact]
	public void SerializeListOfStrings() {
		Assert.Equal( // strings:
			"a:2:{i:0;s:5:\"Hello\";i:1;s:5:\"World\";}",
			PhpSerialization.Serialize(new List<string>() { "Hello", "World" })
		);
	}

	[Fact]
	public void SerializeListOfBools() {
		Assert.Equal( // booleans:
			"a:2:{i:0;b:1;i:1;b:0;}",
			PhpSerialization.Serialize(new List<object>() { true, false })
		);
	}

	[Fact]
	public void SerializeMixedList() {
		Assert.Equal( // mixed types:
			"a:5:{i:0;b:1;i:1;i:1;i:2;d:1.23;i:3;s:3:\"end\";i:4;N;}",
			PhpSerialization.Serialize(new List<object>() { true, 1, 1.23, "end", null })
		);
	}
}
