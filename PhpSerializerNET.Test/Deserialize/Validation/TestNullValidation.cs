
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Deserialize.Validation;

public class TestNullValidation {
	[Theory]
	[InlineData("N", "Unexpected end of input. Expected ';' at index 1, but input ends at index 0")]
	[InlineData("N?", "Unexpected token at index 1. Expected ';' but found '?' instead.")]
	public void ThrowsOnTruncatedInput(string input, string exceptionMessage) {
		var ex = Assert.Throws<DeserializationException>(() => PhpSerialization.Deserialize(input));
		Assert.Equal(exceptionMessage, ex.Message);
	}
}
