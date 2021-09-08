<center>

[![Nuget](https://img.shields.io/nuget/v/PhpSerializerNET?style=flat-square)](https://www.nuget.org/packages/PhpSerializerNET/) [![GitHub](https://img.shields.io/github/license/StringEpsilon/PhpSerializerNET?style=flat-square)](https://github.com/StringEpsilon/PhpSerializerNET/blob/main/LICENSE) [![Azure DevOps builds](https://img.shields.io/azure-devops/build/StringEpsilon/StringEpsilon/4?style=flat-square)](https://dev.azure.com/StringEpsilon/StringEpsilon/_build/latest?definitionId=4&branchName=main) ![Build Status](https://img.shields.io/azure-devops/coverage/StringEpsilon/StringEpsilon/4/main?style=flat-square)
</center>

-----

# PhpSerializerNET

This is a .NET library for working with the [PHP serialization format](https://en.wikipedia.org/wiki/PHP_serialization_format).

Usage is rather simple:

```c#
using PhpSerializerNET;

string serializedData = PhpSerializer.Serialize(myObject);
```

[and for deserializing, see here](./docs/Deserializing.md)


## TODOs

* [ ] Documentation.
* [ ] Write better exceptions.
	- Partially done.
* [ ] Check the compliance with the serialization format more closely while parsing.
* [ ] Deduplicate some of the code
* [ ] General polish.
* [ ] Cover all features and most error handlinng with unit tests.

## Non-TODO:

- Support for the object notation PHP has for the format. I simply don't need it. But if you do, please submit a PR.

## Why?

I'm working with some legacy project that uses the format and the only other implementation of a serializer/deserializer I found had no license attached and I needed something GPL compatible. So wrote this one from scratch.

## License

This project is licenced under the MPL 2.0.
