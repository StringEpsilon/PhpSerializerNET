
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using Xunit;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Deserialize;

public class StringDeserializationTest {
	[Theory]
	[InlineData("s:12:\"Hello World!\";", "Hello World!")]
	[InlineData("s:0:\"\";", "")]
	[InlineData("s:14:\"äöüßÄÖÜ\";", "äöüßÄÖÜ")]
	[InlineData("s:4:\"👻\";", "👻")]
	[InlineData("s:9:\"_\";s:1:\"_\";", "_\";s:1:\"_")] // // This is really how the PHP implementation behaves.
	public void DeserializesCorrectly(string input, string expected) {
		Assert.Equal(
			expected, PhpSerialization.Deserialize(input)
		);
	}

	[Theory]
	[InlineData("Hello World!", "s:12:\"Hello World!\";")]
	[InlineData("", "s:0:\"\";")]
	[InlineData("äöüßÄÖÜ", "s:14:\"äöüßÄÖÜ\";")]
	[InlineData("👻", "s:4:\"👻\";")]
	[InlineData("_\";s:1:\"_", "s:9:\"_\";s:1:\"_\";")]
	public void SerializesCorrectly(string input, string expected) {
		Assert.Equal(
			expected, PhpSerialization.Serialize(input)
		);
	}

	[Fact]
	public void DeserializeEmptyStringExplicit() {
		Assert.Equal(
			"",
			PhpSerialization.Deserialize<string>("s:0:\"\";", new PhpDeserializationOptions {
				EmptyStringToDefault = false
			})
		);
		Assert.Null(
			PhpSerialization.Deserialize<string>("s:0:\"\";", new PhpDeserializationOptions {
				EmptyStringToDefault = true
			})
		);
	}

	[Fact]
	public void ExplicitToGuid() {
		Guid guid = PhpSerialization.Deserialize<Guid>("s:36:\"82e2ebf0-43e6-4c10-82cf-57d60383a6be\";");
		Assert.Equal("82e2ebf0-43e6-4c10-82cf-57d60383a6be", guid.ToString());
	}

	[Fact]
	public void DeserializesStringToGuidProperty() {
		var result = PhpSerialization.Deserialize<MappedClass>(
			"a:1:{s:4:\"Guid\";s:36:\"82e2ebf0-43e6-4c10-82cf-57d60383a6be\";}"
		);
		Assert.Equal(
			new Guid("82e2ebf0-43e6-4c10-82cf-57d60383a6be"),
			result.Guid
		);
	}
}