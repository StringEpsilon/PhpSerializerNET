/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Deserialize;

public class PhpDateTimeDeserializationTest {
	[Fact]
	public void DeserializesCorrectly() {
		var result = PhpSerialization.Deserialize(
			"O:8:\"DateTime\":3:{s:4:\"date\";s:26:\"2021-08-18 09:10:23.441055\";s:13:\"timezone_type\";i:3;s:8:\"timezone\";s:3:\"UTC\";}"
		);

		Assert.IsType<PhpDateTime>(result);
		var date = result as PhpDateTime;
		Assert.Equal("UTC", date.Timezone);
		Assert.Equal("2021-08-18 09:10:23.441055", date.Date);
		Assert.Equal("DateTime", date.GetClassName());
	}
}
