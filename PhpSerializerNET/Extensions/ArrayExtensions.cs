/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
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

		public static Dictionary<string, PropertyInfo> GetAllProperties(this PropertyInfo[] properties, PhpDeserializationOptions options) {
			var result = new Dictionary<string, PropertyInfo>();
			foreach (var property in properties) {
				var isIgnored = false;
				var attributes = PhpPropertyAttribute.GetCustomAttributes(property, false);
				PhpPropertyAttribute phpPropertyAttribute = null;
				foreach (var attribute in attributes) {
					if (attribute is PhpIgnoreAttribute) {
						isIgnored = true;
						break;
					}
					if (attribute is PhpPropertyAttribute foundAttribute) {
						phpPropertyAttribute = foundAttribute;
					}
				}
				if (phpPropertyAttribute != null) {
					var attributeName = options.CaseSensitiveProperties
						? phpPropertyAttribute.Name
						: phpPropertyAttribute.Name.ToLower();
					result.Add(attributeName, isIgnored ? null : property);
				}
				var propertyName = options.CaseSensitiveProperties
						? property.Name
						: property.Name.ToLower();
				result.Add(propertyName, isIgnored ? null : property);
			}
			return result;
		}

		public static Dictionary<string, FieldInfo> GetAllFields(this FieldInfo[] fields, PhpDeserializationOptions options) {
			var result = new Dictionary<string, FieldInfo>();
			foreach (var field in fields) {
				var isIgnored = false;
				var attributes = PhpPropertyAttribute.GetCustomAttributes(field, false);
				PhpPropertyAttribute phpPropertyAttribute = null;
				foreach (var attribute in attributes) {
					if (attribute is PhpIgnoreAttribute) {
						isIgnored = true;
						break;
					}
					if (attribute is PhpPropertyAttribute foundAttribute) {
						phpPropertyAttribute = foundAttribute;
					}
				}
				if (phpPropertyAttribute != null) {
					var attributeName = options.CaseSensitiveProperties
						? phpPropertyAttribute.Name
						: phpPropertyAttribute.Name.ToLower();
					result.Add(attributeName, isIgnored ? null : field);
				}

				var fieldName = options.CaseSensitiveProperties
						? field.Name
						: field.Name.ToLower();
				result.Add(fieldName, isIgnored ? null : field);
			}
			return result;
		}
	}
}