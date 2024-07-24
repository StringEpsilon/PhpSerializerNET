/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Deserialize.Validation;

public class TestStringValidation {
	[Theory]
	[InlineData("s", "Unexpected end of input. Expected ':' at index 1, but input ends at index 0")]
	[InlineData("s:3:abc\";", "Unexpected token at index 4. Expected '\"' but found 'a' instead.")]
	[InlineData("s:3:\"abc;", "Unexpected token at index 8. Expected '\"' but found ';' instead.")]
	[InlineData("s:3\"abc\";", "String at position 3 has illegal, missing or malformed length.")]
	[InlineData("s:_:\"abc\";", "String at position 2 has illegal, missing or malformed length.")]
	[InlineData("s:10:\"abc\";", "Illegal length of 10. The string at position 6 points to out of bounds index 16.")]
	[InlineData("s:3:\"abc\"", "Unexpected end of input. Expected ';' at index 9, but input ends at index 8")]
	public void ThrowsOnMalformedString(string input, string exceptionMessage) {
		var ex = Assert.Throws<DeserializationException>(() => PhpSerialization.Deserialize(input));
		Assert.Equal(exceptionMessage, ex.Message);
	}
}