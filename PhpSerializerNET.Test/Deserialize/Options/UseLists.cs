/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using Xunit;

namespace PhpSerializerNET.Test.Deserialize.Options {
	public class UseListsTest {
		[Fact]
		public void Option_Never() {
			var test = PhpSerialization.Deserialize(
				"a:2:{i:0;s:1:\"a\";i:1;s:1:\"b\";}",
				new PhpDeserializationOptions() {
					UseLists = ListOptions.Never
				}
			);

			var dictionary = test as Dictionary<object, object>;
			Assert.NotNull(dictionary);
			Assert.Equal(2, dictionary.Count);
			Assert.Equal("a", dictionary[(long)0]);
			Assert.Equal("b", dictionary[(long)1]);
		}

		[Fact]
		public void Option_Default() {
			var result = PhpSerialization.Deserialize(
				"a:2:{i:0;s:1:\"a\";i:1;s:1:\"b\";}",
				new PhpDeserializationOptions() {
					UseLists = ListOptions.Default
				}
			);

			var list = result as List<object>;
			Assert.NotNull(list);
			Assert.Equal(2, list.Count);
			Assert.Equal("a", list[0]);
			Assert.Equal("b", list[1]);
		}

		[Fact]
		public void Option_Default_NonConsequetive() {
			// Same option, non-consecutive integer keys:
			var result = PhpSerialization.Deserialize(
				"a:2:{i:2;s:1:\"a\";i:4;s:1:\"b\";}",
				new PhpDeserializationOptions() {
					UseLists = ListOptions.Default
				}
			);

			Assert.Equal(typeof (Dictionary<object, object>), result.GetType());
			var dictionary = result as Dictionary<object, object>;
			Assert.NotNull(dictionary);
			Assert.Equal(2, dictionary.Count);
			Assert.Equal("a", dictionary[(long)2]);
			Assert.Equal("b", dictionary[(long)4]);
		}

		[Fact]
		public void Option_OnAllIntegerKeys() {
			var test = PhpSerialization.Deserialize(
				"a:2:{i:0;s:1:\"a\";i:1;s:1:\"b\";}",
				new PhpDeserializationOptions() {
					UseLists = ListOptions.OnAllIntegerKeys
				}
			);

			var list = test as List<object>;
			Assert.NotNull(list);
			Assert.Equal(2, list.Count);
			Assert.Equal("a", list[0]);
			Assert.Equal("b", list[1]);

		}

		[Fact]
		public void Option_OnAllIntegerKeys_NonConsequetive() {
			// Same option, non-consecutive integer keys:
			var result = PhpSerialization.Deserialize(
				"a:2:{i:2;s:1:\"a\";i:4;s:1:\"b\";}",
				new PhpDeserializationOptions() {
					UseLists = ListOptions.OnAllIntegerKeys
				}
			);

			var list = result as List<object>;
			Assert.NotNull(list);
			Assert.Equal(2, list.Count);
			Assert.Equal("a", list[0]);
			Assert.Equal("b", list[1]);
		}
	}
}