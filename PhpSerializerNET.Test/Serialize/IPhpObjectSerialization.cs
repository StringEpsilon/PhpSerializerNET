/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class IPhpObjectSerializationTest {
		public class MyPhpObject : IPhpObject {
			public string GetClassName() => "MyPhpObject";
			public void SetClassName(string className) {}
			public string Foo {get;set;}
		}

		[TestMethod]
		public void SerializeIPhpObject() {
			Assert.AreEqual( // strings:
				"O:11:\"MyPhpObject\":1:{s:3:\"Foo\";s:0:\"\";}",
				PhpSerialization.Serialize( new MyPhpObject() {Foo =""})
			);
		}
	}
}