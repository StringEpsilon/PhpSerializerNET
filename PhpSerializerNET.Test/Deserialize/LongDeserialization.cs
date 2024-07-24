
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Deserialize;

public class LongDeserializationTest {
	[Theory]
	[InlineData("N;", null)]
	[InlineData("i:1;", 1L)]
	public void SupportsNull<T>(string input, T expected) {
		var result = PhpSerialization.Deserialize<T>(input);
		Assert.Equal(expected, result);
	}

	[Theory]
	[InlineData("i:-32768;", short.MinValue)]
	[InlineData("i:32767;", short.MaxValue)]
	[InlineData("i:0;", ushort.MinValue)]
	[InlineData("i:65535;", ushort.MaxValue)]
	[InlineData("i:0;", uint.MinValue)]
	[InlineData("i:4294967295;", uint.MaxValue)]
	[InlineData("N;", 0L)]
	[InlineData("i:0;", 0L)]
	[InlineData("i:1;", 1L)]
	[InlineData("i:-9223372036854775808;", long.MinValue)]
	[InlineData("i:9223372036854775807;", long.MaxValue)]
	[InlineData("i:0;", ulong.MinValue)]
	[InlineData("i:18446744073709551615;", ulong.MaxValue)]
	[InlineData("i:-128;", sbyte.MinValue)]
	[InlineData("i:127;", sbyte.MaxValue)]
	[InlineData("i:255;", byte.MaxValue)]
	public void SupportsOtherNumberTypes<T>(string input, T expected) {
		var result = PhpSerialization.Deserialize<T>(input);
		Assert.IsType<T>(result);
		Assert.Equal(expected, result);
	}
}