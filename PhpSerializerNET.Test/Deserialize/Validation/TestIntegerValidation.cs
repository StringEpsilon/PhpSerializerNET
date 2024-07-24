/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/
using Xunit;

namespace PhpSerializerNET.Test.Deserialize.Validation;

public class TestIntegerValidation {
	[Theory]
	[InlineData("i:;", "Unexpected token at index 2: Expected number, but found ';' instead.")]
	[InlineData("i:INF;", "Unexpected token at index 2. 'I' is not a valid part of a number.")]
	[InlineData("i:NaN;", "Unexpected token at index 2. 'N' is not a valid part of a number.")]
	[InlineData("i:12345b:;", "Unexpected token at index 7. 'b' is not a valid part of a number.")]
	[InlineData("i:12345.;", "Unexpected token at index 7. '.' is not a valid part of a number.")]
	public void ThrowsOnMalformedInteger(string input, string exceptionMessage) {
		var exception = Assert.Throws<DeserializationException>(() => {
			PhpSerialization.Deserialize(input);
		});
		Assert.Equal(
			exceptionMessage,
			exception.Message
		);
	}

}
