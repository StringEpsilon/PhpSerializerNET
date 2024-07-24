/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using Xunit;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Deserialize.Validation;

public class TestOtherErrors {
	[Fact]
	public void ThrowsOnUnexpectedToken() {
		var ex = Assert.Throws<DeserializationException>(() => PhpSerialization.Deserialize("_"));
		Assert.Equal("Unexpected token '_' at position 0.", ex.Message);

		ex = Assert.Throws<DeserializationException>(() => PhpSerialization.Deserialize("i:42;_"));
		Assert.Equal("Unexpected token '_' at position 5.", ex.Message);

		ex = Assert.Throws<DeserializationException>(() => PhpSerialization.Deserialize("_i:42;"));
		Assert.Equal("Unexpected token '_' at position 0.", ex.Message);
	}

	[Fact]
	public void ErrorOnTuple() {
		var ex = Assert.Throws<DeserializationException>(
			() => PhpSerialization.Deserialize("s:7:\"AString\";s:7:\"AString\";")
		);

		Assert.Equal("Unexpected token 's' at position 14.", ex.Message);
	}

	[Fact]
	public void ErrorOnEmptyInput() {
		var ex = Assert.Throws<ArgumentOutOfRangeException>(
			() => PhpSerialization.Deserialize("")
		);

		const string expected = "PhpSerialization.Deserialize(): Parameter 'input' must not be null or empty. (Parameter 'input')";
		Assert.Equal(expected, ex.Message);

		ex = Assert.Throws<ArgumentOutOfRangeException>(
			() => PhpSerialization.Deserialize<string>("")
		);

		Assert.Equal(expected, ex.Message);

		ex = Assert.Throws<ArgumentOutOfRangeException>(
			() => PhpSerialization.Deserialize("", typeof(string))
		);

		Assert.Equal(expected, ex.Message);
	}


	[Fact]
	public void ThrowOnIllegalKeyType() {
		var ex = Assert.Throws<DeserializationException>(
			() => PhpSerialization.Deserialize<MyPhpObject>("O:8:\"stdClass\":1:{b:1;s:4:\"true\";}")
		);
		Assert.Equal(
			"Error encountered deserizalizing an object of type 'PhpSerializerNET.Test.DataTypes.MyPhpObject': " +
			"The key '1' (from the token at position 18) has an unsupported type of 'Boolean'.",
			ex.Message
		);
	}

	[Fact]
	public void ThrowOnIntegerKeyPhpObject() {
		var ex = Assert.Throws<ArgumentException>(
			() => PhpSerialization.Deserialize<PhpObjectDictionary>("O:8:\"stdClass\":1:{i:0;s:4:\"true\";}")
		);
	}
}
