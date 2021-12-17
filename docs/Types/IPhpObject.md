[back to overview](../README.md)

---

# Interface IPhpObject

The interface is for manipulating the class name of an object in regards to the serialized data. I.E. the `stdClass` in `O:8:\"stdClass":0:{}`.

Please note that for serializatin, any types that implement IPhpObject will always be serialized using object notation.

## Methods

### SetClassName

Set the class name of the object for serialization. Is also used internally when deserializing an object that implements this interface.

**Parameters:** 
- `string` - The class name to set.
**Returns:** *void*

### GetClassName

Get the class name that was specified in the serialization data of the object.

**Parameters:** *None*.
**Returns:** `string` - The class name.

