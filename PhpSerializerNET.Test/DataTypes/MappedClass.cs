/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;

namespace PhpSerializerNET.Test.DataTypes {

	public class MappedClass {
		[PhpProperty("en")]
		public string English { get; set; }

		[PhpProperty("de")]
		public string German { get; set; }

		[PhpIgnore]
		public string It { get; set; }

		public Guid Guid { get; set; }
	}
}
