[back to overview](../README.md)

---

**Table of contents**
1. [Dynamic](#Dynamic)
	1. [Deserializing Arrays](#Deserializing-Arrays)
	2. [Deserializing Objects](#Deserializing-Objects)
2. [Explicetly typed](#Explicetly-typed)

---

# Deserializing

## "Dynamic"

Simply call `PhpSerialization.Deserialize()` with your serialized data. The returned object might be a downcast class or a boxed primitive, depending on the deserialized data.

```c#
using PhpSerializerNET;

object myData = PhpSerialization.Deserialize("s:3:\"Foo\";");
// myData is string && myData == "Foo"
```

`Deserialize()` will return:

- `null` - for `N;`
- `boolean` - for `b:x;`
- `long` - for `i:x;`
- `double` - for `d:x;`
- `string` - for `s:x:"y";`
- `List<object>` - for `a:x:{...}` (see [deserializing arrays](#Deserializing-Arrays))
- `Dictionary<object, object>` - for `a:x:{...}` (see [deserializing arrays](#Deserializing-Arrays))
- Possibly Any class or struct instance for objects (`O:x:"y":z:{...}`), see [deserializing objects](#Deserializing-Objects)

And a variety of options for PHP objects, see [deserializing objects](#Deserializing-Objects) for details.

You can also control some aspects of deserialization and what data structures will be produced via the `options` parameter:

```c#
using PhpSerializerNET;

object myData = PhpSerialization.Deserialize(
	"s:3:\"Foo\";",
	new PhpDeserializationOptions(){
		// These are all the defaults options:
		CaseSensitiveProperties = true,
		AllowExcessKeys = false,
		UseLists = ListOptions.Default,
		EmptyStringToDefault = true,
		NumberStringToBool = false,
		InputEncoding = Encoding.UTF8,
		StdClass = StdClassOption.Dictionary,
		EnableTypeLookup = true,
	}
);
```

More details on each options and examples are found in the [PhpDeserializationOptions documentation](../Options/PhpDeserializationOptions.md).

## Deserializing Arrays

Since PHP arrays are associative, they can represent either a straight forward list / arrays, a sparse list, or a dictionary.
Currently, this library only supports deserialing into `List<object>`s and `Dictionary<object,object>`s. What is used depends on the data of the array and your settings.

By default, a run of consecutive integer keys will create a `List<object>`:

```C#
object myData = PhpSerialization.Deserialize(
	"a:2:{i:0;s:1:\"a\";i:0;s:1:\"b\";}"
);

// myData == List<object> { "a", "b" }
```
For more info on that behavior, see [ListOptions.Default](../Options/PhpDeserializationOptions.md#Default).


If non-integer keys are encountered, a dictionary will be created instead:
```C#
object myData = PhpSerialization.Deserialize(
	"a:2:{s:1:\"0\";s:1:\"a\";s:1:\"0\";s:1:\"b\";}"
);

// myData == Dictionary<object, object> { { Key = "0", Value = "a" }, { Key = "1", Value = "b" } }
```

This can also have entirely mixed datatypes:

```C#
object myData = PhpSerialization.Deserialize(
	"a:2:{i:0;s:1:\"a\";s:1:\"0\";s:1:\"b\";b:1;s:1:\"c\",d:1.33;s:1:\"d\"}"
);

/*
myData == Dictionary<object, object> {
	{ Key = 0, Value = "a" },
	{ Key = "1", Value = "b" },
	{ Key = true, Value = "c" },
	{ Key = 1.33, Value = "d" },
}
*/
```

If you always want to use dictionaries, you can set `UseLists` to [`ListOptions.Never`](../Options/PhpDeserializationOptions.md#Never).

## Deserializing Objects

This highly depends on the settings. Please refer to:

- [`StdClass`](../Options/PhpDeserializationOptions.md#StdClass).
- [`EnableTypeLookup`](../Options/PhpDeserializationOptions.md#EnableTypeLookup).

And to an extend also:

- [`AllowExcessKeys`](../Options/PhpDeserializationOptions.md#EnableTypeLookup).

The default is that type lookup is enabled and `stdClass` will be deserialized into [`PhpObjectDictionary`](../Types/PhpObjectDictionary.md).

## Explicetly typed

In addition to having boxed or downcast `object`s, you can also specify the target type if you know the structure of the data in advance.

```c#
using PhpSerializerNET;

string myData = PhpSerialization.Deserialize<string>("s:3:\"Foo\";");
// myData == "Foo"
```

Arrays and Objects can be deserialized directly onto known classes and structs too:

```c#
/*  Example 1: Object to struct. */
public struct MyStruct {
	public int Foo { get; set; }
	public double Bar { get; set; }
}

var myData = PhpSerialization.Deserialize<MyStruct>("a:4:{s:3:\"Foo\";i:1000;s:3:\"Bar\";d:3.1415;}");
// myData.Foo == 1000
// myData.Bar == 3.1415
```

```c#
/*  Example 2: Object to struct. */
public struct MyStruct {
	public int Foo { get; set; }
	public double Bar { get; set; }
}

var myData = PhpSerialization.Deserialize<MyStruct>("O:8:\"sdtClass\":2:{s:3:\"Foo\";i:1000;s:3:\"Bar\";d:3.1415;}");
// myData.Foo == 1000
// myData.Bar == 3.1415
```

Additionally, you may also specify the type of collection for an array. Please note that the constraints of list creation outlined in ["Deserializing Arrays"](#Deserializing-Arrays) apply.

```c#
/*  Example 3: List of strings. */
var myData = PhpSerialization.Deserialize<List<string>>("a:6:{i:0;s:1:\"a\";i:1;s:1:\"b\";i:3;s:1:\"c\";}");
// myData.Foo == List<string> { "a", "b", "c" }
```

You can also cast integers into enums:

```c#
/*  Example 4: Enums. */
public enum MyEnum {
	Foo = 1,
	Bar = 2,
	Baz = 3
}

var result = PhpSerialization.Deserialize<MyEnum>("i:2;");
// result = MyEnum.Bar
```

If value or structure can not be assigned to the specified type, a `DeserializationException` is thrown with some information about what assignement failed. Please pay attention to the InnerException in those cases.

```c#
/*  Example 5: Wrong. */
public struct MyStruct {
	public int Foo { get; set; }
	public double Bar { get; set; }
}

try {
	PhpSerialization.Deserialize<MyStruct>("b:1;");
} catch (DeserializationException ex){
	// ex.Message == "Can not assign value \"1\" (at position 0) to target type of MyStruct."
}
```

```c#
/*  
Example 6: Missing member. */
public struct MyStruct {
	public int Foo { get; set; }
	public double Bar { get; set; }
}

try {
	PhpSerialization.Deserialize<MyStruct>(
		"a:1:{s:15:\"Rumpelstiltskin\";i:666;}"
	);
} catch (DeserializationException ex){
	// ex.Message == "Could not bind the key \"Rumpelstiltskin\" to struct of type \"MyStruct\": No such field."
}
```
