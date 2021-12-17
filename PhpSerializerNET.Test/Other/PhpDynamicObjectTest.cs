/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Other {
	[TestClass]
	public class PhpDynamicObjectTest {
		[TestMethod]
		public void CanReadAndWriteProps() {
			dynamic testObject = new PhpDynamicObject();

			testObject.foo = "Foo";
			Assert.AreEqual("Foo", testObject.foo);
		}

		[TestMethod]
		public void GetAndSetClassname() {
			dynamic testObject = new PhpDynamicObject();

			testObject.SetClassName("MyClass");
			Assert.AreEqual("MyClass", testObject.GetClassName());
		}
	}
}
