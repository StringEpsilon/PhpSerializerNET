/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Deserialize;

public class DeserializeStructsTest {
	[Fact]
	public void DeserializeArrayToStruct() {
		var value = PhpSerialization.Deserialize<AStruct>(
			"a:2:{s:3:\"foo\";s:3:\"Foo\";s:3:\"bar\";s:3:\"Bar\";}"
		);
		Assert.Equal("Foo", value.foo);
		Assert.Equal("Bar", value.bar);
	}

	[Fact]
	public void DeserializeObjectToStruct() {
		var value = PhpSerialization.Deserialize<AStruct>(
			"O:8:\"sdtClass\":2:{s:3:\"foo\";s:3:\"Foo\";s:3:\"bar\";s:3:\"Bar\";}"
		);
		Assert.Equal("Foo", value.foo);
		Assert.Equal("Bar", value.bar);
	}


	[Fact]
	public void DeserializeWithIgnoredField() {
		var value = PhpSerialization.Deserialize<AStructWithIgnore>(
			"a:2:{s:3:\"foo\";s:3:\"Foo\";s:3:\"bar\";s:3:\"Bar\";}"
		);
		Assert.Equal("Foo", value.foo);
		Assert.Null(value.bar);
	}

	[Fact]
	public void DeserializePropertyName() {
		var value = PhpSerialization.Deserialize<AStructWithRename>(
			"a:2:{s:3:\"foo\";s:3:\"Foo\";s:6:\"foobar\";s:3:\"Bar\";}"
		);
		Assert.Equal("Foo", value.foo);
		Assert.Equal("Bar", value.bar);
	}

	[Fact]
	public void DeserializeBoolToStruct() {
		var ex = Assert.Throws<DeserializationException>(
			() => PhpSerialization.Deserialize<AStruct>(
				"b:1;"
			)
		);

		Assert.Equal(
			"Can not assign value \"1\" (at position 0) to target type of AStruct.",
			ex.Message
		);
	}

	[Fact]
	public void DeserializeStringToStruct() {
		var ex = Assert.Throws<DeserializationException>(
			() => PhpSerialization.Deserialize<AStruct>(
				"s:3:\"foo\";"
			)
		);

		Assert.Equal(
			"Can not assign value \"foo\" (at position 0) to target type of AStruct.",
			ex.Message
		);
	}

	[Fact]
	public void DeserializeNullToStruct() {
		Assert.Equal(
			default,
			PhpSerialization.Deserialize<AStruct>(
				"N;"
			)
		);
	}
}