using System;
using System.Runtime.CompilerServices;
using PhpSerializerNET;

internal static class FormatValidator {
	internal static int Validate(in ReadOnlySpan<byte> input) {
		int count = 0;
		int position = 0;
		Validate(input, ref count, ref position);
		if (input.Length > position) {
			throw new DeserializationException($"Unexpected token '{(char)input[position]}' at position {position}.");
		}
		return count;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Validate(ReadOnlySpan<byte> input, ref int count, ref int position) {
		switch (input[position]) {
			case (byte)'r':
			case (byte)'R':
				VisitReference(input, ref position);
				break;
			case (byte)'b':
				VisitBoolean(input, ref position);
				break;
			case (byte)'N':
				VisitNull(input, ref position);
				break;
			case (byte)'s':
				VisitString(input, ref position);
				break;
			case (byte)'i':
				VisitInteger(input, ref position);
				break;
			case (byte)'d':
				VisitDouble(input, ref position);
				break;
			case (byte)'a':
				VisitArray(input, ref count, ref position);
				break;
			case (byte)'O':
				VisitObject(input, ref count, ref position);
				break;
			default:
				throw new DeserializationException(
					$"Unexpected token '{(char)input[position]}' at position {position}."
				);
		}
		count++;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void VisitToken(ReadOnlySpan<byte> input, byte token, ref int position) {
		if (input.Length <= position) {
			throw new DeserializationException(
				$"Unexpected end of input. Expected '{(char)token}' at index {position}, but input ends at index {position-1}"
			);
		}
		if (input[position] != token) {
			throw new DeserializationException(
				$"Unexpected token at index {position}. Expected '{(char)token}' but found '{(char)input[position]}' instead."
			);
		}
		position++;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int VisitLength(ReadOnlySpan<byte> input, string dataType, ref int position) {
		int length = 0;
		for (; position < input.Length; position++) {
			switch (input[position]) {
				case (byte)':':
					return length;
				case >= (byte)'0' and <= (byte)'9':
					length = length * 10 + (input[position] - 48);
					break;
				default:
					throw new DeserializationException(
						$"{dataType} at position {position} has illegal, missing or malformed length."
					);
			}
		}
		return length;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void VisitDigits(ReadOnlySpan<byte> input, ref int position) {
		if (input[position] == (byte)';') {
			throw new DeserializationException(
				$"Unexpected token at index {position}: Expected number, but found ';' instead."
			);
		}
		for (; position < input.Length; position++) {
			switch (input[position]) {
				case (byte)';':
					return;
				case (byte)'+':
				case (byte)'-':
				case >= (byte)'0' and <= (byte)'9':
					break;
				default:
					throw new DeserializationException(
						$"Unexpected token at index {position}. '{(char)input[position]}' is not a valid part of a number."
					);
			}
		}
		// Edgecase: input ends here without a delimeter following:
		throw new DeserializationException(
			$"Unexpected end of input. Expected ';' at index {position}, but input ends at index {input.Length-1}"
		);
	}

	private static void VisitReference(in ReadOnlySpan<byte> input, ref int position) {
		// r:1234;
		position++; // ":1234;"
		VisitToken(input, (byte)':', ref position); // "1234;"
		VisitDigits(input, ref position); // ";"
		VisitToken(input, (byte)';', ref position); // ""
	}

	private static void VisitDouble(in ReadOnlySpan<byte> input, ref int position) {
		// i:1234;
		position++; // ":1234;"
		VisitToken(input, (byte)':', ref position); // "1234;"
		if (input[position] == (byte)';') {
			throw new DeserializationException(
				$"Unexpected token at index {position}: Expected floating point number, but found ';' instead."
			);
		}
		for (; position < input.Length; position++) {
			switch (input[position]) {
				case (byte)';':
					position++;
					return;
				case (byte)'+':
				case (byte)'.':
				case (byte)'-':
				case (byte)'E' or (byte)'e': // exponents.
				case (byte)'I' or (byte)'F': // infinity.
				case (byte)'N' or (byte)'A': // NaN.
				case >= (byte)'0' and <= (byte)'9':
					break;
				default:
					throw new DeserializationException(
						$"Unexpected token at index {position}. '{(char)input[position]}' is not a valid part of a floating point number."
					);
			}
		}
		// Edgecase: input ends here without a delimeter following:
		throw new DeserializationException(
			$"Unexpected end of input. Expected ';' at index {position}, but input ends at index {input.Length-1}"
		);
	}

	private static void VisitInteger(in ReadOnlySpan<byte> input, ref int position) {
		// i:1234;
		position++; // ":1234;"
		VisitToken(input, (byte)':', ref position); // "1234;"
		VisitDigits(input, ref position); // ";"
		VisitToken(input, (byte)';', ref position); // ""
	}

	private static void VisitString(in ReadOnlySpan<byte> input, ref int position) {
		// s:11:"Hello World";
		position++; // ':11:"Hello World";'
		VisitToken(input, (byte)':', ref position); // '11:"Hello World";'
		int length = VisitLength(input, "String", ref position); // ':"Hello World";'
		VisitToken(input, (byte)':', ref position); // '"Hello World";'
		VisitToken(input, (byte)'"', ref position); // 'Hello World";'
		if (position + length >= input.Length) {
			throw new DeserializationException(
				$"Illegal length of {length}. The string at position {position} points to out of bounds index {position + length}."
			);
		}
		position += length; // '";'
		VisitToken(input, (byte)'"', ref position); // ';'
		VisitToken(input, (byte)';', ref position); // ''
	}

	private static void VisitNull(in ReadOnlySpan<byte> input, ref int position) {
		// 'N;'
		position++; // ';'
		VisitToken(input, (byte)';', ref position); // ''
	}

	private static void VisitBoolean(in ReadOnlySpan<byte> input, ref int position) {
		// 'b:0;'
		position++; // ':0;'
		VisitToken(input, (byte)':', ref position); // '0;'
		if (position >= input.Length ) {
			throw new DeserializationException(
				$"Unexpected end of input. Expected '0' or '1' at index {position}, but input ends at index {input.Length-1}"
			);
		}
		if (input[position] != (byte)'0' && input[position] != (byte)'1') {
			throw new DeserializationException(
				$"Unexpected token in boolean at index {position}. "
				+ $"Expected either '1' or '0', but found '{(char)input[position]}' instead."
			);
		}
		position++; // ';'
		VisitToken(input, (byte)';', ref position); // '0;'
	}

	private static void VisitArray(in ReadOnlySpan<byte> input, ref int count, ref int position) {
		// 'a:2:{i:0;b:1;i:1;b:2;}'
		int arrayStart = position;
		position++; // ':2:{i:0;b:1;i:1;b:2;}'
		VisitToken(input, (byte)':', ref position); // '2:{i:0;b:1;i:1;b:2;}'
		int length = VisitLength(input, "Array", ref position); // ':{i:0;b:1;i:1;b:2;}'
		VisitToken(input, (byte)':', ref position); // '{i:0;b:1;i:1;b:2;}'
		VisitToken(input, (byte)'{', ref position); // 'i:0;b:1;i:1;b:2;}'
		int i = 0;
		while (input[position] != (byte)'}') {
			Validate(input, ref count, ref position); // 'b:1;i:1;b:2;}'
			Validate(input, ref count, ref position); // 'i:1;b:2;}'
			i++;
			if (i > length) {
				throw new DeserializationException(
					$"Array at position {arrayStart} should be of length {length}, " +
					$"but actual length is {i} or more."
				);
			}
		}
		// '}'
		VisitToken(input, (byte)'}', ref position);
	}

	private static void VisitObject(in ReadOnlySpan<byte> input, ref int count, ref int position) {
		// 'O:8:"stdClass":2:{i:0;b:1;i:1;b:2;}'
		int objectStart = position;
		position++; // ':8:"stdClass":2:{i:0;b:1;i:1;b:2;}'
		VisitToken(input, (byte)':', ref position); // '8:"stdClass":2:{i:0;b:1;i:1;b:2;}'
		int nameLength = VisitLength(input, "Object", ref position); // '8:"stdClass":2:{i:0;b:1;i:1;b:2;}'
		VisitToken(input, (byte)':', ref position); // '"stdClass":2:{i:0;b:1;i:1;b:2;}'
		VisitToken(input, (byte)'"', ref position); // 'stdClass":2:{i:0;b:1;i:1;b:2;}'
		if (position + nameLength >= input.Length) {
			throw new DeserializationException(
				$"Illegal length of {nameLength}. The string at position {position} points to out of bounds index {position + nameLength}."
			);
		}
		position += nameLength; // '":2:{i:0;b:1;i:1;b:2;}'
		VisitToken(input, (byte)'"', ref position); // ':2:{i:0;b:1;i:1;b:2;}'
		VisitToken(input, (byte)':', ref position); // '2:{i:0;b:1;i:1;b:2;}'
		int length = VisitLength(input, "Object", ref position); // ':{i:0;b:1;i:1;b:2;}'
		VisitToken(input, (byte)':', ref position); // '{i:0;b:1;i:1;b:2;}'
		VisitToken(input, (byte)'{', ref position); // 'i:0;b:1;i:1;b:2;}'
		int i = 0;
		while (input[position] != (byte)'}') {
			Validate(input, ref count, ref position); // 'b:1;i:1;b:2;}'
			Validate(input, ref count, ref position); // 'i:1;b:2;}'
			i++;
			if (i > length) {
				throw new DeserializationException(
					$"Object at position {objectStart} should have {length} properties, " +
					$"but actually has {i} or more properties."
				);
			}
		}
		// '}'
		VisitToken(input, (byte)'}', ref position);
	}
}
