/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using System.Globalization;

namespace PhpSerializerNET {
	/// <summary>
	/// PHP Serialization format token. Holds type, length, position (of the token in the input string) and child information.
	/// </summary>
	internal struct PhpSerializeToken {
		internal PhpSerializerType Type;
		internal string Value;

		internal List<PhpSerializeToken> Children;
		internal int Position;

		/// <summary>
		/// Convert the token value to a <see cref="long"/>.
		/// </summary>
		/// <returns>
		/// The token value as a <see cref="long"/>.
		/// </returns>
		internal long ToLong() {
			return long.Parse(this.Value, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Convert the token value to a <see cref="double"/>.
		/// </summary>
		/// <returns>
		/// The token value as a <see cref="double"/>.
		/// </returns>
		internal double ToDouble() {
			return this.Value switch {
				"INF" => double.PositiveInfinity,
				"-INF" => double.NegativeInfinity,
				"NAN" => double.NaN,
				_ => double.Parse(this.Value, CultureInfo.InvariantCulture),
			};
			;
		}

		/// <summary>
		/// Convert the token value to a <see cref="bool"/>
		/// </summary>
		/// <returns>
		/// The token value as a <see cref="bool"/>
		/// </returns>
		internal bool ToBool() {
			return this.Value == "1";
		}
	}
}