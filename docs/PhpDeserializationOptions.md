
---

# PhpDeserializationOptions

Options for deserializing PHP data.

## CaseSensitiveProperties

**Description:** Whether or not properties are matched case sensitive. 

**Default value:** `true`

**Example 1 - true (default):**

```C#
public class ExampleClass {
	public string Foo { get; set; }
	public string Bar { get; set; }
}

var deserialized = PhpSerialization.Deserialize<ExampleClass>(
	"a:2:{s:3:\"foo\";s:3:\"abc\";s:3:\"bar\";s:3:\"xyz\";}",
	new PhpDeserializationOptions() { 
		CaseSensitiveProperties = true,
		AllowExcessKeys = true // To avoid exception, see below.
	}
);

// deserialized.Foo == null because "foo" != "Foo"
// deserialized.bar == null because "bar" != "Bar"
```

**Example 2 - false:**

```C#
public class ExampleClass {
	public string Foo { get; set; }
	public string Bar { get; set; }
}

var deserialized = PhpSerialization.Deserialize<ExampleClass>(
	"a:2:{s:3:\"foo\";s:3:\"abc\";s:3:\"bar\";s:3:\"xyz\";}",
	new PhpDeserializationOptions() { 
		CaseSensitiveProperties = false, 
		AllowExcessKeys = true // To avoid exception, see below.
	}
);

// deserialized.Foo == "abc"
// deserialized.bar == "xyz"
```

## AllowExcessKeys

**Description:** If true, keys present in the array but not on the target type will be ignored. Otherwise an exception will be thrown.

**Default value:** `false`

**Example 1 - false (default)**

```C#
public class ExampleClass {
	public string Foo { get; set; }
	public string Bar { get; set; }
}

// Throws exception, because "Baz" is not defined on "ExampleClass":
var deserialized = PhpSerialization.Deserialize<ExampleClass>(
	"a:3:{s:3:\"Foo\";s:3:\"abc\";s:3:\"Bar\";s:3:\"xyz\";s:3:\"Baz\";s:3:\"...\";}",
	new PhpDeserializationOptions() {
		AllowExcessKeys = true
	}
);
```

**Example 2 - true**

```C#
public class ExampleClass {
	public string Foo { get; set; }
	public string Bar { get; set; }
}

// No exception, "Baz" is simply ignored and dropped.
var deserialized = PhpSerialization.Deserialize<ExampleClass>(
	"a:3:{s:3:\"Foo\";s:3:\"abc\";s:3:\"Bar\";s:3:\"xyz\";s:3:\"Baz\";s:3:\"...\";}",
	new PhpDeserializationOptions() {
		AllowExcessKeys = true
	}
);
// deserialized.Foo == "abc"
// deserialized.bar == "xyz"
```

## UseLists

**Description:** Determines how and when associative arrays are deserialized into a `List<T>` instead of a dictionary.

