/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/
using System;
using System.Globalization;
using System.Reflection;

namespace PhpSerializerNET;

internal static class TypeExtensions {
	/// <summary>
	/// Check if a given <see cref="System.Type"> implements IConvertible
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public static bool IsIConvertible(this Type type) =>
		typeof(IConvertible).IsAssignableFrom(type);

	internal static object GetValue(this MemberInfo member, object input) {
		if (member is PropertyInfo property) {
			return property.GetValue(input);
		}
		if (member is FieldInfo field) {
			return field.GetValue(input);
		}
		return null;
	}

	internal static string GetNumericString(this Enum enumValue) {
		return ((IConvertible)enumValue)
			.ToType(enumValue.GetType().GetEnumUnderlyingType(), CultureInfo.InvariantCulture)
			.ToString();
	}

	/// <summary>
	/// Whether a type is a nullable reference type, like bool? int? etc.
	/// Adapted from <see href="https://stackoverflow.com/a/374663/4122889"/>
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	internal static bool IsNullableReferenceType(this Type type) {
		return Nullable.GetUnderlyingType(type) != null;
	}
}
