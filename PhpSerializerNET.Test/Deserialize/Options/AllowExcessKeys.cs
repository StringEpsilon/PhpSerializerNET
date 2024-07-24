/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test.Deserialize.Options {
	public class AllowExcessKeysTest {
		const string StructTestInput = "a:3:{s:3:\"foo\";s:3:\"Foo\";s:3:\"bar\";s:3:\"Bar\";s:6:\"foobar\";s:6:\"FooBar\";}";
		const string ObjectTestInput = "a:2:{s:7:\"AString\";s:3:\"foo\";s:7:\"BString\";s:3:\"bar\";}";

		[Fact]
		public void Struct_DeserializesWithOptionEnabled() {
			var value = PhpSerialization.Deserialize<AStruct>(
				StructTestInput,
				new PhpDeserializationOptions() { AllowExcessKeys = true }
			);

			Assert.Equal(
				"Foo",
				value.foo
			);
			Assert.Equal(
				"Bar",
				value.bar
			);
		}

		[Fact]
		public void Struct_ThrowsWithOptionDisabled() {
			var ex = Assert.Throws<DeserializationException>(() => PhpSerialization.Deserialize<AStruct>(
				StructTestInput,
				new PhpDeserializationOptions() { AllowExcessKeys = false }
			));
			Assert.Equal("Could not bind the key \"foobar\" to struct of type AStruct: No such field.", ex.Message);
		}

		[Fact]
		public void Object_DeserializesWithOptionEnabled() {
			var deserializedObject = PhpSerialization.Deserialize<SimpleClass>(
				ObjectTestInput,
				new PhpDeserializationOptions() { AllowExcessKeys = true }
			);
			Assert.NotNull(deserializedObject);
		}

		[Fact]
		public void Object_ThrowsWithOptionDisabled() {
			var ex = Assert.Throws<DeserializationException>(() => PhpSerialization.Deserialize<SimpleClass>(
				ObjectTestInput,
				new PhpDeserializationOptions() { AllowExcessKeys = false }
			));
			Assert.Equal("Could not bind the key \"BString\" to object of type SimpleClass: No such property.", ex.Message);
		}

		[Fact]
		public void Enabled_ProperlyAssignsAllKeys() {
			// Explicit test for issue #27.
			var result = PhpSerialization.Deserialize<SimpleClass>(
				"O:11:\"SimpleClass\":3:{s:1:\"_\";b:0;s:4:\"True\";b:1;s:5:\"False\";b:0;}",
				new PhpDeserializationOptions() { AllowExcessKeys = true }
			);
			Assert.True(result.True);
			Assert.False(result.False);
		}
	}
}