/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

namespace PhpSerializerNET;

/// <summary>
/// PHP data types that can be de/serialized.
/// </summary>
internal enum PhpDataType : byte {
	/// <summary>
	/// Null (N;)
	/// </summary>
	Null,
	/// <summary>
	/// Boolean value (b:n;)
	/// </summary>
	Boolean,
	/// <summary>
	/// Integer value (i:[value];)
	/// </summary>
	Integer,
	/// <summary>
	/// Floating point number (f:[value];)
	/// </summary>
	Floating,
	/// <summary>
	/// String (s:[length]:"[value]")
	/// </summary>
	String,
	/// <summary>
	/// Array (a:[length]:{[children]})
	/// </summary>
	Array,
	/// <summary>
	/// Object (O:[identLength]:"[ident]":[length]:{[children]})
	/// </summary>
	Object
}

