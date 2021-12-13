
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;

namespace PhpSerializerNET {
	/// <summary>
	/// Indicates that instances of the decorated class or struct should be serialized into objects.
	///
	/// Will also be used to find the proper deserialization target on deserialization, see the <see cref="PhpDeserializationOptions"/>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	public class PhpClass : Attribute {
		public string Name { get; set; }

		public PhpClass(string name = null) {
			this.Name = name;
		}
	}
}