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
	private static ValueSpan _empty = new ValueSpan(0,0);
	internal readonly int Start;
	internal readonly int Length;
	public ValueSpan(int start, int length) {
		this.Start = start;
		this.Length = length;
	}

	public static ValueSpan Empty => _empty;

	public ReadOnlySpan<byte> GetSlice(ReadOnlySpan<byte> input) => input.Slice(this.Start, this.Length);


	internal double GetDouble(ReadOnlySpan<byte> input) {
		var value = input.Slice(Start, Length);
		return value switch {
			[(byte)'I', (byte)'N', (byte)'F'] => double.PositiveInfinity,
			[(byte)'-', (byte)'I', (byte)'N', (byte)'F'] => double.NegativeInfinity,
			[(byte)'N', (byte)'A', (byte)'N'] => double.NaN,
			_ => double.Parse(value, CultureInfo.InvariantCulture),
		};
	}

	internal bool GetBool(ReadOnlySpan<byte> input) => input[this.Start] == '1';

	internal long GetLong(ReadOnlySpan<byte> input) => long.Parse(
		input.Slice(this.Start, this.Length),
		CultureInfo.InvariantCulture
	);

	internal string GetString(ReadOnlySpan<byte> input, Encoding inputEncoding) {
		return inputEncoding.GetString(input.Slice(this.Start, this.Length));
	}
}