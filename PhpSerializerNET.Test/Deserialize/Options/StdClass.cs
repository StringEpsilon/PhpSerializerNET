/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections;
using System.Collections.Generic;
using Xunit;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Deserialize.Options {
	public class StdClassTest {
		public struct MyStruct {
			public double John;
			public double Jane;
		}

		[Fact]
		public void Option_Throw() {
			var ex = Assert.Throws<DeserializationException>(
				() => PhpSerialization.Deserialize(
					"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
					new PhpDeserializationOptions() { StdClass = StdClassOption.Throw }
				)
			);

			Assert.Equal(
				"Encountered 'stdClass' and the behavior 'Throw' was specified in deserialization options.",
				ex.Message
			);
		}

		[Fact]
		public void Option_Dynamic() {
			dynamic result = (PhpDynamicObject)PhpSerialization.Deserialize(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				new PhpDeserializationOptions() { StdClass = StdClassOption.Dynamic }
			);

			Assert.Equal(3.14, result.John);
			Assert.Equal(2.718, result.Jane);
			Assert.Equal("stdClass", result.GetClassName());
		}

		[Fact]
		public void Option_Dictionary() {
			var result = (IDictionary)PhpSerialization.Deserialize(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				new PhpDeserializationOptions() { StdClass = StdClassOption.Dictionary }
			);

			Assert.Equal(3.14, result["John"]);
			Assert.Equal(2.718, result["Jane"]);
		}

		[Fact]
		public void Overridden_By_Class() {
			var result = PhpSerialization.Deserialize<NamedClass>(
				"O:8:\"stdClass\":2:{s:3:\"Foo\";d:3.14;s:3:\"Bar\";d:2.718;}",
				new PhpDeserializationOptions() { StdClass = StdClassOption.Dynamic }
			);

			Assert.IsType<NamedClass>(result);
			Assert.Equal(3.14, result.Foo);
			Assert.Equal(2.718, result.Bar);
		}

		[Fact]
		public void Overridden_By_Struct() {
			var result = PhpSerialization.Deserialize<MyStruct>(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				new PhpDeserializationOptions() { StdClass = StdClassOption.Dynamic }
			);

			Assert.IsType<MyStruct>(result);
			Assert.Equal(3.14, result.John);
			Assert.Equal(2.718, result.Jane);
		}

		[Fact]
		public void Overridden_By_Dictionary() {
			var result = PhpSerialization.Deserialize<Dictionary<string, object>>(
				"O:8:\"stdClass\":2:{s:4:\"John\";d:3.14;s:4:\"Jane\";d:2.718;}",
				new PhpDeserializationOptions() { StdClass = StdClassOption.Dynamic }
			);

			Assert.Equal(3.14, result["John"]);
			Assert.Equal(2.718, result["Jane"]);
		}
	}
}