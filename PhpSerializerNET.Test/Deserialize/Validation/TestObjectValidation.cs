/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Deserialize.Validation;

public class TestObjectValidation {
	[Theory]
	[InlineData(
		"O:-1:\"stdClass\":1:{s:3:\"Foo\";N;}",
		"Object at position 2 has illegal, missing or malformed length."
	)]
	[InlineData(
		"O:200:\"stdClass\":1:{s:3:\"Foo\";N;}",
		"Illegal length of 200. The string at position 7 points to out of bounds index 207."
	)]
	[InlineData(
		"O:8:\"stdClass:1:{s:3:\"Foo\";N;}",
		"Unexpected token at index 13. Expected '\"' but found ':' instead."
	)]
	[InlineData(
		"O:2:stdClass\":1:{s:3:\"Foo\";N;}",
		"Unexpected token at index 4. Expected '\"' but found 's' instead."
	)]
	[InlineData(
		"O:1:\"a\":2:{i:0;i:0;i:1;i:1;i:2;i:2;}",
		"Object at position 0 should have 2 properties, but actually has 3 or more properties."
	)]
	[InlineData(
		"O:8:\"stdClass\"1:{s:3:\"Foo\";N;}",
		"Unexpected token at index 14. Expected ':' but found '1' instead."
	)]
	[InlineData(
		"O:8:\"stdClass\":1{s:3:\"Foo\";N;}",
		"Object at position 16 has illegal, missing or malformed length."
	)]
	[InlineData(
		"O:8:\"stdClass\":1:s:3:\"Foo\";N;}",
		"Unexpected token at index 17. Expected '{' but found 's' instead."
	)]
	public void ThrowsOnMalformedObject(string input, string exceptionMessage) {
		var ex = Assert.Throws<DeserializationException>(() => PhpSerialization.Deserialize(input));
		Assert.Equal(exceptionMessage, ex.Message);
	}
}