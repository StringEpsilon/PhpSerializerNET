/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Serialize {
	/// <summary>
	///
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class PhpIgnoreNull : PhpSerializationFilter {
		public override string Serialize(object key, object value, PhpSerializiationOptions options) {
			if (value != null) {
				return PhpSerialization.Serialize(key, options) + PhpSerialization.Serialize(value, options);
			}
			return null;
		}
	}

	public class IgnoreTestClass {
		public string Foo { get; set; }
		[PhpIgnoreNull]
		public string Bar { get; set; }
	}

	[TestClass]
	public class PhpSerializationFilterTest {
		[TestMethod]
		public void IgnoreNullIgnoresNull() {

			Assert.AreEqual(
				"a:1:{s:3:\"Foo\";N;}",
				PhpSerialization.Serialize(
					new IgnoreTestClass() { }
				)
			);

			Assert.AreEqual(
				"a:2:{s:3:\"Foo\";N;s:3:\"Bar\";s:3:\"bar\";}",
				PhpSerialization.Serialize(
					new IgnoreTestClass() { Bar = "bar" }
				)
			);
		}
	}
}