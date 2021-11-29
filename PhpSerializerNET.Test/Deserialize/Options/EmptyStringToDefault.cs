/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test {
	[TestClass]
	public class EmptyStringToDefaultTest {
		[TestMethod]
		public void Enabled_EmptyStringToInt() {
			var result = PhpSerialization.Deserialize<int>("s:0:\"\";");
			Assert.AreEqual(0, result);
		}

		[TestMethod]
		public void Enabled_StringToBool() {
			var result = PhpSerialization.Deserialize<bool>("s:0:\"\";");
			Assert.AreEqual(false, result);
		}

		[TestMethod]
		public void Enabled_StringToDouble() {
			var result = PhpSerialization.Deserialize<double>("s:0:\"\";");
			Assert.AreEqual(0, result);
		}

		[TestMethod]
		public void Disabled_EmptyStringToInt() {
			var exception = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize<int>("s:0:\"\";", new PhpDeserializationOptions(){EmptyStringToDefault = false})
			);
			
			Assert.AreEqual(
				"Exception encountered while trying to assign '' to type Int32. See inner exception for details.",
				exception.Message
			);
		}

		[TestMethod]
		public void Disabled_StringToBool() {
			var exception = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize<bool>("s:0:\"\";", new PhpDeserializationOptions(){EmptyStringToDefault = false})
			);
			
			Assert.AreEqual(
				"Exception encountered while trying to assign '' to type Boolean. See inner exception for details.",
				exception.Message
			);
		}

		[TestMethod]
		public void Disabled_StringToDouble() {
			var exception = Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize<double>("s:0:\"\";", new PhpDeserializationOptions(){EmptyStringToDefault = false})
			);
			
			Assert.AreEqual(
				"Exception encountered while trying to assign '' to type Double. See inner exception for details.",
				exception.Message
			);
		}
	}
}