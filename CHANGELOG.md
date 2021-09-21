# 0.5.1

- Fixed misleading exception message on malformed objects.
- Fixed valid classnames being rejected as malformed.
- Fixed type-lookup logic trying to deserialize with `null` Type information.

Known issues:
- Objects with classname `DateTime` will fail to deserialize, unless the option `EnableTypeLookup` is set to `false`.

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