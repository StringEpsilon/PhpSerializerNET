/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/
namespace PhpSerializerNET;

#nullable enable

/// <summary>
/// PHP data token. Holds the type, position (in the input string), length and value.
/// </summary>
internal readonly struct PhpToken {
	internal readonly PhpDataType Type;
	internal readonly int Position;
	internal readonly int Length;
	internal readonly string? Value;
	internal PhpToken(PhpDataType type, int position, string? value = null, int length = 0) {
		this.Type = type;
		this.Position = position;
		this.Value = value;
		this.Length = length;
	}
}