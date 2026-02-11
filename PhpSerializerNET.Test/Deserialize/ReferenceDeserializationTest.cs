
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

	[Fact]
	public void ReferenceWithNullValuesBefore() {
		var value = PhpSerialization.Deserialize<ReferenceWithNulls>(
			"""a:2:{s:5:"first";a:2:{s:9:"modifiers";N;s:5:"inner";O:5:"Inner":1:{s:5:"value";s:4:"test";}}s:6:"second";a:2:{s:9:"modifiers";N;s:5:"inner";r:4;}}"""
		);

		Assert.Null(value.first.modifiers);
		Assert.Null(value.second.modifiers);
		Assert.Equal("test", value.first.inner.value);
		Assert.Equal("test", value.second.inner.value);
	}

	[Fact]
	public void MultipleValueReferencesBeforeTarget() {
		var value = PhpSerialization.Deserialize<MultiRef>(
			"""a:5:{s:5:"first";O:3:"Obj":1:{s:2:"id";s:1:"A";}s:6:"second";r:2;s:5:"third";r:2;s:6:"fourth";O:3:"Obj":1:{s:2:"id";s:1:"B";}s:5:"fifth";r:6;}"""
		);

		Assert.Equal("A", value.first.id);
		Assert.Equal("A", value.second.id);
		Assert.Equal("A", value.third.id);
		Assert.Equal("B", value.fourth.id);
		Assert.Equal("B", value.fifth.id);
	}
}
