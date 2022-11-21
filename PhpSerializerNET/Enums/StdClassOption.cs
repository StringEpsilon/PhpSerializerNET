/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

namespace PhpSerializerNET;

public enum StdClassOption {
	/// <summary>
	/// Deserialize all 'stdClass' objects into <see cref="PhpObjectDictionary"/> (extending <see cref="System.Collections.Generic.Dictionary{string,object}"/>)
	/// </summary>
	Dictionary,

	/// <summary>
	/// Deserialize all 'stdClass' objects dynamic objects (<see cref="PhpDynamicObject"/>)
	/// </summary>
	Dynamic,

	/// <summary>
	/// Throw an exception and abort deserialization when encountering stdClass objects.
	/// </summary>
	Throw,
}
