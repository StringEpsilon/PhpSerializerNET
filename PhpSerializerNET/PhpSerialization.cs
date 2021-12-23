/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

// Consumers of this library may use https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references
#nullable enable

using System;
using System.Collections.Generic;

namespace PhpSerializerNET {
	public static class PhpSerialization {

		/// <summary>
		/// Deserialize the given string into an object.
		/// </summary>
		/// <param name="input">
		/// Data in the PHP de/serialization format.
		/// </param>
		/// <param name="options">
		/// Options for deserialization. See the  <see cref="PhpDeserializationOptions"/> class for more details.
		/// </param>
		/// <returns>
		/// <see cref="null" />, <br/>
		/// <see cref="bool" />, <br/>
		/// <see cref="long" />, <br/>
		/// <see cref="double" />, <br/>
		/// <see cref="string" />, <br/>
		/// <see cref="List{object}"/> for arrays with integer keys <br/>
		/// <see cref="Dictionary{object,object}"/> for arrays with mixed keys or objects <br/>
		/// <see cref="PhpDynamicObject"/> for objects (see options).
		/// </returns>
		public static object? Deserialize(string input, PhpDeserializationOptions? options = null) {
			if (string.IsNullOrEmpty(input)) {
				throw new ArgumentException("PhpSerialization.Deserialize(): Parameter 'input' must not be null or empty.");
			}
			return new PhpDeserializer(input, options).Deserialize();
		}

		/// <summary>
		/// The serialized data to deserialize.
		/// </summary>
		/// <param name="input">
		/// Data in the PHP de/serialization format.
		/// </param>
		/// <param name="options">
		/// Options for deserialization. See the  <see cref="PhpDeserializationOptions"/> class for more details.
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
			PhpDeserializationOptions? options = null
		) {
			if (string.IsNullOrEmpty(input)) {
				throw new ArgumentException("PhpSerialization.Deserialize(): Parameter 'input' must not be null or empty.");
			}
			return new PhpDeserializer(input, options).Deserialize<T>();
		}

		/// <summary>
		/// The serialized data to deserialize.
		/// </summary>
		/// <param name="input">
		/// Data in the PHP de/serialization format.
		/// </param>
		/// <param name="options">
		/// Options for deserialization. See the  <see cref="PhpDeserializationOptions"/> class for more details.
		/// </param>
		/// <param name="type">
		/// The desired output type.
		/// This should be one of the primitives or a class with a public parameterless constructor.
		/// </typeparam>
		/// <returns>
		/// The deserialized object.
		/// </returns>
		public static object? Deserialize(
			string input,
			Type type,
			PhpDeserializationOptions? options = null
		) {
			if (string.IsNullOrEmpty(input)) {
				throw new ArgumentException("PhpSerialization.Deserialize(): Parameter 'input' must not be null or empty.");
			}
			return new PhpDeserializer(input, options).Deserialize(type);
		}

		/// <summary>
		/// Serialize an object into the PHP format.
		/// </summary>
		/// <param name="input">
		/// Object to serialize.
		/// </param>
		/// <returns>
		/// String representation of the input object.
		/// Note that circular references are terminated with "N;"
		/// Arrays, lists and dictionaries are serialized into arrays.
		/// Objects may also be serialized into arrays, if their respective struct or class does not have the <see cref="PhpClass"/> attribute.
		/// </returns>
		public static string Serialize(object? input, PhpSerializiationOptions? options = null) {
			return new PhpSerializer(options)
				.Serialize(input) ?? throw new NullReferenceException($"{nameof(PhpSerializer)}.{nameof(Serialize)} returned null");
		}

		/// <summary>
		/// Reset the type lookup cache.
		/// Can be useful for scenarios in which new types are loaded at runtime in between deserialization tasks.
		/// </summary>
		public static void ClearTypeCache() =>
			PhpDeserializer.ClearTypeCache();

		/// <summary>
		/// Reset the property info cache.
		/// Can be useful for scenarios in which new types are loaded at runtime in between deserialization tasks.
		/// </summary>
		public static void ClearPropertyInfoCache() =>
			PhpDeserializer.ClearPropertyInfoCache();
	}
}
