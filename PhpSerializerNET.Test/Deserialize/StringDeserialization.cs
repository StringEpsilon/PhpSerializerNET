
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Deserialize {
	[TestClass]
	public class StringDeserializationTest {
		[TestMethod]
		public void SerializeHelloWorld() {
			Assert.AreEqual(
				"s:12:\"Hello World!\";",
				PhpSerialization.Serialize("Hello World!")
			);
		}
		
		[TestMethod]
		public void DeserializeEmptyStringExplicit() {
			Assert.AreEqual(
				"",
				PhpSerialization.Deserialize<string>("s:0:\"\";")
			);
		}

		[TestMethod]
		public void DeserializeHelloWorld() {
			Assert.AreEqual(
				"Hello World!",
				PhpSerialization.Deserialize("s:12:\"Hello World!\";")
			);
		}

		[TestMethod]
		public void DeserializeEmptyString() {
			Assert.AreEqual(
				"",
				PhpSerialization.Deserialize("s:0:\"\";")
			);
		}

		[TestMethod]
		public void DeserializeUmlauts() {
			Assert.AreEqual(
				"äöüßÄÖÜ",
				PhpSerialization.Deserialize("s:14:\"äöüßÄÖÜ\";")
			);
		}

		[TestMethod]
		public void DeserializeEmoji() {
			Assert.AreEqual(
				"👻",
				PhpSerialization.Deserialize("s:4:\"👻\";")
			);
		}

		[TestMethod]
		public void DeserializeHalfnesting() {
			// This is really how the PHP implementation behaves.
			Assert.AreEqual(
				"_\";s:1:\"_",
				PhpSerialization.Deserialize("s:9:\"_\";s:1:\"_\";")
			);
		}

		[TestMethod]
		public void ExplicitToGuid() {
			Guid guid = PhpSerialization.Deserialize<Guid>("s:36:\"82e2ebf0-43e6-4c10-82cf-57d60383a6be\";");
			Assert.AreEqual("82e2ebf0-43e6-4c10-82cf-57d60383a6be", guid.ToString());
		}
	}
}