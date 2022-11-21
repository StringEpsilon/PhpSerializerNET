/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace PhpSerializerNET;

public class PhpTokenizer {

	private int _position;
	private readonly Encoding _inputEncoding;

	private readonly byte[] _input;
	private readonly int _lastIndex;

#if DEBUG
	private char DebugCurrentCharacter => (char)_input[_position];
	private char[] DebugInput => _inputEncoding.GetChars(_input);
#endif

	public PhpTokenizer(string input, Encoding inputEncoding) {
		this._inputEncoding = inputEncoding;
		this._input = Encoding.Convert(
			Encoding.Default,
			this._inputEncoding,
			Encoding.Default.GetBytes(input)
		);
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
	private PhpSerializerType GetDataType() {
		var result = (char)this._input[this._position] switch {
			'N' => PhpSerializerType.Null,
			'b' => PhpSerializerType.Boolean,
			's' => PhpSerializerType.String,
			'i' => PhpSerializerType.Integer,
			'd' => PhpSerializerType.Floating,
			'a' => PhpSerializerType.Array,
			'O' => PhpSerializerType.Object,
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

	// [MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetTerminator() {
		this.GetCharacter(';');
	}
	// [MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetDelimiter() {
		this.GetCharacter(':');
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private string GetNumbers(bool isFloating) {
		bool valid = true;
		int start = this._position;
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
		return this._input.Utf8Substring(start, end - start, this._inputEncoding);
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int GetLength(PhpSerializerType dataType) {
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private string GetBoolean() {
		this.CheckBounds("0' or '1");

		string result = (char)this._input[this._position] switch {
			'1' => "1",
			'0' => "0",
			_ => throw new DeserializationException(
				$"Unexpected token in boolean at index {this._position}. Expected either '1' or '0', but found '{(char)this._input[this._position]}' instead."
			)
		};
		this._position++;
		return result;
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetBracketClose() {
		this.GetCharacter('}');
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetBracketOpen() {
		this.GetCharacter('{');
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private string GetNCharacters(int length) {
		if (this._position + length > this._lastIndex) {
			throw new DeserializationException(
				$"Illegal length of {length}. The string at position {this._position} points to out of bounds index {this._position + length}."
			);
		}
		int start = this._position;
		this._position += length;
		return this._input.Utf8Substring(start, length, this._inputEncoding);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal PhpSerializeToken GetToken() {
		return this.GetDataType() switch {
			PhpSerializerType.Boolean => this.GetBooleanToken(),
			PhpSerializerType.Null => this.GetNullToken(),
			PhpSerializerType.String => this.GetStringToken(),
			PhpSerializerType.Integer => this.GetIntegerToken(),
			PhpSerializerType.Floating => this.GetFloatingToken(),
			PhpSerializerType.Array => this.GetArrayToken(),
			PhpSerializerType.Object => this.GetObjectToken(),
			_ => new PhpSerializeToken()
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private PhpSerializeToken GetObjectToken() {
		var result = new PhpSerializeToken() {
			Type = PhpSerializerType.Object,
			Position = _position - 1,
		};
		this.GetDelimiter();
		int classNamelength = this.GetLength(PhpSerializerType.Object);
		this.GetDelimiter();
		this.GetCharacter('"');
		result.Value = this.GetNCharacters(classNamelength);
		this.GetCharacter('"');
		this.GetDelimiter();
		int propertyCount = this.GetLength(PhpSerializerType.Object);
		this.GetDelimiter();
		this.GetBracketOpen();
		result.Children = new PhpSerializeToken[propertyCount * 2];
		int i = 0;
		try {
			while (this._input[this._position] != '}') {
				result.Children[i++] = this.GetToken();
			}
		} catch (System.IndexOutOfRangeException ex) {
			throw new DeserializationException(
				$"Object at position {result.Position} should have {propertyCount} properties, " +
				$"but actually has {(int)((i + 1) / 2)} or more properties.",
				ex
			);
		}
		this.GetBracketClose();
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private PhpSerializeToken GetArrayToken() {
		var result = new PhpSerializeToken() { Type = PhpSerializerType.Array, Position = _position - 1 };
		this.GetDelimiter();
		int length = this.GetLength(PhpSerializerType.Array);
		this.GetDelimiter();
		this.GetBracketOpen();
		result.Children = new PhpSerializeToken[length * 2];
		int i = 0;
		try {
			while (this._input[this._position] != '}') {
				result.Children[i++] = this.GetToken();
			}
		} catch (IndexOutOfRangeException ex) {
			throw new DeserializationException(
				$"Array at position {result.Position} should be of length {length}, " +
				$"but actual length is {(int)((i + 1) / 2)} or more.",
				ex
			);
		}
		this.GetBracketClose();
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private PhpSerializeToken GetFloatingToken() {
		var result = new PhpSerializeToken() { Type = PhpSerializerType.Floating, Position = _position - 1 };
		this.GetDelimiter();
		result.Value = this.GetNumbers(true);
		this.GetTerminator();
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private PhpSerializeToken GetIntegerToken() {
		var result = new PhpSerializeToken() { Type = PhpSerializerType.Integer, Position = _position - 1 };
		this.GetDelimiter();
		result.Value = this.GetNumbers(false);
		this.GetTerminator();
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private PhpSerializeToken GetStringToken() {
		var result = new PhpSerializeToken() { Type = PhpSerializerType.String, Position = _position - 1 };
		this.GetDelimiter();
		int length = this.GetLength(result.Type);
		this.GetDelimiter();
		this.GetCharacter('"');
		result.Value = this.GetNCharacters(length);
		this.GetCharacter('"');
		this.GetTerminator();
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private PhpSerializeToken GetNullToken() {
		this.GetTerminator();
		return new PhpSerializeToken() { Type = PhpSerializerType.Null, Position = _position - 2 };
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private PhpSerializeToken GetBooleanToken() {
		var result = new PhpSerializeToken() { Type = PhpSerializerType.Boolean, Position = _position - 1 };
		this.GetDelimiter();
		result.Value = this.GetBoolean();
		this.GetTerminator();
		return result;
	}

	internal PhpSerializeToken Tokenize() {
		var result = this.GetToken();
		if (this._position <= this._lastIndex) {
			throw new DeserializationException($"Unexpected token '{(char)this._input[this._position]}' at position {this._position}.");
		}
		return result;
	}
}