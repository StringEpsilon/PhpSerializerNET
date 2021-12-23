# How to contribute

## Bug reports and feature requests.

0. Please indicate clearly if you report a bug or want a new feature added. 
1. Open one issue per feature and/or bug. This makes tracking issues much easier.
2. For bug reports: State clearly what is the expected behavior vs. the actual behavior.
3. For feature requests: Describe the desired feature in as much detail as you can, preferrably with a desccription / example of the API you want added.

## Pull requests

0. Please add a changelog item for every user facing change you added. See [changelog](#changelog)
1. Please add unit tests for fixed bugs or new features.

## Changelog

Please write the changelog items in such a way that everyone who uses the library can understand the impact of the change. They should convey what a new feature means for them, if a bugfix affects them or what it means to to catch up with a breaking change. Remember that not every user of the libary has knowledge of it's internals - so commit messages rarely make for a good changelog entry.

The changelog is loosely formatted, but I've more or less settled on this general structure:

```md
<!-- Version number as headline. Or simply "Future". -->
# "x.y (future)" or "Future"

## Breaking changes
<!-- Any changes to the public API that is not backwards compatible, changes in the output of either Deserialization or Serialization operations, changes in the default options and so on. -->

**Deserialization:**
<!-- Breaking changes to deserialization. -->

**Serialization:**
<!-- Breaking changes to serialization. -->

## Regular changes
<!-- Added features, bugfixes and performance improvements. -->

**Deserialization:**
<!-- Changes to deserialization. -->

**Serialization:**
<!-- Changes to Serialization. -->
``` 
