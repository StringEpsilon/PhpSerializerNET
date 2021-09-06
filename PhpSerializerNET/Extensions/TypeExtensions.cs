/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/


using System;
using System.Globalization;

namespace PhpSerializerNET
{
	public static class TypeExtensions
	{
		/// <summary>
		/// Check if a given <see cref="System.Type"> implements IConvertible
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsIConvertible(this Type type)
		{
			return typeof(IConvertible).IsAssignableFrom(type);
		}

		/// <summary>
		/// Try to convert a given IConverible object to a type.
		/// </summary>
		/// <param name="type">
		/// The type to convert to.
		/// </param>
		/// <param name="value">
		/// Object to try to convert.
		/// </param>
		/// <param name="fallThrough">
		/// If true, return the value as-is when it could not be converted. Defaults to false.
		/// </param>
		/// <returns></returns>
		public static object Convert(this Type type, object value, bool fallThrough = false)
		{
			if (type.IsIConvertible() && value is IConvertible convertible)
			{
				return convertible.ToType(type, CultureInfo.InvariantCulture);
			}
			if (fallThrough)
			{
				return value;
			}
			throw new Exception($"Error: Can not convert {value.GetType().Name} into {type.Name}.");
		}
	}
}