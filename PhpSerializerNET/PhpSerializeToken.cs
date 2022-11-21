/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/
namespace PhpSerializerNET;

/// <summary>
/// PHP Serialization format token. Holds type, length, position (of the token in the input string) and child information.
/// </summary>
internal record struct PhpSerializeToken(
	PhpSerializerType Type,
	int Position,
	string Value,
	PhpSerializeToken[] Children
);