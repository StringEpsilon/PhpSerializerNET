
/**
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;
using System.Collections.Generic;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Deserialize;

public class ObjectDeserializationTest {
	[Fact]
	public void References() {
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

	[Fact]
	public void ListOfObjects() {
		// Regression test for https://github.com/StringEpsilon/PhpSerializerNET/issues/40
		var result = PhpSerialization.Deserialize<List<MixedKeysPhpClass>>(
			"""a:3:{i:0;a:4:{s:3:"Foo";s:3:"Foo";s:3:"Bar";s:3:"Bar";s:1:"a";s:1:"A";s:1:"b";s:1:"B";}i:1;a:4:{s:3:"Foo";s:3:"Foo";s:3:"Bar";s:3:"Bar";s:1:"a";s:1:"A";s:1:"b";s:1:"B";}i:2;a:4:{s:3:"Foo";s:3:"Foo";s:3:"Bar";s:3:"Bar";s:1:"a";s:1:"A";s:1:"b";s:1:"B";}}"""
		);

		Assert.Equal(3, result.Count);
	}
	[Fact]
	public void ImplicitListOfObjects() {
		// Regression test for https://github.com/StringEpsilon/PhpSerializerNET/issues/40
		var result = PhpSerialization.Deserialize(
			"""a:3:{i:0;a:4:{s:3:"Foo";s:3:"Foo";s:3:"Bar";s:3:"Bar";s:1:"a";s:1:"A";s:1:"b";s:1:"B";}i:1;a:4:{s:3:"Foo";s:3:"Foo";s:3:"Bar";s:3:"Bar";s:1:"a";s:1:"A";s:1:"b";s:1:"B";}i:2;a:4:{s:3:"Foo";s:3:"Foo";s:3:"Bar";s:3:"Bar";s:1:"a";s:1:"A";s:1:"b";s:1:"B";}}""",
			new PhpDeserializationOptions { UseLists = ListOptions.Default }
		) as List<object>;

		Assert.Equal(3, result.Count);
	}
}
