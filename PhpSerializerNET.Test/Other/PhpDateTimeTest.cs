/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Other {
	[TestClass]
	public class PhpDateTimeTest {
		[TestMethod]
		public void ThrowsOnSetClassName() {
			var testObject = new PhpDateTime();

			var ex = Assert.ThrowsException<InvalidOperationException>(() => {
				testObject.SetClassName("stdClass");
			});
			Assert.AreEqual("Cannot set name on object of type PhpDateTime name is of constant DateTime", ex.Message);
		}

	}
}
