/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Deserialize.Options {
	[TestClass]
	public class InputEncodingTest {
		private static string Latin1TestString = Encoding.Latin1.GetString(
			Encoding.Convert(
				Encoding.Default,
				Encoding.Latin1, 
				Encoding.Default.GetBytes("s:3:\"äöü\";")
			)
		);

		[TestMethod]
		public void WrongEncodingFails() {
			Assert.ThrowsException<DeserializationException>(
				() => PhpSerialization.Deserialize(Latin1TestString)
			);
		}

		[TestMethod]
		public void CorrectEncodingWorks() {
			var result = PhpSerialization.Deserialize(
				Latin1TestString, 
				new PhpDeserializationOptions(){
					InputEncoding =Encoding.Latin1
				}
			);

			Assert.AreEqual("äöü", result);
		}
	}
}