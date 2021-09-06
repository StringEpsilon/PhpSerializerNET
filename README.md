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

`Deserialize()`  will return one of the follow object types:

`bool`, `long`, `double`, `string`, `null` (object), `Dictionary<object, object>` or `List<object>`.

`List<object>` is only returned when an associative array only has consecutive integers as keys. 
You can adjust this behavior via the `PhpDeserializationOptions.UseLists` option:
`Default` => Return list on consecutive integers.
`OnAllIntegerKeys` => Return list if all keys are integers.
`Never` => Always return a dictionary.

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

When deserializing associative arrays into objects, 
`PhpDeserializationOptions.CaseSensitiveProperties` enables / disables case sensistive matching of array key to property. Default is case sensitive search.
`PhpDeserializationOptions.AllowExcessKeys` will pass over keys that are in the array data but not on the object instead of throwing an exception.

## TODOs

[ ] Documentation.
[ ] Write better exceptions.
	- Partially done.
[ ] Check the compliance with the serialization format more closely while parsing.
[ ] Deduplicate some of the code
[ ] General polish.
[ ] Cover all features and most error handlinng with unit tests.


And maybe publish to nuget? IDK.

## Non-TODO:

- Support for the object notation PHP has for the format. I simply don't need it. But if you do, please submit a PR.

## Why?

I'm working with some legacy project that uses the format and the only other implementation of a serializer/deserializer I found had no license attached and I needed something GPL compatible. So wrote this one from scratch.

## License

This project is licenced under the MPL 2.0.