/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Reflection;
using System.Text;

namespace PhpSerializerNET {
	internal static class ArrayExtensions {

		public static string Utf8Substring(this byte[] array, int start, int length, Encoding encoding) {
			byte[] substring = new byte[length];
			if (length > array.Length - start) {
				return "";
			}
			System.Buffer.BlockCopy(array, start, substring, 0, length);

			if (encoding == Encoding.UTF8) {
				return Encoding.UTF8.GetString(substring);
			} else {
				return Encoding.UTF8.GetString(
					Encoding.Convert(encoding, Encoding.UTF8, substring)
				);
			}
		}


		public static MemberInfo FindField(this FieldInfo[] fields, string name, PhpDeserializationOptions options) {
			if (!options.CaseSensitiveProperties) {
				name = name.ToLower();
			}
			// Explicetly named properties have priority:
			foreach (var field in fields) {
				PhpPropertyAttribute attribute = PhpPropertyAttribute.GetCustomAttribute(
					field,
					typeof(PhpPropertyAttribute),
					false
				) as PhpPropertyAttribute;

				if (attribute != null) {
					var propertyName = options.CaseSensitiveProperties
						? attribute.Name
						: attribute.Name.ToLower();
					if (propertyName == name) {
						return field;
					}
				}
			}
			foreach (var field in fields) {
				var propertyName = options.CaseSensitiveProperties
					? field.Name
					: field.Name.ToLower();
				if (propertyName == name) {
					return field;
				}
			}
			return null;
		}

		public static MemberInfo FindProperty(this PropertyInfo[] properties, string name, PhpDeserializationOptions options) {
			PropertyInfo member = null;
			if (!options.CaseSensitiveProperties) {
				name = name.ToLower();
			}
			// Explicetly named properties have priority:
			foreach (var property in properties) {
				PhpPropertyAttribute attribute = PhpPropertyAttribute.GetCustomAttribute(
					property,
					typeof(PhpPropertyAttribute),
					false
				) as PhpPropertyAttribute;

				if (attribute != null) {
					var propertyName = options.CaseSensitiveProperties
						? attribute.Name
						: attribute.Name.ToLower();
					if (propertyName == name) {
						return property;
					}
				}
			}
			foreach (var property in properties) {
				var propertyName = options.CaseSensitiveProperties
					? property.Name
					: property.Name.ToLower();
				if (propertyName == name) {
					return property;
				}
			}
			return member;
		}
	}
}