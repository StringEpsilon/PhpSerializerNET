[back to overview](../Index.md)

---

# Class PhpDynamicObject

Extends [System.Dynamic.DynamicObject](https://docs.microsoft.com/en-us/dotnet/api/system.dynamic.dynamicobject?view=net-6.0)
Implements [`IPhpObject`](./IPhpObject.md).

A dynamic object that can hold any property, used for deserializing object notation while providing a way to access the specified classname.

## Methods

### SetClassName
Implementation of [`IPhpObject.SetClassName`](./IPhpObject.md#SetClassName).

### GetClassName
Implementation of [`IPhpObject.GetClassName`](./IPhpObject.md#GetClassName).

### TryGetMember
Overrides [System.Dynamic.DynamicObject.TryGetMember()](https://docs.microsoft.com/en-us/dotnet/api/system.dynamic.dynamicobject.trygetmember?view=net-6.0)

### TrySetMember
Overrides [System.Dynamic.DynamicObject.TrySetMember()](https://docs.microsoft.com/en-us/dotnet/api/system.dynamic.dynamicobject.trygetmember?view=net-6.0)
