
---

# PhpDeserializationOptions

Options for deserializing PHP data.

## CaseSensitiveProperties

**Description:** Whether or not properties are matched case sensitive. 

**Default value:** `true`

## AllowExcessKeys

**Description:** If true, keys present in the array but not on the target type will be ignored. Otherwise an exception will be thrown.

**Default value:** `false`

## UseLists

**Description:** Determines how and when associative arrays are deserialized into a `List<T>` instead of a dictionary.

**Default value:** `ListOptions.Default` [details here](#ListOptions)

## EmptyStringToDefault

**Description:** On deserializing an IConvertible from a PHP string, treat an empty string as the default value of the target type. For example "" => 0 for an integer.

**Default value:** `true`

## NumberStringToBool

**Description:** Whether or not to convert strings "1"` and "0" to boolean.

**Default value:** `false`

## InputEncoding

**Description:** Encoding of the input. Default is UTF-8. Encoding can make a difference in string lenghts and selecting the wrong encoding for a given input can cause the deserialization to fail.

**Default value:** `System.Text.Encoding.UTF8`

## StdClass

**Description:** Target datatype for objects of type "stdClass". 

Note: This does not affect explicitly typed deserialization via ```PhpSerialization.Deserialize<T>()```

**Default value:** `StdClassOption.Dictionary` [details here](#ListOptions)

## EnableTypeLookup

**Description:** Enable or disable lookup in currently loaded assemblies for target classes and structs to deserialize objects into. i.E. `o:8:"UserInfo":...` being mapped to a UserInfo class.

**Notes:** 
* Disabling this option can improve performance in some cases and reduce memory footprint. Finding a class or struct with a given name is a relatively costly operation and the library caches the results. 
* The caching might lead to some unexpected behaviors too. If the library can not find a type for a given name, the negative result will be stored for future deserializations. If a new assembly with a matchign type is loaded in the meantime, the type lookup will still fail and the fallback option (see [PhpDeserializationOptions.StdClass](#StdClass)) will be used.

**Default value:** `true`

# ListOptions

Available values for [PhpDeserializationOptions.UseLists](#UseLists).

## Default

Convert associative array to list when all keys are consecutive integers.

**Notes:** 
* This means `0, 1, 2, 3, 4`, but also `9, 10, 11, 12`. The library does not check that the indizes start at 0.
* The consecutiveness is only checked in the positive direction. 

## OnAllIntegerKeys

Convert associative array to list when all keys are integers, consecutive or not.

## Never

Always use dictionaries.

# StdClassOption

Available values for [PhpDeserializationOptions.StdClass](#StdClass).

## Dictionary

Deserialize all 'stdClass' objects into `Dictionary<string, object>`

## Dynamic

Deserialize all 'stdClass' objects dynamic objects (see `PhpDynamicObject`).

## Throw

Throw an exception and abort deserialization when encountering stdClass objects.
