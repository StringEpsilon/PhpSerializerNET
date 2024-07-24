/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Text;
using Xunit;

namespace PhpSerializerNET.Test.Deserialize.Options {
	public class InputEncodingTest {
		private static readonly string Latin1TestString = Encoding.Latin1.GetString(
			Encoding.Convert(
				Encoding.Default,
				Encoding.Latin1,
				Encoding.Default.GetBytes("s:3:\"äöü\";")
			)
		);

		[Fact]
		public void WrongEncodingFails() {

			var ex = Assert.Throws<DeserializationException>(
				() => PhpSerialization.Deserialize(Latin1TestString)
			);

			// The deserialization failed, because the length of "äöü" in bytes is 6 in UTF8 but 3 in Latin1,
			// which results in a misalignment and failure to find the end of the string.
			// I have cross-checked that the PHP implementation (at least in versions I tested) fails for the same reason.
			Assert.Equal("Unexpected token at index 8. Expected '\"' but found '¶' instead.", ex.Message);
		}

		[Fact]
		public void CorrectEncodingWorks() {
			var result = PhpSerialization.Deserialize(
				Latin1TestString,
				new PhpDeserializationOptions() {
					InputEncoding = Encoding.Latin1
				}
			);

			Assert.Equal("äöü", result);
		}
	}
}