/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Deserialize.Validation;

public class TestBoolValidation {
	[Theory]
	[InlineData("b", "Unexpected end of input. Expected ':' at index 1, but input ends at index 0")]
	[InlineData("b:1", "Unexpected end of input. Expected ';' at index 3, but input ends at index 2")]
	[InlineData("b:", "Unexpected end of input. Expected '0' or '1' at index 2, but input ends at index 1")]
	[InlineData("b:2;", "Unexpected token in boolean at index 2. Expected either '1' or '0', but found '2' instead.")]
	public void ThrowsOnMalformeBool(string input, string exceptionMessage) {
		var ex = Assert.Throws<DeserializationException>(() => PhpSerialization.Deserialize(input));
		Assert.Equal(exceptionMessage, ex.Message);
	}
}
