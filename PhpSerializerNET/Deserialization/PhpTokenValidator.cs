/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace PhpSerializerNET;

internal ref struct PhpTokenValidator {
	private int _position;
	private int _tokenCount = 0;
	private readonly ReadOnlySpan<byte> _input;
	private readonly int _lastIndex;

	internal PhpTokenValidator(in ReadOnlySpan<byte> input) {
		this._input = input;
		this._position = 0;
		this._lastIndex = this._input.Length - 1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckBounds(string expectation) {
		if (this._lastIndex < this._position) {
			throw new DeserializationException(
				$"Unexpected end of input. Expected '{expectation}' at index {this._position}, but input ends at index {this._lastIndex}"
			);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckBounds(char expectation) {
		if (this._lastIndex < this._position) {
			throw new DeserializationException(
				$"Unexpected end of input. Expected '{expectation}' at index {this._position}, but input ends at index {this._lastIndex}"
			);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private PhpDataType GetDataType() {
		var result = this._input[this._position] switch {
			(byte)'N' => PhpDataType.Null,
			(byte)'b' => PhpDataType.Boolean,
			(byte)'s' => PhpDataType.String,
			(byte)'i' => PhpDataType.Integer,
			(byte)'d' => PhpDataType.Floating,
			(byte)'a' => PhpDataType.Array,
			(byte)'O' => PhpDataType.Object,
			_ => throw new DeserializationException($"Unexpected token '{(char)this._input[this._position]}' at position {this._position}.")
		};
		this._position++;
		return result;
	}

	private void GetCharacter(char character) {
		this.CheckBounds(character);
		if (this._input[this._position] != character) {
			throw new DeserializationException(
				$"Unexpected token at index {this._position}. Expected '{character}' but found '{(char)this._input[this._position]}' instead."
			);
		}
		this._position++;
	}


	private void GetTerminator() {
		this.GetCharacter(';');
	}

	private void GetDelimiter() {
		this.GetCharacter(':');
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetNumbers(bool isFloating) {
		bool valid = true;
		int end = this._position;

		for (; this._input[this._position] != ';' && this._position < this._lastIndex && valid; this._position++) {
			_ = (char)this._input[this._position] switch {
				>= '0' and <= '9' => true,
				'+' => true,
				'-' => true,
				'.' => isFloating,
				'E' or 'e' => isFloating, // exponents.
				'I' or 'N' or 'F' => isFloating, // infinity.
				'N' or 'A' => isFloating, // NaN.
				_ => throw new DeserializationException(
					$"Unexpected token at index {this._position}. " +
					$"'{(char)this._input[this._position]}' is not a valid part of a {(isFloating ? "floating point " : "")}number."
				),
			};
			end++;
		}

		this._position = end;

		// Edgecase: input ends here without a delimeter following. Normal handling would give a misleading exception:
		if (this._lastIndex == this._position && (char)this._input[this._position] != ';') {
			throw new DeserializationException(
				$"Unexpected end of input. Expected ':' at index {this._position}, but input ends at index {this._lastIndex}"
			);
		}
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int GetLength(PhpDataType dataType) {
		int length = 0;

		for (; this._input[this._position] != ':' && this._position < this._lastIndex; this._position++) {
			length = (char)this._input[this._position] switch {
				>= '0' and <= '9' => length * 10 + (_input[_position] - 48),
				_ => throw new DeserializationException(
					$"{dataType} at position {this._position} has illegal, missing or malformed length."
				),
			};
		}
		return length;
	}

	private void GetBoolean() {

		this.CheckBounds("0' or '1");
		var item = this._input[this._position];
		if (item !=  48 && item != 49) {
			throw new DeserializationException(
				$"Unexpected token in boolean at index {this._position}. Expected either '1' or '0', but found '{(char)item}' instead."
			);
		}
		this._position++;
	}

	private void GetBracketClose() {
		this.GetCharacter('}');
	}

	private void GetBracketOpen() {
		this.GetCharacter('{');
	}

	private void GetNCharacters(int length) {
		if (this._position + length > this._lastIndex) {
			throw new DeserializationException(
				$"Illegal length of {length}. The string at position {this._position} points to out of bounds index {this._position + length}."
			);
		}
		this._position += length;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void GetToken() {
		switch (this.GetDataType()) {
			case PhpDataType.Boolean:
				this.GetBooleanToken();
				break;
			case PhpDataType.Null:
				this.GetNullToken();
				break;
			case PhpDataType.String:
				this.GetStringToken();
				break;
			case PhpDataType.Integer:
				this.GetIntegerToken();
				break;
			case PhpDataType.Floating:
				this.GetFloatingToken();
				break;
			case PhpDataType.Array:
				this.GetArrayToken();
				break;
			case PhpDataType.Object:
				this.GetObjectToken();
				break;
		};
		this._tokenCount++;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetObjectToken() {
		int position = _position - 1;
		this.GetDelimiter();
		int classNamelength = this.GetLength(PhpDataType.Object);
		this.GetDelimiter();
		this.GetCharacter('"');
		this.GetNCharacters(classNamelength);
		this.GetCharacter('"');
		this.GetDelimiter();
		int propertyCount = this.GetLength(PhpDataType.Object);
		this.GetDelimiter();
		this.GetBracketOpen();
		int i = 0;
		while (this._input[this._position] != '}') {
			this.GetToken();
			i++;
			if (i > propertyCount*2) {
				throw new DeserializationException(
					$"Object at position {position} should have {propertyCount} properties, " +
					$"but actually has {(i + 1) / 2} or more properties."
				);
			}
		}

		this.GetBracketClose();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetArrayToken() {
		int position = _position - 1;
		this.GetDelimiter();
		int length = this.GetLength(PhpDataType.Array);
		this.GetDelimiter();
		this.GetBracketOpen();
		int maxTokenCount = length * 2;
		int i = 0;
		while (this._input[this._position] != '}') {
			this.GetToken();
			i++;
			if (i > maxTokenCount) {
				throw new DeserializationException(
					$"Array at position {position} should be of length {length}, " +
					$"but actual length is {(int)((i + 1) / 2)} or more."
				);
			}
		}
		this.GetBracketClose();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetFloatingToken() {
		this.GetDelimiter();
		this.GetNumbers(true);
		this.GetTerminator();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetIntegerToken() {
		this.GetDelimiter();
		this.GetNumbers(false);
		this.GetTerminator();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetStringToken() {
		this.GetDelimiter();
		int length = this.GetLength(PhpDataType.String);
		this.GetDelimiter();
		this.GetCharacter('"');
		this.GetNCharacters(length);
		this.GetCharacter('"');
		this.GetTerminator();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetNullToken() {
		this.GetTerminator();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetBooleanToken() {
		this.GetDelimiter();
		this.GetBoolean();
		this.GetTerminator();
	}

	/// <summary>
	/// Validate the PHP data and return the number of tokens found.
	/// </summary>
	/// <param name="input"> The raw UTF8 bytes of PHP data to validate. </param>
	/// <returns> The number of tokens found. </returns>
	/// <exception cref="DeserializationException"></exception>
	internal static int Validate(ReadOnlySpan<byte> input) {
		var validatior = new PhpTokenValidator(input);
		validatior.GetToken();
		if (validatior._position <= validatior._lastIndex) {
			throw new DeserializationException($"Unexpected token '{(char)input[validatior._position]}' at position {validatior._position}.");
		}
		return validatior._tokenCount;
	}
}
