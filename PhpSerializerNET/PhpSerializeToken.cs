/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PhpSerializerNET
{
	internal class PhpSerializeToken
	{
		public PhpSerializerType Type { get; set; }
		public int Length { get; set; }
		public string Value { get; set; }

		public List<PhpSerializeToken> Children { get; set; }
		public int Position { get; internal set; }

		public PhpSerializeToken()
		{
		}

		internal object ToObject(PhpDeserializationOptions options)
		{
			return this.Type switch
			{
				PhpSerializerType.Null => null,
				PhpSerializerType.Boolean => this.Value == "1" ? true : false,
				PhpSerializerType.Integer => long.Parse(this.Value),
				PhpSerializerType.Floating => this.ParseFloat(this.Value),
				PhpSerializerType.String => this.Value,
				PhpSerializerType.Array => this.ToCollection(options),
				_ => throw new Exception("Unsupported datatype.")
			};
		}

		private double ParseFloat(string input)
		{
			return input switch
			{
				"INF" => double.PositiveInfinity,
				"-INF" => double.NegativeInfinity,
				"NAN" => double.NaN,
				_ => double.Parse(input, CultureInfo.InvariantCulture)
			};
		}

		private object ToCollection(PhpDeserializationOptions options)
		{
			var result = new Dictionary<object, object>();
			for (int i = 0; i < this.Children.Count; i += 2)
			{
				result.Add(this.Children[i].ToObject(options), this.Children[i + 1].ToObject(options));
			}
			if (this.Length != result.Count()){
				throw new DeserializationException(
					$"Array at position {this.Position} should be of length {this.Length}, but actual length is {result.Count}."
				);
			}

			if (options.UseLists != ListOptions.Never)
			{
				if (result.Any(x => x.Key is not long)){
					return result;
				}

				if (options.UseLists == ListOptions.Default)
				{
					var orderedEntries = result.OrderBy(x => (long)x.Key);
					var previousKey = ((long)orderedEntries.First().Key)-1;
					var resultList = new List<object>();
					foreach(var entry in orderedEntries){
						if ((long)entry.Key == previousKey + 1){
							previousKey = (long)entry.Key;
							resultList.Add(entry.Value);
						}else{
							return result;
						}
					}
					return resultList;
				}
				else
				{
					return result.Values.ToList();
				}
			}
			return result;

		}
	}
}