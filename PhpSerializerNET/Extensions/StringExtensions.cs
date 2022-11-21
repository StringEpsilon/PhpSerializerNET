using System.Globalization;

namespace PhpSerializerNET;

internal static class StringExtensions {
	internal static double PhpToDouble(this string value) {
		return value switch {
			"INF" => double.PositiveInfinity,
			"-INF" => double.NegativeInfinity,
			"NAN" => double.NaN,
			_ => double.Parse(value, CultureInfo.InvariantCulture),
		};
	}

	internal static bool PhpToBool(this string value) => value == "1";

	internal static long PhpToLong(this string value) => long.Parse(value, CultureInfo.InvariantCulture);
}