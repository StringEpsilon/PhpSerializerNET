/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/
namespace PhpSerializerNET;

using System;
using System.Globalization;
using System.Text;

internal readonly struct ValueSpan {
	internal static ValueSpan Empty => new(0, 0);
	internal readonly int Start;
	internal readonly int Length;

	internal ValueSpan(int start, int length) {
		this.Start = start;
		this.Length = length;
	}

	internal ReadOnlySpan<byte> GetSlice(in ReadOnlySpan<byte> input) => input.Slice(this.Start, this.Length);

	internal double GetDouble(in ReadOnlySpan<byte> input) {
		var value = input.Slice(this.Start, this.Length);
		return value switch {
		[(byte)'I', (byte)'N', (byte)'F'] => double.PositiveInfinity,
		[(byte)'-', (byte)'I', (byte)'N', (byte)'F'] => double.NegativeInfinity,
		[(byte)'N', (byte)'A', (byte)'N'] => double.NaN,
			_ => double.Parse(value, CultureInfo.InvariantCulture),
		};
	}

	internal bool GetBool(in ReadOnlySpan<byte> input) => input[this.Start] == '1';

	internal int GetInt(in ReadOnlySpan<byte> input) {
		// All the PHP integers we deal with here can only be the number characters and an optional "-".
		// See also the Validator code.
		// 'int.Parse()' has to make considerations that we can skip here, making this manual approach faster.
		var span = input.Slice(this.Start, this.Length);
		if (span[0] == (byte)'-') {
			int result = span[1] - 48;
			for (int i = 2; i < span.Length; i++) {
				result = result * 10 + (span[i] - 48);
			}
			return result*-1;
		} else if (span[0] == (byte)'+') {
			int result = span[1] - 48;
			for (int i = 2; i < span.Length; i++) {
				result = result * 10 + (span[i] - 48);
			}
			return result;
		} else {
			int result = span[0] - 48;
			for (int i = 1; i < span.Length; i++) {
				result = result * 10 + (span[i] - 48);
			}
			return result;
		}
	}

	internal string GetString(in ReadOnlySpan<byte> input, Encoding inputEncoding) {
		return inputEncoding.GetString(input.Slice(this.Start, this.Length));
	}
}