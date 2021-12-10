
---

# PhpIgnore

Tells both the serializer and deserializer to ignore a given property. 

When deserializing, the property or field will assume the default value.

When serializing, the property will be omitted entirely. That means both the key and the value in arrays and objects.

## Example

```C#
public class MyClass {
	public string Foo {get;set;};

	[PhpIgnore]
	public string Bar {get;set;};
}

var myObject = PhpSerialization.Deserialize<MyClass>(
	"a:2:{s:3:\"Foo\";s:3:\"abc\";s:3:\"Bar\";s:3:\"xyz\";}"
);
// myObject.Bar will be null, despite the array having a "Bar" element with a value.

myObject.Bar = "xyz";
var serialized = PhpSerialization.Serialize(
	myObject
);
// serialized data will be: "a:2:{s:3:"Foo";s:3:"abc";}" with the "Bar" property ignored.
```