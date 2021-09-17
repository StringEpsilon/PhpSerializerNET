# Deserialize() 

```c#
using PhpSerializerNET;

object deserializedObject = PhpSerialization.Deserialize("s:12:\"Hello World!\"");
```

`Deserialize()`  will return one of the follow object types:

`bool`, `long`, `double`, `string`, `null` (object), `Dictionary<object, object>` or `List<object>`.

`List<object>` is only returned when an associative array has only consecutive integers as keys. 

You can adjust this behavior via the `PhpDeserializationOptions.UseLists` option:
`Default` => Return list on consecutive integers.
`OnAllIntegerKeys` => Return list if all keys are integers, consecutive or not.
`Never` => Always return a dictionary.

# Deserialize\<T\>()

```c#
var greeting = PhpSerialization.Deserialize<string>("s:12:\"Hello World!\"");
```

Tested and supported are:
- `List<T>`
- `Dictionary<TKey, TValue>`
- `string`
- `int`
- `long`
- `double`
- `string`
- `bool`

And any classes with a public parameterless constructor.

Currently (as of version 0.1.0), you might get some unexpected behavior when trying to deserialize into a dictionary or list specifically. Please report any issues and edgecases you find. 

When deserializing associative arrays into objects, 
`PhpDeserializationOptions.CaseSensitiveProperties` enables / disables case sensistive matching of array key to property. Default is case sensitive search.
`PhpDeserializationOptions.AllowExcessKeys` will pass over keys that are in the array data but not on the object instead of throwing an exception.

You can also `[PhpIgnore]` specific properties or map certain keys in the array to a property of a different name with `[PhpProperty("name")]`

Example:

```c#
public class ExampleClass {
	[PhpProperty("en")]
	public string English {get;set;}

	[PhpProperty("de")]
	public string German {get;set;}

	[PhpIgnore]
	public string it {get;set;}
}


var myObject = PhpSerialization.Deserialize<ExampleClass>(
	"a:3:{s:2:\"en\";s:12:\"Hello World!\";s:2:\"de\";s:11:\"Hallo Welt!\";s:2:\"it\";s:11:\"Ciao mondo!\";}"
);

// myObject.English == "Hello World!"
// myObject.German == "Hallo Welt!"
// myObject.it == null
```

# Options

You can provide an instance of `PhpDeserializationOptions` as an argument for the `Deserialize` call. 
Example with all the default values:

```c#
var foo = PhpSerialization.Deserialize(
	myData, 
	new PhpDeserializationOptions() {
		// Whether or not properties are matched case sensitive.
		public bool CaseSensitiveProperties = true,

		// If true, keys present in the array but not on the target class will be ignored.
		// Otherwise an exception will be thrown.
		public bool AllowExcessKeys = false,

		// Determines how and when associative arrays are deserialized into System.Collections.Generic.List<object>
		// instead of a dictionary.
		public ListOptions UseLists = ListOptions.Default,

		// Whether or not to convert strings "1"` and "0" to boolean.
		public bool NumberStringToBool = false;
	}
);
```

# Deserializing php objects

TODO.