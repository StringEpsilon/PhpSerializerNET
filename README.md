# PhpSerializerNET

This is a .NET library for working with the [PHP serialization format](https://en.wikipedia.org/wiki/PHP_serialization_format).

Usage is rather simple:

```c#
using PhpSerializerNET;

string serializedData = PhpSerializer.Serialize(myObject);
```

and

```c#
using PhpSerializerNET;

object deserializedObject = PhpSerializer.Deserialize("s:12:\"Hello World!\"");
```

This library also supports deserialising into specific datatypes:

```c#
var greeting = PhpSerializer.Deserialize<string>("s:12:\"Hello World!\"");
```

Tested and supported are:
- `List<T>`
- `Dictionary<TKey, TValue>`
- `string`
- `int`
- `long`
- `double`
- `string`
- `bool`

And any classes with a public parameterless constructor.

## Known Issues

- Some legal / valid string values can throw off the parser and result in exceptions.
	- See PhpSerializer.cs, line 390. It just searches for `;}`.

## TODOs

- Documentation.

- Write better exceptions.
- Check the compliance with the serialization format more closely while parsing.
- Deduplicate some of the code, especially around the IConvertible handling.
- Split off the tokenizer into it's own class.
- General polish.

## Why?

I'm working with some legacy project that uses the format and the only other implementation of a serializer/deserializer I found had no license attached and I needed something GPL compatible. So wrote this one from scratch.

## License

This project is licenced under the MPL 2.0.