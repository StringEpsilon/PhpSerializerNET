/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;
using PhpSerializerNET.Test.DataTypes;

namespace PhpSerializerNET.Test;

public partial class DeserializeWithRuntimeTypeTest {

	[Fact]
	public void DeserializeObjectWithRuntimeType() {
		var expectedType = typeof(NamedClass);
		var result = PhpSerialization.Deserialize("O:8:\"stdClass\":2:{s:3:\"Foo\";d:3.14;s:3:\"Bar\";d:2.718;}", expectedType);

		Assert.NotNull(result);
		Assert.IsType(expectedType, result);

		Assert.Equal(
			3.14,
			((NamedClass)result).Foo
		);
		Assert.Equal(
			2.718,
			((NamedClass)result).Bar
		);
	}

	[Fact]
	public void DeserializeArrayWithRuntimeType() {
		var expectedType = typeof(NamedClass);
		var result = PhpSerialization.Deserialize("a:2:{s:3:\"Foo\";d:3.14;s:3:\"Bar\";d:2.718;}", expectedType);

		Assert.NotNull(result);
		Assert.IsType(expectedType, result);

		Assert.Equal(
			3.14,
			((NamedClass)result).Foo
		);
		Assert.Equal(
			2.718,
			((NamedClass)result).Bar
		);
	}
}