**Default value:** `ListOptions.Default` [details and examples here](#ListOptions)

## EmptyStringToDefault

**Description:** On deserializing an IConvertible from a PHP string, treat an empty string as the default value of the target type. For example "" => 0 for an integer.

**Default value:** `true`

## NumberStringToBool

**Description:** Whether or not to convert strings "1"` and "0" to boolean.

**Default value:** `false`

**Example 1 - false (default)**

```C#
var deserialized = PhpSerialization.Deserialize(
	"s:1:\"1\"",
	new PhpDeserializationOptions() {
		NumberStringToBool = false
	}
);
// deserialized == "1"

// Throws exception, because the string "1" can not be assigned or converted to type boolean.
var deserializedBool = PhpSerialization.Deserialize<boolean>(
	"s:1:\"1\"",
	new PhpDeserializationOptions() {
		NumberStringToBool = false
	}
);
```

**Example 2 - true**

```C#
var deserialized = PhpSerialization.Deserialize(
	"s:1:\"1\"",
	new PhpDeserializationOptions() {
		NumberStringToBool = true
	}
);
// deserialized == true

var deserializedBool = PhpSerialization.Deserialize<boolean>(
	"s:1:\"1\"",
	new PhpDeserializationOptions() {
		NumberStringToBool = false
	}
);
// deserializedBool == true
```

## InputEncoding

**Description:** Encoding of the input. Default is UTF-8. Encoding can make a difference in string lenghts and selecting the wrong encoding for a given input can cause the deserialization to fail.

**Default value:** `System.Text.Encoding.UTF8`

**Example:** 
[See the unit tests](https://github.com/StringEpsilon/PhpSerializerNET/blob/main/PhpSerializerNET.Test/Deserialize/Options/InputEncoding.cs)

## StdClass

**Description:** Target datatype for objects of type "stdClass". 

Note: This does not affect explicitly typed deserialization via ```PhpSerialization.Deserialize<T>()```

**Default value:** `StdClassOption.Dictionary` [details and examples here](#ListOptions)

## EnableTypeLookup

**Description:** Enable or disable lookup in currently loaded assemblies for target classes and structs to deserialize objects into. i.E. `o:8:"UserInfo":...` being mapped to a UserInfo class.

**Notes:** 
* Disabling this option can improve performance in some cases and reduce memory footprint. Finding a class or struct with a given name is a relatively costly operation and the library caches the results. 
* The caching might lead to some unexpected behaviors too. If the library can not find a type for a given name, the negative result will be stored for future deserializations. If a new assembly with a matchign type is loaded in the meantime, the type lookup will still fail and the fallback option (see [PhpDeserializationOptions.StdClass](#StdClass)) will be used.

**Default value:** `true`

**Example 1 - true (default)**

```c#
public class ExampleClass {
	public string Foo { get; set; }
	public string Bar { get; set; }
}

var deserialized = PhpSerialization.Deserialize(
	"O:12:\"ExampleClass\":2:{s:3:\"Foo\";s:3:\"abc\";s:3:\"Bar\";s:3:\"xyz\";}",
	new PhpDeserializationOptions() {
		EnableTypeLookup = true
	}
);
// deserialized is instance of ExampleClass
```

**Example 2 - false**

```c#
public class ExampleClass {
	public string Foo { get; set; }
	public string Bar { get; set; }
}

var deserialized = PhpSerialization.Deserialize(
	"O:12:\"ExampleClass\":2:{s:3:\"Foo\";s:3:\"abc\";s:3:\"Bar\";s:3:\"xyz\";}",
	new PhpDeserializationOptions() {
		EnableTypeLookup = false
	}
);
// deserialized is instance of PhpObjectDictionary (extending Dictionary(string, object))
// See also the StdClass option.
```

# ListOptions

Available values for [PhpDeserializationOptions.UseLists](#UseLists).

## Default

Convert associative array to list when all keys are consecutive integers. Otherwise make a dictionary.

**Notes:** 
* This means `0, 1, 2, 3, 4`, but also `9, 10, 11, 12`. The library does not check that the indizes start at 0.
* The consecutiveness is only checked in the positive direction. 

**Example**

```c#
var deserialized = PhpSerialization.Deserialize(
	"a:3:{i:1;s:1:\"a\";i:2;s:1:\"b\";i:3;s:1:\"c\";}",
	new PhpDeserializationOptions() {
		AllowExcessKeys = true
	}
);
// deserialized == List<string> { "a", "b", "c" }
```

## OnAllIntegerKeys

Convert associative array to list when all keys are integers, consecutive or not. Otherwise make a dictionary.

**Example**

```c#
var deserialized = PhpSerialization.Deserialize(
	"a:3:{i:1;s:1:\"a\";i:5;s:1:\"b\";i:200;s:1:\"c\";}",
	new PhpDeserializationOptions() {
		AllowExcessKeys = true
	}
);
// deserialized == List<string> { "a", "b", "c" }
```

## Never

Always use dictionaries.

**Example**

```c#
var deserialized = PhpSerialization.Deserialize(
	"a:3:{i:1;s:1:\"a\";i:5;s:1:\"b\";i:200;s:1:\"c\";}",
	new PhpDeserializationOptions() {
		AllowExcessKeys = true
	}
);
// deserialized == Dictionary<integer, string> { {1, "a"} , {2, "b"} , {3, "c"} }
```

# StdClassOption

Available values for [PhpDeserializationOptions.StdClass](#StdClass).

## Dictionary

Deserialize all 'stdClass' objects into `PhpObjectDictionary` (extending `Dictionary<string, object>`).

## Dynamic

Deserialize all 'stdClass' objects into dynamic objects (see `PhpDynamicObject`).

## Throw

Throw an exception and abort deserialization when encountering stdClass objects.
