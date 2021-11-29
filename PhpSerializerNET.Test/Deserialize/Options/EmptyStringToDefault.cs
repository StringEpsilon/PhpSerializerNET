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
			var ex = Assert.ThrowsException<System.FormatException>(
				() => PhpSerialization.Deserialize<int>("s:0:\"\";", new PhpDeserializationOptions(){EmptyStringToDefault = false})
			);
			// TODO: Rethrow the exception in the code with a useful error message, then test that here.
		}

		[TestMethod]
		public void Disabled_StringToBool() {
			var ex = Assert.ThrowsException<System.FormatException>(
				() => PhpSerialization.Deserialize<bool>("s:0:\"\";", new PhpDeserializationOptions(){EmptyStringToDefault = false})
			);
			// TODO: Rethrow the exception in the code with a useful error message, then test that here.
		}

		[TestMethod]
		public void Disabled_StringToDouble() {
			var ex = Assert.ThrowsException<System.FormatException>(
				() => PhpSerialization.Deserialize<double>("s:0:\"\";", new PhpDeserializationOptions(){EmptyStringToDefault = false})
			);
			// TODO: Rethrow the exception in the code with a useful error message, then test that here.
		}
	}
}