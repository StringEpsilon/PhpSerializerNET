[back to overview](../README.md)

---

# PhpObjectDictionary

Extends [System.Collections.Generic.Dictionary<string, object>](https://docs.microsoft.com/de-de/dotnet/api/system.collections.generic.dictionary-2?view=net-6.0)
Implements [`IPhpObject`](./IPhpObject.md)

This is also the default return type for deserialized objects.

Aside from the `GetClassName()` and `SetClassName()` implementations for the IPhpObject interface, this class behaves exactly like any other Dictionary.

Like the PhpDynamicObject, it can also be used to create arbitrary PHP Objects for serialization:

```c#
var objectDictionary = new PhpObjectDictionary();
objectDictionary.Add("firstname", "Joseph");
objectDictionary.Add("lastname", "Bishop");
objectDictionary.SetClassName("Person");

PhpSerialization.Serialize(myObject);
// O:6:"Person":2:{s:9:"firstname";s:6:"Joseph";s:8:"lastname";s:6:"Bishop";}
```

**Important**:

`PhpObjectDictionary` only supports string keys. You can not deserialize PHP objects with integer keys using this type. This may be addressed in future releases.