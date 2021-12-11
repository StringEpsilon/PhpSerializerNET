[back to overview](../README.md)

---

# Class PhpDateTime

POCO for the PHP `DateTime` class.

PHP documentation is available here: https://www.php.net/manual/en/class.datetime.php

## Constructor

Only has the default constructor.

## Methods

### SetClassName

Implementation of [`IPhpObject.SetClassName`](./IPhpObject.md#SetClassName).
This method has no effect, because PhpDateTime always has a fixed classname.

**Parameters:**
- `string` - No effect.

**Returns:** `void`

### GetClassName

Implementation of [`IPhpObject.GetClassName`](./IPhpObject.md#GetClassName).
This method always returns "DateTime".

**Parameters:** None.

**Returns:** `string` - Always "DateTime"

## Properties

### Date

**Type:** `string`
**PhpProperty name**: "date"

### TimezoneType

**Type:** `int`
**PhpProperty name**: "timezone_type"

### Timezone

**Type:** `string`
**PhpProperty name**: "timezone"