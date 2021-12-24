[back to overview](../README.md)

---

# Serializing

It's as simple as this:

```c#
using PhpSerializerNET;

string serializedData = PhpSerialization.Serialize(myData);
```

## Simple types

Simple types here means strings, integers, doubles, and `null`. They are very straight forward in how they map.

```c#
Console.WriteLine(PhpSerialization.Serialize("Hello World"));
// Output: s:11:"Hello World";

Console.WriteLine(PhpSerialization.Serialize(12345));
// Output: i:12345;

Console.WriteLine(PhpSerialization.Serialize(true));
// Output: b:1;

Console.WriteLine(PhpSerialization.Serialize(null));
// Output: N;

Console.WriteLine(PhpSerialization.Serialize(3.1415));
// Output: d:3.1415;
```

## Complex types

These are your data structures. Anything `ICollection`, arrays, structs and objects.

### Collections and arrays

These will always use the array notation of `a:<length>:{<data>}`.

`IList` and arrays will be serialized using integer keys, starting with an index of 0.

```c#
Console.WriteLine(PhpSerialization.Serialize(new List<int>(){ 10, 20, 30 });
// Output: a:3:{i:0;i:10;i:1;i:20;i:2;i:30;}

Console.WriteLine(PhpSerialization.Serialize(new int[]{ 10, 20, 30 });
// Output: a:3:{i:0;i:10;i:1;i:20;i:2;i:30;}
```

`IDictionary` implementations will use whatever key is used in the dictionary instead of the integer. PHP arrays are always associative arrays with arbitrary keys, so this is perfectly valid.

```c#
Console.WriteLine(PhpSerialization.Serialize(
	new Dictionary<object, object>{ 
		{ "a", 10 },
		{ 1, 20 },
		{ true, 30 },
	}
);
// Output: a:3:{s:1:"a";i:10;i:1;i:20;b:1;i:30;}
``` 

## Objects and structs

By default, objects and structs will be serialized into an array with string keys.

You can instead use object notation by annotating your type declaration with [[PhpClass]](../Attributes/PhpClass.md) or by implementing [IPhpObject](../Types/IPhpObject.md).

## Enums

By default, enums are serialized into strings, using their `.ToString()` representation. You can also specify integer serialization with the [NumericEnums](../Options/PhpSerializiationOptions.md#NumericEnums) option.