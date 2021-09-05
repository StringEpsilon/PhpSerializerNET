/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Collections.Generic;
using System.Globalization;

namespace PhpSerializerNET
{
	internal class PhpSerializeToken
	{
		public PhpSerializerType Type { get; set; }
		public int Length { get; set; }
		public string Value { get; set; }

		public List<PhpSerializeToken> Children { get; set; }

		public PhpSerializeToken()
		{
		}

		internal object ToObject()
		{
			return this.Type switch
			{
				PhpSerializerType.Null => null,
				PhpSerializerType.Boolean => this.Value == "1" ? true : false,
				PhpSerializerType.Integer => long.Parse(this.Value),
				PhpSerializerType.Floating => this.ParseFloat(this.Value),
				PhpSerializerType.String => this.Value,
				PhpSerializerType.Array => this.ToCollection(),
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

		private object ToCollection()
		{
			bool isList = true;

			for (int i = 0; i < this.Children.Count; i += 2)
			{
				if (this.Children[i].Type != PhpSerializerType.Integer)
				{
					isList = false;
					break;
				}
			}

			if (isList)
			{
				var result = new List<object>();
				for (int i = 0; i < this.Children.Count; i += 2)
				{
					result.Add(this.Children[i + 1].ToObject());
				}
				return result;
			}
			else
			{
				var result = new Dictionary<object, object>();
				for (int i = 0; i < this.Children.Count; i += 2)
				{
					result.Add(this.Children[i].ToObject(), this.Children[i + 1].ToObject());
				}
				return result;
			}
		}
	}
}