/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

namespace PhpSerializerNET {

	/// <summary>
	/// Options for serializing objects.
	/// </summary>
	public class PhpSerializiationOptions {
		/// <summary>
		/// Whether or not to throw on encountering a circular reference or to terminate it with null. 
		/// Default false.
		/// </summary>
		public bool ThrowOnCircularReferences { get; set; } = false;

		internal static PhpSerializiationOptions DefaultOptions = new();
	}
}