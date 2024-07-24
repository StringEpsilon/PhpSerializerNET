/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Other;

public class PhpDynamicObjectTest {
	[Fact]
	public void CanReadAndWriteProps() {
		dynamic testObject = new PhpDynamicObject();

		testObject.foo = "Foo";
		Assert.Equal("Foo", testObject.foo);
	}

	[Fact]
	public void GetAndSetClassname() {
		dynamic testObject = new PhpDynamicObject();

		testObject.SetClassName("MyClass");
		Assert.Equal("MyClass", testObject.GetClassName());
	}
}
