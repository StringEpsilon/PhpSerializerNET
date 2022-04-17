[back to overview](../README.md)

---

# Class PhpDynamicObject

Extends [System.Dynamic.DynamicObject](https://docs.microsoft.com/en-us/dotnet/api/system.dynamic.dynamicobject?view=net-6.0)
Implements [`IPhpObject`](./IPhpObject.md).

A dynamic object that can hold any property, used for deserializing object notation while providing a way to access the specified classname.

**Important**:

`PhpObjectDictionary` only supports string keys. You can not deserialize PHP objects with integer keys using this type. This may be addressed in future releases.

## Methods

### SetClassName
Implementation of [`IPhpObject.SetClassName`](./IPhpObject.md#SetClassName).

### GetClassName
Implementation of [`IPhpObject.GetClassName`](./IPhpObject.md#GetClassName).

### TryGetMember
Overrides [System.Dynamic.DynamicObject.TryGetMember()](https://docs.microsoft.com/en-us/dotnet/api/system.dynamic.dynamicobject.trygetmember?view=net-6.0)

### TrySetMember
Overrides [System.Dynamic.DynamicObject.TrySetMember()](https://docs.microsoft.com/en-us/dotnet/api/system.dynamic.dynamicobject.trygetmember?view=net-6.0)

## Usage

Like with any dynamic object, you can just assign properties and read them:

```c#
dynamic myObject = new PhpDynamicObject();
myObject.Foo = "abc";
myObject.Bar = "def";

System.Console.WriteLine(myObject.Foo + myObject.Bar); // abcdef
```

You can also use it to construct arbitary PHP objects for serialization, including a classname.

```c#
dynamic myObject = new PhpDynamicObject();
myObject.firstname = "Joseph";
myObject.lastname = "Bishop";
myObject.SetClassName("Person");

PhpSerialization.Serialize(myObject);
// O:6:"Person":2:{s:9:"firstname";s:6:"Joseph";s:8:"lastname";s:6:"Bishop";}
```

