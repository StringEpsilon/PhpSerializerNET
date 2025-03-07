/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Deserialize.Validation;

public class TestReferenceValidation {
	[Theory]
	[InlineData("r", "Unexpected end of input. Expected ':' at index 1, but input ends at index 0")]
	[InlineData("r:1", "Unexpected token at index 2: Expected number, but found ';' instead.")]
	[InlineData("r:1;", "Invalid reference: '1' can not be resolved.")]
	public void ThrowsOnInvalidReference(string input, string exceptionMessage) {
		var ex = Assert.Throws<DeserializationException>(() => PhpSerialization.Deserialize(input));
		Assert.Equal(exceptionMessage, ex.Message);
	}
}