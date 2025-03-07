
/**
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Deserialize;

public class ReferenceDeserializationTest {
	[Fact]
	public void InObject() {
		var result = PhpSerialization.Deserialize<MixedKeysPhpClass>(
			"""O:8:"stdClass":4:{i:0;s:3:"Foo";i:1;R:2;s:1:"a";s:1:"A";s:1:"b";R:3;}"""
		);
		Assert.NotNull(result);
		Assert.NotNull(result);
		Assert.Equal("Foo", result.Foo);
		Assert.Equal("Foo", result.Bar);
		Assert.Equal("A", result.Baz);
		Assert.Equal("A", result.Dummy);

		result = PhpSerialization.Deserialize<MixedKeysPhpClass>(
			"""O:8:"stdClass":4:{i:0;s:3:"Foo";i:1;R:2;s:1:"a";s:1:"A";s:1:"b";R:2;}"""
		);
		Assert.NotNull(result);
		Assert.NotNull(result);
		Assert.Equal("Foo", result.Foo);
		Assert.Equal("Foo", result.Bar);
		Assert.Equal("A", result.Baz);
		Assert.Equal("Foo", result.Dummy);
	}

	[Fact]
	public void InArray() {
		var value = PhpSerialization.Deserialize<AStruct>(
			"a:2:{s:3:\"foo\";s:3:\"Foo\";s:3:\"bar\";R:2;}"
		);
		Assert.Equal("Foo", value.foo);
		Assert.Equal("Foo", value.bar);
	}

	[Fact]
	public void ReferencingArray() {
		var value = PhpSerialization.Deserialize<BStruct>(
			"""a:2:{s:5:"First";a:2:{s:3:"foo";s:3:"one";s:3:"bar";s:3:"two";}s:6:"Second";R:2;}"""
		);

		Assert.Equal("one", value.First.foo);
		Assert.Equal("two", value.First.bar);
		Assert.Equal("one", value.Second.foo);
		Assert.Equal("two", value.Second.bar);
	}

}
