
/**
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Deserialize;

public class ObjectDeserializationTest {
	[Fact]
	public void IntegerKeysClass() {
		var result = PhpSerialization.Deserialize<MixedKeysPhpClass>(
			"O:8:\"stdClass\":4:{i:0;s:3:\"Foo\";i:1;s:3:\"Bar\";s:1:\"a\";s:1:\"A\";s:1:\"b\";s:1:\"B\";}"
		);

		Assert.NotNull(result);
		Assert.Equal("Foo", result.Foo);
		Assert.Equal("Bar", result.Bar);
		Assert.Equal("A", result.Baz);
		Assert.Equal("B", result.Dummy);
	}
}
