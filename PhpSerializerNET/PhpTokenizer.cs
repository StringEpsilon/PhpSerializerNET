/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Linq;
using System.Text;

namespace PhpSerializerNET {
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
			_inputEncoding = inputEncoding;
			this._input = Encoding.Convert(
				Encoding.Default, 
				_inputEncoding, 
				Encoding.Default.GetBytes(input)
			);
			_position = 0;
			_lastIndex = _input.Length - 1;
		}

		private void CheckBounds(string expectation) {
			if (_lastIndex < _position) {
				throw new DeserializationException(
					$"Unexpected end of input. Expected '{expectation}' at index {_position}, but input ends at index {_lastIndex}"
				);
			}
		}

		private void CheckBounds(char expectation) {
			if (_lastIndex < _position) {
				throw new DeserializationException(
					$"Unexpected end of input. Expected '{expectation}' at index {_position}, but input ends at index {_lastIndex}"
				);
			}
		}

		private PhpSerializerType GetDataType() {
			var result = (char)_input[_position] switch {
				'N' => PhpSerializerType.Null,
				'b' => PhpSerializerType.Boolean,
				's' => PhpSerializerType.String,
				'i' => PhpSerializerType.Integer,
				'd' => PhpSerializerType.Floating,
				'a' => PhpSerializerType.Array,
				'O' => PhpSerializerType.Object,
				_ => throw new DeserializationException($"Unexpected token '{(char)_input[_position]}' at position {_position}.")
			};
			_position++;
			return result;
		}

		private void GetCharacter(char character) {
			this.CheckBounds(character);
			if (_input[_position] != character) {
				throw new DeserializationException(
					$"Unexpected token at index {_position}. Expected '{character}' but found '{(char)_input[_position]}' instead."
				);
			}
			_position++;
		}

		private void GetTerminator() {
			GetCharacter(';');
		}

		private void GetDelimiter() {
			GetCharacter(':');
		}

		private string GetNumbers(bool isFloating) {
			bool valid = true;
			int start = _position;
			int end = _position;

			for (; _input[_position] != ';' && _position < _lastIndex && valid; _position++) {
				valid = (char)_input[_position] switch {
					>= '0' and <= '9' => true,
					'+' => true,
					'-' => true,
					'.' => isFloating,
					'E' or 'e' => isFloating, // exponents.
					'I' or 'N' or 'F' => isFloating, // infinity.
					'N' or 'A' => isFloating, // NaN.
					_ => false,
				};
				if (!valid) {
					throw new DeserializationException(
						$"Unexpected token at index {_position}. " +
						$"'{(char)_input[_position]}' is not a valid part of a {(isFloating ? "floating point " : "")}number."
					);
				}
				end++;
			}

			_position = end;

			// Edgecase: input ends here without a delimeter following. Normal handling would give a misleading exception:
			if (_lastIndex == _position && (char)_input[_position] != ';') {
				throw new DeserializationException(
					$"Unexpected end of input. Expected ':' at index {_position}, but input ends at index {_lastIndex}"
				);
			}
			return _input.Utf8Substring(start, end - start, _inputEncoding);
		}


		private int GetLength(PhpSerializerType dataType) {
			int length = 0;

			for (; _input[_position] != ':' && _position < _lastIndex; _position++) {
				length = (char)_input[_position] switch {
					>= '0' and <= '9' => length * 10 + (_input[_position]-48),
					_ => throw new DeserializationException(
						$"{dataType} at position {_position} has illegal, missing or malformed length."
					),
				};
			}
			return length;
		}

		private string GetBoolean() {
			this.CheckBounds("0' or '1");

			string result = (char)_input[_position] switch {
				'1' => "1",
				'0' => "0",
				_ => throw new DeserializationException(
					$"Unexpected token in boolean at index {_position}. Expected either '1' or '0', but found '{(char)_input[_position]}' instead."
				)
			};
			_position++;
			return result;
		}

		private void GetBracketClose() {
			GetCharacter('}');
		}

		private void GetBracketOpen() {
			GetCharacter('{');
		}

		private string GetNCharacters(int length) {
			if (_position + length > _lastIndex) {
				throw new DeserializationException(
					$"Illegal length of {length}. The string at position {_position} points to out of bounds index {_position + length}."
				);
			}
			int start = _position;
			_position += length;
			return _input.Utf8Substring(start, length, _inputEncoding);
		}

		internal PhpSerializeToken GetToken() {
			var result = new PhpSerializeToken();
			result.Position = _position;
			result.Type = this.GetDataType();
			switch (result.Type) {
				case PhpSerializerType.Boolean: {
						GetDelimiter();
						result.Value = GetBoolean();
						GetTerminator();
						break;
					}
				case PhpSerializerType.Null: {
						GetTerminator();
						break;
					}
				case PhpSerializerType.String: {
						GetDelimiter();
						int length = GetLength(result.Type);
						GetDelimiter();
						GetCharacter('"');
						result.Value = GetNCharacters(length);
						GetCharacter('"');
						GetTerminator();
						break;
					}
				case PhpSerializerType.Integer: {
						GetDelimiter();
						result.Value = GetNumbers(false);
						GetTerminator();
						break;
					}
				case PhpSerializerType.Floating: {
						GetDelimiter();
						result.Value = GetNumbers(true);
						GetTerminator();
						break;
					}
				case PhpSerializerType.Array: {
						GetDelimiter();
						int length = GetLength(result.Type);
						GetDelimiter();
						GetBracketOpen();
						result.Children = new(length*2);
						while (_input[_position] != '}') {
							result.Children.Add(GetToken());
						}
						GetBracketClose();
						if (length * 2 != result.Children.Count) {
							throw new DeserializationException(
								$"Array at position {result.Position} should be of length {length}, " +
								$"but actual length is {result.Children.Count / 2}."
							);
						}
						break;
					}
				case PhpSerializerType.Object: {
						GetDelimiter();
						int classNamelength = GetLength(result.Type);
						GetDelimiter();
						GetCharacter('"');
						result.Value = GetNCharacters(classNamelength);
						GetCharacter('"');
						GetDelimiter();
						int propertyCount = GetLength(result.Type);
						GetDelimiter();
						GetBracketOpen();
						result.Children = new(propertyCount*2);
						while (_input[_position] != '}') {
							result.Children.Add(GetToken());
						}
						GetBracketClose();

						if (propertyCount * 2 != result.Children.Count) {
							throw new DeserializationException(
								$"Object at position {result.Position} should have {propertyCount} properties, " +
								$"but actually has {result.Children.Count / 2} properties."
							);
						}
						break;
					}
			}
			return result;
		}

		internal PhpSerializeToken Tokenize() {
			var result = GetToken();
			if (_position <= _lastIndex){
				throw new DeserializationException($"Unexpected token '{(char)_input[_position]}' at position {_position}.");
			}
			return result;
		}
	}
}