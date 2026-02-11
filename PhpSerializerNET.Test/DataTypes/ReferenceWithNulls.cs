/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

namespace PhpSerializerNET.Test.DataTypes;

public class ReferenceWithNulls {
	public ReferenceWithNullsItem first { get; set; }
	public ReferenceWithNullsItem second { get; set; }
}

public class ReferenceWithNullsItem {
	public object modifiers { get; set; }
	public ReferenceWithNullsInner inner { get; set; }
}

public class ReferenceWithNullsInner {
	public string value { get; set; }
}
