[back to overview](../Index.md)

---
# PhpClass

Indicates that instances of the decorated class or struct should be serialized with Object notation and with which classname (if specified)

Will also be used to find the proper deserialization target on deserialization, see the <see cref="PhpDeserializationOptions"/>

## Example 1: Serialize with classname

```C#
[PhpClass("FooBar")]
public class ExampleClass {
	public string Foo { get; set; }
	public string Bar { get; set; }
}

var serialized = PhpSerialization.Serialize(
	new ExampleClass(){ Foo = "abc", Bar = "xyz"}
);
// "serialized" will be:
// O:6:"FooBar":2:{s:3:"Foo";s:3:"abc";s:3:"Bar";s:3:"xyz";}
```

## Example 2: Serialize without classname

```C#
[PhpClass]
public class ExampleClass {
	public string Foo { get; set; }
	public string Bar { get; set; }
}

var serialized = PhpSerialization.Serialize(
	new ExampleClass(){ Foo = "abc", Bar = "xyz"}
);
// "serialized" will be:
// O:8:"stdClass":2:{s:3:"Foo";s:3:"abc";s:3:"Bar";s:3:"xyz";}
```


## Example 2: Deserialize with classname

```C#
[PhpClass("FooBar")]
public class ExampleClass {
	public string Foo { get; set; }
	public string Bar { get; set; }
}

var deserialized = PhpSerialization.Deserialize(
	"O:6:\"FooBar\":2:{s:3:\"Foo\";s:3:\"abc\";s:3:\"Bar\";s:3:\"xyz\";}"
);
// "deserialized" will be an instance of type ExampleClass.
```
