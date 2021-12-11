[back to overview](../README.md)

---

**Table of contents**
1. [PhpProperty](#PhpProperty)
	1. [Example 1: Serialize](#Example-1:-Serialize)
	2. [Example 2: Deserialize with classname](#Example-1:-Deserialize-with-classname)
	3. [Example 3: Deserialization with actual name.](#Example-1:-Deserialize-with-actual-name)

---

# PhpProperty

Specifiy the name of the property in the serialization data, for both serialization and deserialization.

Please be aware that the actual name of the property may be used as a fallback for assignment. See [example 3](#Example-1:-Deserialize-with-actual-name).

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


## Example 2: Deserialize with classname

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

## Example 3: Deserialization with actual name.

```C#
public class ExampleClass {
	[PhpProperty("Foo")]
	public string FirstElement { get; set; }
	[PhpProperty("Bar")]
	public string SecondElement { get; set; }
}

var deserialized = PhpSerialization.Deserialize<ExampleClass>(
	"a:2:{s:12:\"FirstElement\";s:13:\"abc\";s:3:\"SecondElement\";s:3:\"xyz\";}"
);

// The serialization uses the actual property names, so they will be used for assignment. Hence:
// deserialized.FirstElement == "abc"
// deserialized.SecondElement == "xyz"
```