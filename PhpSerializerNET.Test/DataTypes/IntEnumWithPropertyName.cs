/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

namespace PhpSerializerNET.Test.DataTypes;

public enum IntEnumWithPropertyName {

	[PhpProperty("a")]
	A = 1,

	[PhpProperty("c")]
	B = 13,

	// No prop name
	C,
}