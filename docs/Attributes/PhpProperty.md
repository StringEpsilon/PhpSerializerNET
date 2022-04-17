[back to overview](../README.md)

---

**Table of contents**
1. [PhpProperty](#PhpProperty)
	1. [Signatures](#signatures)
1. [Examples](#Examples)
	1. [Example 1: Serialize](#Example-1:-Serialize)
	2. [Example 2: Serialize with integer keys](#Example-1:-Serialize)
	3. [Example 3: Deserialize with classname](#Example-3:-Deserialize)
	4. [Example 4: Deserialization with actual name.](#Example-4:-Deserialization-with-actual-name)
	4. [Example 5: Deserialize with integer names](#Example-5:-Deserialize-with-integer-names)


# PhpProperty

Specifiy the name of the property in the serialization data, for both serialization and deserialization.

Please be aware that the actual name of the property may be used as a fallback for assignment. See [example 3](#Example-1:-Deserialize-with-actual-name).

## Signatures

You can use the PhpProperty attribute with both a string and an integer argument.

```cs
[PhpProperty("string")]
```

```cs
[PhpProperty(42)]
```

Indicating that the de/serialization should use the string and integer values as the key for the property respectively.

**Important**:
Deserialization of objects and arrays with integer keys may yield unexpected results and or exceptions on certain target types. `PhpObjectDictionary` and `PhpDynamicObject` only supports string keys, for example, and will throw an exception.

# Examples

## Example 1: Serialize


```C#
public class ExampleClass {
	[PhpProperty("Foo")]
	public string FirstElement { get; set; }
	[PhpProperty("Bar")]
	public string SecondElement { get; set; }
}

var serialized = PhpSerialization.Serialize(
	new ExampleClass(){ FirstElement = "abc", SecondElement = "xyz"}
);
// "serialized" will be:
// a:2:{s:3:"Foo";s:3:"abc";s:3:"Bar";s:3:"xyz";}
```

## Example 1: Serialize with integer keys


```C#
public class ExampleClass {
	[PhpProperty1(0)]
	public string FirstElement { get; set; }
	[PhpProperty(1)]
	public string SecondElement { get; set; }
}

var serialized = PhpSerialization.Serialize(
	new ExampleClass(){ FirstElement = "abc", SecondElement = "xyz"}
);
// "serialized" will be:
// a:2:{i:0;s:3:"abc";i:1;s:3:"xyz";}
```


## Example 3: Deserialize

```C#
public class ExampleClass {
	[PhpProperty("Foo")]
	public string FirstElement { get; set; }
	[PhpProperty("Bar")]
	public string SecondElement { get; set; }
}

var deserialized = PhpSerialization.Deserialize<ExampleClass>(
	"a:2:{s:3:\"Foo\";s:3:\"abc\";s:3:\"Bar\";s:3:\"xyz\";}"
);

// deserialized.FirstElement == "abc"
// deserialized.SecondElement == "xyz"
```

## Example 4: Deserialization with actual name

```C#
public class ExampleClass {
	public string FirstElement { get; set; }
	public string SecondElement { get; set; }
}

var deserialized = PhpSerialization.Deserialize<ExampleClass>(
	"a:2:{s:12:\"FirstElement\";s:13:\"abc\";s:13:\"SecondElement\";s:3:\"xyz\";}"
);

// The serialization uses the actual property names, so they will be used for assignment. Hence:
// deserialized.FirstElement == "abc"
// deserialized.SecondElement == "xyz"
```

## Example 5: Deserialize with integer names

```C#
public class ExampleClass {
	[PhpProperty(0)]
	public string FirstElement { get; set; }
	[PhpProperty(1)]
	public string SecondElement { get; set; }
}

var deserialized = PhpSerialization.Deserialize<ExampleClass>(
	"a:2:{i:0;s:3:\"abc\";i:1;s:3:\"xyz\";}"
);

// deserialized.FirstElement == "abc"
// deserialized.SecondElement == "xyz"
```