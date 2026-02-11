/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using Xunit;

namespace PhpSerializerNET.Test.Deserialize.Validation;

public class TestArrayValidation {
	[Theory]
	[InlineData("a", "Unexpected end of input. Expected ':' at index 1, but input ends at index 0")]
	[InlineData("a:-1:{};", "Array at position 2 has illegal, missing or malformed length.")]
	[InlineData("a:100:};", "Unexpected token at index 6. Expected '{' but found '}' instead.")]
	[InlineData("a:10000   ", "Array at position 7 has illegal, missing or malformed length.")]
	[InlineData("a:10000", "Unexpected end of input. Expected ':' at index 7, but input ends at index 6")]
	[InlineData("a:10000:", "Unexpected end of input. Expected '{' at index 8, but input ends at index 7")]
	[InlineData("a:2:{i:0;i:0;i:1;i:1;i:2;i:2;}", "Array at position 0 should be of length 2, but actual length is 3 or more.")]
	public void ThrowsOnMalformedArray(string input, string exceptionMessage) {
		var ex = Assert.Throws<DeserializationException>(() => PhpSerialization.Deserialize(input));
		Assert.Equal(exceptionMessage, ex.Message);
	}

	[Fact]
	public void AllowsValidArray() {
		var result = PhpSerialization.Deserialize("a:24:{i:0;i:2147483647;i:1;i:2147483647;i:2;i:2147483647;i:3;i:2147483647;i:4;i:2147483647;i:5;i:2147483647;i:6;i:2147483647;i:7;i:2147483647;i:8;i:2147483647;i:9;i:2147483647;i:10;i:2147483647;i:11;i:2147483647;i:12;i:2147483647;i:13;i:2147483647;i:14;i:2147483647;i:15;i:2147483647;i:16;i:2147483647;i:17;i:2147483647;i:18;i:2147483647;i:19;i:2147483647;i:20;i:2147483647;i:21;i:2147483647;i:22;i:2147483647;i:23;i:2147483647;}");
		if (result is List<object>) {
			Assert.Equal(24, ((List<object>)result).Count);
		} else {
			Assert.Fail("Expected List<object> but got " + result.GetType().Name);
		}
	}
}
