/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Deserialize.Validation;

public class TestArrayValidation {
	[Theory]
	[InlineData("a", "Unexpected end of input. Expected ':' at index 1, but input ends at index 0")]
	[InlineData("a:-1:{};", "Array at position 2 has illegal, missing or malformed length.")]
	[InlineData("a:100:};", "Unexpected token at index 6. Expected '{' but found '}' instead.")]
	[InlineData("a:10000   ", "Array at position 7 has illegal, missing or malformed length.")]
	[InlineData("a:10000:", "Unexpected end of input. Expected '{' at index 8, but input ends at index 7")]
	[InlineData("a:1000000", "Unexpected token at index 8. Expected ':' but found '0' instead.")]
	[InlineData("a:2:{i:0;i:0;i:1;i:1;i:2;i:2;}", "Array at position 0 should be of length 2, but actual length is 3 or more.")]
	public void ThrowsOnMalformedArray(string input, string exceptionMessage) {
		var ex = Assert.Throws<DeserializationException>(() => PhpSerialization.Deserialize(input));
		Assert.Equal(exceptionMessage, ex.Message);
	}
}
