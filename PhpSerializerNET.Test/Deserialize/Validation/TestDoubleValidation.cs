/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Deserialize.Validation;
public class TestDoubleValidation {
	[Theory]
	[InlineData("d", "Unexpected end of input. Expected ':' at index 1, but input ends at index 0")]
	[InlineData("b     ", "Unexpected token at index 1. Expected ':' but found ' ' instead.")]
	[InlineData("d:111111", "Unexpected end of input. Expected ':' at index 7, but input ends at index 7")]
	[InlineData("d:bgg5;", "Unexpected token at index 2. 'b' is not a valid part of a floating point number.")]
	[InlineData("d:;", "Unexpected token at index 2: Expected floating point number, but found ';' instead.")]
	public void ThrowsOnMalformedDouble(string input, string exceptionMessage) {
		var ex = Assert.Throws<DeserializationException>(() => PhpSerialization.Deserialize(input));
		Assert.Equal(exceptionMessage, ex.Message);
	}
}
