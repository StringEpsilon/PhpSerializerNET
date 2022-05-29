/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;


namespace PhpSerializerNET {

	public static class TypeLookup {
		private static readonly Dictionary<string, Type> TypeLookupCache = new() {
			{ "DateTime", typeof(PhpDateTime) }
		};

		private static readonly Dictionary<Type, Dictionary<object, PropertyInfo>> PropertyInfoCache = new();


		/// <summary>
		/// Reset the type lookup cache.
		/// Can be useful for scenarios in which new types are loaded at runtime in between deserialization tasks.
		/// </summary>
		public static void ClearTypeCache() {
			lock (TypeLookupCache) {
				TypeLookupCache.Clear();
				TypeLookupCache.Add("DateTime", typeof(PhpDateTime));
			}
		}

		/// <summary>
		/// Reset the property info cache.
		/// Can be useful for scenarios in which new types are loaded at runtime in between deserialization tasks.
		/// </summary>
		public static void ClearPropertyInfoCache() {
			lock (PropertyInfoCache) {
				PropertyInfoCache.Clear();
			}

			// lock (EnumInfoCacheSyncObject) {
			// 	EnumInfoCache.Clear();
			// }
		}

		public static Type FindTypeInAssymbly(string typeName, bool cacheEnabled) {
			Type result = null;
			if (cacheEnabled) {
				lock (TypeLookupCache) {
					if (TypeLookupCache.ContainsKey(typeName)) {
						return TypeLookupCache[typeName];
					}
				}
			}

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic)) {
				result = assembly
					.GetExportedTypes()
					.Select(type => new { type, attribute = type.GetCustomAttribute<PhpClass>() })
					// PhpClass attribute should win over classes who happen to have the name
					.OrderBy(c => c.attribute != null ? 0 : 1)
					.Where(y => y.type.Name == typeName || y.attribute?.Name == typeName)
					.Select(c => c.type)
					.FirstOrDefault();

				if (result != null) {
					break;
				}
			}
			if (cacheEnabled) {
				lock (TypeLookupCache) {
					TypeLookupCache.Add(typeName, result);
				}
			}
			return result;
		}

		public static Dictionary<object, PropertyInfo> GetPropertyInfos(Type type, PhpDeserializationOptions options) {
			bool cacheEnabled = options.TypeCache.HasFlag(TypeCacheFlag.PropertyInfo);
			if (cacheEnabled) {
				lock (PropertyInfoCache) {
					if (PropertyInfoCache.ContainsKey(type)) {
						return PropertyInfoCache[type];
					}
				}
			}
			Dictionary<object, PropertyInfo> properties = type.GetProperties().GetAllProperties(options);
			if (cacheEnabled) {
				lock (PropertyInfoCache) {
					PropertyInfoCache.Add(type, properties);
				}
			}
			return properties;
		}
	}
}
