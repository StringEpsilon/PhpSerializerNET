/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

namespace PhpSerializerNET.Test.DataTypes {
	public partial class DeserializeObjects {
		public class CircularTest {
			public string Foo { get; set; }
			public CircularTest Bar { get; set; }
		}
	}
}
