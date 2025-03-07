/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

// Consumers of this library may use https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references
#nullable enable

using System;
using System.Collections.Generic;
using System.Text;

namespace PhpSerializerNET;

public static class PhpSerialization {
	/// <summary>
	/// Reset the type lookup cache.
	/// Can be useful for scenarios in which new types are loaded at runtime in between deserialization tasks.
	/// </summary>
	public static void ClearTypeCache() => TypeLookup.ClearTypeCache();

	/// <summary>
	/// Reset the property info cache.
	/// Can be useful for scenarios in which new types are loaded at runtime in between deserialization tasks.
	/// </summary>
	public static void ClearPropertyInfoCache()  => TypeLookup.ClearPropertyInfoCache();

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
			throw new ArgumentOutOfRangeException(nameof(input), "PhpSerialization.Deserialize(): Parameter 'input' must not be null or empty.");
		}
		if (options == null)  {
			options = PhpDeserializationOptions.DefaultOptions;
		}
		int size = options.InputEncoding.GetByteCount(input);
		Span<byte> inputBytes = size < 256
			? stackalloc byte[size]
			: new byte[size];
		options.InputEncoding.GetBytes(input, inputBytes);
		int tokenCount = PhpTokenValidator.Validate(inputBytes);
		Span<PhpToken> tokens = new PhpToken[tokenCount];
		PhpTokenizer.Tokenize(inputBytes, tokens);
		return new PhpDeserializer(tokens, inputBytes, options).Deserialize();
	}

	public static object? DeserializeUtf8(
		in ReadOnlySpan<byte> input,
		PhpDeserializationOptions? options = null
	) {
		if (input.Length == 0) {
			throw new ArgumentOutOfRangeException
				(nameof(input),
				"PhpSerialization.DeserializeUtf8(): Parameter 'input' must not be empty."
			);
		}
		if (options == null)  {
			options = PhpDeserializationOptions.DefaultOptions;
		} else if (options.InputEncoding != Encoding.UTF8) {
			throw new ArgumentException("Can not use input encoding other than UTF8", nameof(options));
		}
		int tokenCount = PhpTokenValidator.Validate(input);
		Span<PhpToken> tokens = new PhpToken[tokenCount];
		PhpTokenizer.Tokenize(input, tokens);
		return new PhpDeserializer(tokens, input, options).Deserialize();
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
	public static T? Deserialize<T>(string input, PhpDeserializationOptions? options = null)
		=> (T?)Deserialize(input, typeof(T), options);

	public static T? DeserializeUtf8<T>(ReadOnlySpan<byte> input, PhpDeserializationOptions? options = null)
		=> (T?)DeserializeUtf8(input, typeof(T), options);


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
			throw new ArgumentOutOfRangeException(nameof(input), "PhpSerialization.Deserialize(): Parameter 'input' must not be null or empty.");
		}
		if (options == null)  {
			options = PhpDeserializationOptions.DefaultOptions;
		}
		int size = options.InputEncoding.GetByteCount(input);
		Span<byte> inputBytes = size < 256
			? stackalloc byte[size]
			: new byte[size];
		options.InputEncoding.GetBytes(input, inputBytes);
		int tokenCount = PhpTokenValidator.Validate(inputBytes);
		Span<PhpToken> tokens = new PhpToken[tokenCount];
		PhpTokenizer.Tokenize(inputBytes, tokens);
		return new PhpDeserializer(tokens, inputBytes, options).Deserialize(type);
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
	public static object? DeserializeUtf8(
		ReadOnlySpan<byte> input,
		Type type,
		PhpDeserializationOptions? options = null
	) {
		if (input.Length == 0) {
			throw new ArgumentOutOfRangeException
				(nameof(input),
				"PhpSerialization.DeserializeUtf8(): Parameter 'input' must not be empty."
			);
		}
		if (options == null)  {
			options = PhpDeserializationOptions.DefaultOptions;
		} else if (options.InputEncoding != Encoding.UTF8) {
			throw new ArgumentException("Can not use input encoding other than UTF8", nameof(options));
		}
		int tokenCount = PhpTokenValidator.Validate(input);
		Span<PhpToken> tokens = new PhpToken[tokenCount];
		PhpTokenizer.Tokenize(input, tokens);
		return new PhpDeserializer(tokens, input, options).Deserialize(type);
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
}
