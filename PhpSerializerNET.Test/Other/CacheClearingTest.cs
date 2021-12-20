/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhpSerializerNET.Test.Other {
	[TestClass]
	public class CacheClearingTest {
		private const string testInput = "O:11:\"MappedClass\":2:{s:2:\"en\";s:12:\"Hello World!\";s:2:\"de\";s:11:\"Hallo Welt!\";}";

		[TestMethod]
		public void ClearsCacheProperly() {
			PhpSerialization.Deserialize(testInput); // warmup.
			PhpSerialization.ClearTypeCache();

			// Measure the fist call, with the lookup:
			var uncachedCall1 = new System.Diagnostics.Stopwatch();
			uncachedCall1.Start();
			PhpSerialization.Deserialize(testInput);
			uncachedCall1.Stop();

			// Second call should then have the cached type info:
			var cachedCall = new System.Diagnostics.Stopwatch();
			cachedCall.Start();
			PhpSerialization.Deserialize(testInput);
			cachedCall.Stop();

			Assert.IsTrue(cachedCall.ElapsedTicks * 125 < uncachedCall1.ElapsedTicks);

			// Clear the cache and measure a third call.
			PhpSerialization.ClearTypeCache();
			var uncalledCall2 = new System.Diagnostics.Stopwatch();
			uncalledCall2.Start();
			PhpSerialization.Deserialize(testInput);
			uncalledCall2.Stop();

			// Yes, the difference between cached and uncached really is that big. Bigger, in fact.
			Assert.IsTrue(cachedCall.ElapsedTicks * 125 < uncalledCall2.ElapsedTicks);
		}

		[TestMethod]
		public void NoCacheClearControlTest() {
			PhpSerialization.Deserialize(testInput); // warmup.

			// Measure the fist call, with the lookup:
			var uncachedCall1 = new System.Diagnostics.Stopwatch();
			uncachedCall1.Start();
			PhpSerialization.Deserialize(testInput);
			uncachedCall1.Stop();

			var cachedCall = new System.Diagnostics.Stopwatch();
			cachedCall.Start();
			PhpSerialization.Deserialize(testInput);
			cachedCall.Stop();

			var uncalledCall2 = new System.Diagnostics.Stopwatch();
			uncalledCall2.Start();
			PhpSerialization.Deserialize(testInput);
			uncalledCall2.Stop();

			// Since this is the control and we don't clear the cache, we check that the performance difference is *not* present:
			Assert.IsFalse(cachedCall.ElapsedTicks * 125 < uncachedCall1.ElapsedTicks);
			Assert.IsFalse(cachedCall.ElapsedTicks * 125 < uncalledCall2.ElapsedTicks);
		}
	}
}
