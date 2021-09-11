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