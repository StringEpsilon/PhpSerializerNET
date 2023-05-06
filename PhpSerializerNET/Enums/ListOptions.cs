/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

namespace PhpSerializerNET;

/// <summary>
/// Available behaviors for dealing with associative arrays and their conversion to Lists.
/// </summary>
public enum ListOptions {
	/// <summary>
	/// Convert associative array to list when all keys are consecutive integers
	/// </summary>
	Default,

	/// <summary>
	/// Convert associative array to list when all keys are integers, consecutive or not.
	/// </summary>
	OnAllIntegerKeys,

	/// <summary>
	/// Always use dictionaries.
	/// </summary>
	Never
}
