[back to overview](../README.md)

---

**Table of contents**
1. [ThrowOnCircularReferences](#ThrowOnCircularReferences)
2. [NumericEnums](#NumericEnums)
3. [DefaultOptions](#DefaultOptions)

---


# PhpSerializiationOptions

## ThrowOnCircularReferences

**Description:** Whether or not to throw on encountering a circular reference or to terminate it with null.
**Type:** `boolean`
**Default:** `false`

### Example 1 - false (default)

When disabled, the offending reference will simply be `null`'d out.

```c#
public class Circular {
	public int A { get; set; }
	public Circular B { get; set; }
}

var circular = new Circular() { A = 1 };
circular.B = circular;

Console.WriteLine(PhpSerialization.Serialize(circular, new (){ ThrowOnCircularReferences = false}));
// Output: a:2:{s:1:"A";i:1;s:1:"B";N;}
```

### Example 2 - true
```c#
var circular = new Circular() { A = 1 };
circular.B = circular;

Console.WriteLine(PhpSerialization.Serialize(circular, new (){ ThrowOnCircularReferences = true }));
// Throws System.ArgumentException with Message: "Input object has a circular reference."
```

## NumericEnums

**Description:** Whether to serialize enums as numeric values (integers) or using their string representation via `Enum.ToString()`.
**Type:** `boolean`
**Default:** `false`

### Example 1 - false (default)
```c#
public enum MyEnum {
	A = 1,
	B,
}
Console.WriteLine(PhpSerialization.Serialize(MyEnum.A, new (){ NumericEnums = false }));
// Output: s:1:"A";
```

### Example 2 - true
```c#
Console.WriteLine(PhpSerialization.Serialize(MyEnum.A, new (){ NumericEnums = true }));
// Output: i:1;
```