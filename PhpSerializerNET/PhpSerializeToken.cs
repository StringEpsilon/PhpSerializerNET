/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PhpSerializerNET {
	internal class PhpSerializeToken {
		internal PhpSerializerType Type { get; set; }
		internal int Length { get; set; }
		internal string Value { get; set; }

		internal List<PhpSerializeToken> Children { get; set; }
		internal int Position { get; set; }

		internal PhpSerializeToken() {
		}

		internal object ToObject(PhpDeserializationOptions options) {
			switch (this.Type) {
				case PhpSerializerType.Null:
					return null;
				case PhpSerializerType.Boolean:
					return this.ToBool();
				case PhpSerializerType.Integer:
					return this.ToLong();
				case PhpSerializerType.Floating:
					return this.ToDouble();
				case PhpSerializerType.String:
					return this.Value;
				case PhpSerializerType.Array:
					return this.ToCollection(options);
				default:
					throw new Exception("Unsupported datatype.");
			}
		}

		internal long ToLong() {
			return long.Parse(this.Value, CultureInfo.InvariantCulture);
		}

		internal double ToDouble() {
			switch (this.Value) {
				case "INF":
					return double.PositiveInfinity;
				case "-INF":
					return double.NegativeInfinity;
				case "NAN":
					return double.NaN;
				default:
					return double.Parse(this.Value, CultureInfo.InvariantCulture);
			};
		}


		internal object ToCollection(PhpDeserializationOptions options) {
			var result = new Dictionary<object, object>();
			for (int i = 0; i < this.Children.Count; i += 2) {
				result.Add(this.Children[i].ToObject(options), this.Children[i + 1].ToObject(options));
			}
			if (this.Length != result.Count()) {
				throw new DeserializationException(
					$"Array at position {this.Position} should be of length {this.Length}, but actual length is {result.Count}."
				);
			}

			if (options.UseLists != ListOptions.Never) {
				if (result.Any(x => x.Key is not long)) {
					return result;
				}

				if (options.UseLists == ListOptions.Default) {
					var orderedEntries = result.OrderBy(x => (long)x.Key);
					var previousKey = ((long)orderedEntries.First().Key) - 1;
					var resultList = new List<object>();
					foreach (var entry in orderedEntries) {
						if ((long)entry.Key == previousKey + 1) {
							previousKey = (long)entry.Key;
							resultList.Add(entry.Value);
						} else {
							return result;
						}
					}
					return resultList;
				} else {
					return result.Values.ToList();
				}
			}
			return result;

		}

		internal IConvertible ToBool() {
			return this.Value == "1" ? true : false;
		}
	}
}