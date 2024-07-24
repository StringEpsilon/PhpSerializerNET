
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Xunit;

namespace PhpSerializerNET.Test.Serialize;

public class PhpDateTimeSerializationTest {
	[Fact]
	public void Serializes1() {
		var testObject = new PhpDateTime() {
			Date = "2021-12-15 19:32:38.980103",
			TimezoneType = 3,
			Timezone = "UTC",
		};
		Assert.Equal(
			"O:8:\"DateTime\":3:{s:4:\"date\";s:26:\"2021-12-15 19:32:38.980103\";s:13:\"timezone_type\";i:3;s:8:\"timezone\";s:3:\"UTC\";}",
			PhpSerialization.Serialize(testObject)
		);
	}
}
