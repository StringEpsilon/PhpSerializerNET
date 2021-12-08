# 0.9.0:

## Semi breaking:
- Type lookup: Now negative lookup results are also cached. 
	- This may also lead to undeseriable results when adding classes and structs at runtime.
	- May increase the memory footprint over time faster than before.
	- On the upside: It is significantly faster when dealing with objects where automapping doesn't work without having to disable the feature entirely.
- Different exception (System.ArgumentException) on empty input for `PhpSerialization.Deserialize<T>()`

## Regular changes

- Rewrote the parsing and validation logic, which results in different exception messages in many cases.
- Parsing: A very slight performance gain for some deserialization workloads.
- Object / struct creation: Improved performance.
- General: Reduced amount of memory allocated while deserializing.

# 0.8.0:
- Improved performance of the validation step of deserialization.
- Sped up deserializing into explicit types (particularly structs and classes) significantly.
- Sped up serialization, especially when using attribute annotated classes and structs.
- Improved exception messages on malformed inputs when deserializing.
- Cleaner exception when trying to deserialize into incompatible types (i.e. "a" to int32.).

# 0.7.4:
- Improved deserialization performance.
- Fixed invalid output when using PhpSerializiationOptions.NumericEnums = false

# 0.7.3:
- Fixed an issue with empty string deserialization, caused by the `EmptyStringToDefault` code in 0.7.2.

# 0.7.2:
- Added `EmptyStringToDefault` deserialization option, defaults to true.
	- When true, empty strings will be deserialized into the default value of the target IConvertible.
	  For example `s:0:"";` deserialized to an integer yields `0`.
	See issue #13 for details.
- Fixed a regression introduced in 0.7.1 where some data would no longer parse correctly (#12) due to improper handling of array brackets.

# 0.7.1:
- Fixed issue with nested array / object validation (issue #11)
- Added support for System.Guid (issue #10)

# 0.7.0:
- Support de/serialization of enums
- Added serialization option `NumericEnums`:
	Whether or not to serialize enums as integer values
	Defaults to true. If set to false, the enum.ToString() representation will be used.

# 0.6.0:

- Allow more (valid) characters in object class names.
- Added public interface IPhpObject
- Added public class PhpObjectDictionary (implementing IPhpObject).
	- This replaces `IDictionary<string, object>` as the default deserialization target of objects.
- Added public class PhpDynamicObject (implementing IPhpObject)
- Added PhpDateTime to avoid conflicts with System.DateTime.
 
With IPhpObjects, you can get the class name specified in the serialized data via `GetClassName()`.

**Known issues:**
- Can not deserialize dynamic objects.

# 0.5.1

- Fixed misleading exception message on malformed objects.
- Fixed valid classnames being rejected as malformed.
- Fixed type-lookup logic trying to deserialize with `null` Type information.

Known issues:
- ~~Objects with classname `DateTime` will fail to deserialize, unless the option `EnableTypeLookup` is set to `false`.~~ (fixed since)

# 0.5.0

**BREAKING**
- Renamed the static class `PhpSerializer` to `PhpSerialization`

Other changes:
- Added support for object de/serialization (`O:4:"name":n:{...}`).
- Added `[PhpClass()]` attribute.
- Added `StdClass` and `EnableTypeLookup` to deserialization options
- Added options for `PhpSerialization.Serialize()`.
	- `ThrowOnCircularReferences` - whether or not to throw on circular references, defaults to false (this might change in the future.)
- Updated and adjusted some of the XML documentation.

# 0.4.0

- Support for structs.
- Fixed performance drop due to over-checking the input string.
- Refactored deserializer to work in less steps and with cleaner code.
- Slight tweaks of some error messages.

# 0.3.0

- Added InputEncoding option.
- Added ability to deserialize into `List<MyClass>` specifically
	- Currently only works when also setting UseList = Never
- Fixed a big issue with nested arrays stepping over keys.
- Added tests.

# 0.2.0

- Added option to convert strings "1" and "0" into bools when deserializing an object.
- Changed how validation is handled and moved validation out of the tokenization step.
- Added `[PhpIgnore]` and `[PhpProperty("name")]`

# 0.1.0

- Initial release.