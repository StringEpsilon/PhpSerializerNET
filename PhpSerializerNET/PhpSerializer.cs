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
using System.Text;

namespace PhpSerializerNET {
	public static class PhpSerializer {

		/// <summary>
		/// Deserialize the given string into an object.
		/// </summary>
		/// <param name="input">
		/// PHP serialization data.
		/// </param>
		/// <returns>
		/// The deserialized object of type:
		///
		/// object (null),
		/// <see cref="bool" />,
		/// <see cref="long" />,
		/// <see cref="double" />,
		/// <see cref="string" />,
		/// <see cref="List{object}"/>, or
		/// <see cref="Dictionary{object,object}"/>
		/// </returns>
		public static object Deserialize(string input, PhpDeserializationOptions options = null) {
			return new PhpDeserializer(input, options).Deserialize();
		}

		/// <summary>
		/// Try to deserialize the input string into a specific type.
		/// </summary>
		/// <param name="input">
		/// PHP serialization data.
		/// </param>
		/// <typeparam name="T">
		/// The desired output type.
		/// This should be one of the primitives or a class with a public parameterless constructor.
		/// </typeparam>
		/// <returns>
		/// The deserialized object.
		/// </returns>
		public static T Deserialize<T>(
			string input,
			PhpDeserializationOptions options = null
		) {
			return new PhpDeserializer(input, options).Deserialize<T>();
		}

		/// <summary>
		/// Serialize an object into the PHP format.
		/// </summary>
		/// <param name="input">
		/// Object to serialize.
		/// </param>
		/// <param name="seen">
		/// For internal use only. This might be removed in a later release.
		/// (It's to avoid circulare references causing a stackoverflow)
		/// </param>
		/// <returns>
		/// String representation of the input object.
		/// Note that circular references are terminated with "N;"
		/// and that all classes are serialized as associative arrays.
		/// </returns>
		public static string Serialize(object input) {
			var seenObjects = new List<object>();
			return Serialize(input, seenObjects);
		}

		private static string Serialize(object input, List<Object> seenObjects) {
			StringBuilder output = new StringBuilder();
			switch (input) {
				case long longValue: {
						return $"i:{longValue.ToString()};";
					}
				case int integerValue: {
						return $"i:{integerValue.ToString()};";
					}
				case double floatValue: {
						if (double.IsPositiveInfinity(floatValue)) {
							return $"d:INF;";
						}
						if (double.IsNegativeInfinity(floatValue)) {
							return $"d:-INF;";
						}
						if (double.IsNaN(floatValue)) {
							return $"d:NAN;";
						}
						return $"d:{floatValue.ToString(CultureInfo.InvariantCulture)};";
					}
				case string stringValue: {
						// Use the UTF8 byte count, because that's what the PHP implementation does:
						return $"s:{ASCIIEncoding.UTF8.GetByteCount(stringValue)}:\"{stringValue}\";";
					}
				case bool boolValue: {
						return boolValue ? "b:1;" : "b:0;";
					}
				case null: {
						return "N;";
					}
				case IDictionary dictionary: {
						if (seenObjects.Contains(input)) {
							// Terminate circular reference:
							// It might be better to make this optional. The PHP implementation seems to
							// throw an exception, from what I can tell
							return "N;";
						}
						if (dictionary.GetType().GenericTypeArguments.Count() > 0) {
							var keyType = dictionary.GetType().GenericTypeArguments[0];
							if (!keyType.IsIConvertible() && keyType != typeof(object)) {
								throw new Exception($"Can not serialize associative array with key type {keyType.FullName}");
							}
						}
						seenObjects.Add(input);
						output.Append($"a:{dictionary.Count}:");
						output.Append("{");


						foreach (DictionaryEntry entry in dictionary) {

							output.Append($"{Serialize(entry.Key)}{Serialize(entry.Value, seenObjects)}");
						}
						output.Append("}");
						return output.ToString();
					}
				case IList collection: {
						if (seenObjects.Contains(input)) {
							return "N;"; // See above.
						}
						seenObjects.Add(input);
						output.Append($"a:{collection.Count}:");
						output.Append("{");
						for (int i = 0; i < collection.Count; i++) {
							output.Append($"{Serialize(i, seenObjects)}{Serialize(collection[i], seenObjects)}");
						}
						output.Append("}");
						return output.ToString();
					}
				default: {

						if (seenObjects.Contains(input)) {
							return "N;"; // See above.
						}
						if (input.GetType().IsValueType) {
							seenObjects.Add(input);
							var fields = input.GetType().GetFields().Where(y => y.IsPublic && y.GetCustomAttribute<PhpIgnoreAttribute>() == null);
							output.Append($"a:{fields.Count()}:");
							output.Append("{");
							foreach (var field in fields) {
								var fieldName = field.GetCustomAttribute<PhpPropertyAttribute>() != null
									? field.GetCustomAttribute<PhpPropertyAttribute>().Name
									: field.Name;
								output.Append($"{Serialize(fieldName)}{Serialize(field.GetValue(input), seenObjects)}");
							}
							output.Append("}");
							return output.ToString();
						} else {
							seenObjects.Add(input);
							var properties = input.GetType().GetProperties().Where(y => y.CanRead && y.GetCustomAttribute<PhpIgnoreAttribute>() == null);
							output.Append($"a:{properties.Count()}:");
							output.Append("{");
							foreach (var property in properties) {
								var propertyName = property.GetCustomAttribute<PhpPropertyAttribute>() != null
									? property.GetCustomAttribute<PhpPropertyAttribute>().Name
									: property.Name;
								output.Append($"{Serialize(propertyName)}{Serialize(property.GetValue(input), seenObjects)}");
							}
							output.Append("}");
							return output.ToString();
						}
					}
			}
		}
	}
}
