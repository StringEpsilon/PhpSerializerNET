/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/


namespace PhpSerializerNET.Test.DataTypes {
	public class MixedKeysObject {
		[PhpProperty(0)]
		public string Foo { get; set; }
		[PhpProperty(1)]
		public string Bar { get; set; }
		[PhpProperty("a")]
		public string Baz { get; set; }
		[PhpProperty("b")]
		public string Dummy { get; set; }
	}

	[PhpClass]
	public class MixedKeysPhpClass {
		[PhpProperty(0)]
		public string Foo { get; set; }
		[PhpProperty(1)]
		public string Bar { get; set; }
		[PhpProperty("a")]
		public string Baz { get; set; }
		[PhpProperty("b")]
		public string Dummy { get; set; }
	}
}