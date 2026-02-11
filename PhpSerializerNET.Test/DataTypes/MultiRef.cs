/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

namespace PhpSerializerNET.Test.DataTypes;

public class MultiRef {
	public MultiRefObj first { get; set; }
	public MultiRefObj second { get; set; }
	public MultiRefObj third { get; set; }
	public MultiRefObj fourth { get; set; }
	public MultiRefObj fifth { get; set; }
}

public class MultiRefObj {
	public string id { get; set; }
}
