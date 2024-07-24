/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Collections.Generic;
using Xunit;

namespace PhpSerializerNET.Test.Serialize;

public class CircularReferencesTest {
	private class CircularClass {
		public string Foo { get; set; }
		public CircularClass Bar { get; set; }
	}

	[Fact]
	public void SerializeCircularObject() {
		var testObject = new CircularClass() {
			Foo = "First"
		};
		testObject.Bar = new CircularClass() {
			Foo = "Second",
			Bar = testObject
		};

		Assert.Equal(
			"a:2:{s:3:\"Foo\";s:5:\"First\";s:3:\"Bar\";a:2:{s:3:\"Foo\";s:6:\"Second\";s:3:\"Bar\";N;}}",
			PhpSerialization.Serialize(testObject)
		);
	}

	[Fact]
	public void ThrowOnCircularReferencesOption() {
		var testObject = new CircularClass() {
			Foo = "First"
		};
		testObject.Bar = new CircularClass() {
			Foo = "Second",
			Bar = testObject
		};

		var ex = Assert.Throws<ArgumentException>(
			() => PhpSerialization.Serialize(testObject, new PhpSerializiationOptions() { ThrowOnCircularReferences = true })
		);
		Assert.Equal(
			"Input object has a circular reference.",
			ex.Message
		);
	}

	[Fact]
	public void SerializeCircularList() {
		List<object> listA = new() { "A", "B" };
		List<object> listB = new() { "C", "D", listA };
		listA.Add(listB);

		Assert.Equal( // strings:
			"a:3:{i:0;s:1:\"A\";i:1;s:1:\"B\";i:2;a:3:{i:0;s:1:\"C\";i:1;s:1:\"D\";i:2;N;}}",
			PhpSerialization.Serialize(listA)
		);
	}
}
