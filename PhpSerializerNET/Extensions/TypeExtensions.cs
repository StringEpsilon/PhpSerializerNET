/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/
using System;

namespace PhpSerializerNET {
	public static class TypeExtensions {
		/// <summary>
		/// Check if a given <see cref="System.Type"> implements IConvertible
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsIConvertible(this Type type) {
			return typeof(IConvertible).IsAssignableFrom(type);
		}
	}
}