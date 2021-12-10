/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Deserialize.Options {
	[TestClass]
	public class NumberStringToBoolTest {
		[TestMethod]
		public void Enabled_Deserializes() {
			var options = new PhpDeserializationOptions() { NumberStringToBool = true };

			var value = PhpSerialization.Deserialize<bool>("s:1:\"1\";", options);
			Assert.AreEqual(true, value);

			value = PhpSerialization.Deserialize<bool>("s:1:\"0\";", options);
			Assert.AreEqual(false, value);
		}

		[TestMethod]
		public void Disabled_Throws() {
			var exception = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize<bool>(
					"s:1:\"0\";",
					new PhpDeserializationOptions() { NumberStringToBool = false }
				)
			);

			Assert.AreEqual(
				"Exception encountered while trying to assign '0' to type Boolean. See inner exception for details.",
				exception.Message
			);
		}
	}
}