using System;

namespace PhpSerializerNET;


internal class PhpDeserializer {
	private readonly PhpDeserializationOptions _options;
	private PhpSerializeToken _token;

	internal PhpDeserializer(string input, PhpDeserializationOptions options) {
		this._options = options ?? PhpDeserializationOptions.DefaultOptions;
		this._token = new PhpTokenizer(input, this._options.InputEncoding).Tokenize();
	}

	private ObjectDeserializer GetObjectDeserializer() {
		ObjectDeserializer deserializer;
		PrimitiveDeserializer primitiveDeserializer;
		ArrayDeserializer arraydeserializer;
		if (this._options.EnableTypeLookup && this._token.ContainsObjects()) {
			deserializer =  new TypedObjectDeserializer(this._options);
			primitiveDeserializer = new TypedPrimitiveDeserializer(this._options);
			arraydeserializer = new TypedArrayDeserializer(this._options);

			arraydeserializer.PrimitiveDeserializer = primitiveDeserializer;
			arraydeserializer.ObjectDeserializer = deserializer;

			deserializer.PrimitiveDeserializer = primitiveDeserializer;
			deserializer.ArrayDeserializer = arraydeserializer;
			return deserializer;
		}
		deserializer  = new UntypedObjectDeserializer(this._options);
		primitiveDeserializer = new DefaultPrimitiveDeserializer(this._options);
		arraydeserializer = new UntypedArrayDeserializer(this._options);

		arraydeserializer.PrimitiveDeserializer = primitiveDeserializer;
		arraydeserializer.ObjectDeserializer = deserializer;

		deserializer.PrimitiveDeserializer = primitiveDeserializer;
		deserializer.ArrayDeserializer = arraydeserializer;
		return deserializer;
	}

	internal object Deserialize() {
		var primitiveDeserializer = new DefaultPrimitiveDeserializer(this._options);
		switch (this._token.Type) {
			case PhpSerializerType.Array: {
					var arraydeserializer = new UntypedArrayDeserializer(this._options);
					arraydeserializer.ObjectDeserializer = this.GetObjectDeserializer();
					arraydeserializer.PrimitiveDeserializer = primitiveDeserializer;

					arraydeserializer.ObjectDeserializer.ArrayDeserializer = arraydeserializer;
					arraydeserializer.ObjectDeserializer.PrimitiveDeserializer = arraydeserializer.PrimitiveDeserializer;
					return arraydeserializer.Deserialize(this._token);
				}
			case PhpSerializerType.Object: {
				return this.GetObjectDeserializer().Deserialize(this._token);
			}
			default:
				return new DefaultPrimitiveDeserializer(this._options).Deserialize(this._token);
		}
	}

	internal object Deserialize(Type targetType) {
		var primitiveDeserializer = new TypedPrimitiveDeserializer(this._options);
		var arrayDeserializer = new TypedArrayDeserializer(this._options);
		var objectDeserializer = new TypedObjectDeserializer(this._options);

		objectDeserializer.PrimitiveDeserializer = primitiveDeserializer;
		arrayDeserializer.PrimitiveDeserializer = primitiveDeserializer;

		objectDeserializer.ArrayDeserializer = arrayDeserializer;
		arrayDeserializer.ObjectDeserializer = objectDeserializer;
		switch (this._token.Type) {
			case PhpSerializerType.Array: {
				return arrayDeserializer.Deserialize(this._token, targetType);
			}
			case PhpSerializerType.Object: {
				return objectDeserializer.Deserialize(this._token, targetType);
			}
			default:
				return primitiveDeserializer.Deserialize(this._token, targetType);
		}
	}

	internal T Deserialize<T>() {
		return (T)this.Deserialize(typeof(T));
	}


	/// <summary>
	/// Reset the type lookup cache.
	/// Can be useful for scenarios in which new types are loaded at runtime in between deserialization tasks.
	/// </summary>
	public static void ClearTypeCache() {
		TypeLookup.ClearTypeCache();
	}

	/// <summary>
	/// Reset the property info cache.
	/// Can be useful for scenarios in which new types are loaded at runtime in between deserialization tasks.
	/// </summary>
	public static void ClearPropertyInfoCache() {
		TypeLookup.ClearPropertyInfoCache();
	}
}
