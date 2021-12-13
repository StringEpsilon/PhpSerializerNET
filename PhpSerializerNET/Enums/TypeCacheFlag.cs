/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;

namespace PhpSerializerNET {
	[Flags]
	public enum TypeCacheFlag {
		/// <summary>
		/// Do not cache anything.
		/// Beware: This can cause severe performance degradation when dealing with lots of the same Objects in the data to deserialize.
		/// </summary>
		Deactivated = 0,
		/// <summary>
		/// Enable or disable lookup in currently loaded assemblies for target classes and structs to deserialize objects into.
		/// i.E. `o:8:"UserInfo":...` being mapped to a UserInfo class.
		/// Note: This does not affect use of PhpSerialization.Deserialize<T>()
		/// </summary>
		ClassNames = 1,
		/// <summary>
		/// Enable or disable cache for property information of classes and structs that are handled during deserialization.
		/// This can speed up work signifcantly when dealing with a lot of instances of those types but might decrease performance when dealing with
		/// lots of structures or only deserializing a couple instances.
		/// </summary>
		PropertyInfo = 2,
	}
}