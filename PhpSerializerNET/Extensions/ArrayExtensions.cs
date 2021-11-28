/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Linq;
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


		public static MemberInfo FindField(this FieldInfo[] array, string name, PhpDeserializationOptions options) {
			FieldInfo field = null;

			// Explicetly named properties have priority:
			field = array
				.Where(y => y.GetCustomAttribute<PhpPropertyAttribute>() != null)
				.FirstOrDefault(y =>
					options.CaseSensitiveProperties
						? y.GetCustomAttribute<PhpPropertyAttribute>().Name == name
						: y.GetCustomAttribute<PhpPropertyAttribute>().Name.ToLower() == name.ToLower()
				);

			if (field == null) {
				field = options.CaseSensitiveProperties
					? array.FirstOrDefault(y => y.Name == name)
					: array.FirstOrDefault(y => y.Name.ToLower() == name.ToLower());
			}

			return field;
		}

		public static MemberInfo FindProperty(this PropertyInfo[] array, string name, PhpDeserializationOptions options) {
			PropertyInfo member = null;

			// Explicetly named properties have priority:
			member = array
				.Where(y => y.GetCustomAttribute<PhpPropertyAttribute>() != null)
				.FirstOrDefault(y =>
					options.CaseSensitiveProperties
						? y.GetCustomAttribute<PhpPropertyAttribute>().Name == name
						: y.GetCustomAttribute<PhpPropertyAttribute>().Name.ToLower() == name.ToLower()
				);

			if (member == null) {
				member = options.CaseSensitiveProperties
					? array.FirstOrDefault(y => y.Name == name)
					: array.FirstOrDefault(y => y.Name.ToLower() == name.ToLower());
			}

			return member;
		}
	}
}