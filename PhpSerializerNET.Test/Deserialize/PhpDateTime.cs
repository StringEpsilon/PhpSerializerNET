/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class TestPhpDateTime {
		[TestMethod]
		public void DeserializesCorrectly() {
			var result = PhpSerialization.Deserialize(
				"O:8:\"DateTime\":3:{s:4:\"date\";s:26:\"2021-08-18 09:10:23.441055\";s:13:\"timezone_type\";i:3;s:8:\"timezone\";s:3:\"UTC\";}"
			);

			Assert.IsInstanceOfType(result, typeof(PhpDateTime));
			var date = result as PhpDateTime;
			Assert.AreEqual("UTC", date.Timezone);
			Assert.AreEqual("2021-08-18 09:10:23.441055", date.Date);
		}
	}
}
