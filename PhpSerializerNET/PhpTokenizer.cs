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
			this._inputEncoding = inputEncoding;
			this._input = Encoding.Convert(
				Encoding.Default,
				this._inputEncoding,
				Encoding.Default.GetBytes(input)
			);
			this._position = 0;
			this._lastIndex = this._input.Length - 1;
		}

		private void CheckBounds(string expectation) {
			if (this._lastIndex < this._position) {
				throw new DeserializationException(
					$"Unexpected end of input. Expected '{expectation}' at index {this._position}, but input ends at index {this._lastIndex}"
				);
			}
		}

		private void CheckBounds(char expectation) {
			if (this._lastIndex < this._position) {
				throw new DeserializationException(
					$"Unexpected end of input. Expected '{expectation}' at index {this._position}, but input ends at index {this._lastIndex}"
				);
			}
		}

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

		private void GetTerminator() {
			this.GetCharacter(';');
		}

		private void GetDelimiter() {
			this.GetCharacter(':');
		}

		private string GetNumbers(bool isFloating) {
			bool valid = true;
			int start = this._position;
			int end = this._position;

			for (; this._input[this._position] != ';' && this._position < this._lastIndex && valid; this._position++) {
				valid = (char)this._input[this._position] switch {
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
						$"Unexpected token at index {this._position}. " +
						$"'{(char)this._input[this._position]}' is not a valid part of a {(isFloating ? "floating point " : "")}number."
					);
				}
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

		private void GetBracketClose() {
			this.GetCharacter('}');
		}

		private void GetBracketOpen() {
			this.GetCharacter('{');
		}

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

		internal PhpSerializeToken GetToken() {
			var result = new PhpSerializeToken {
				Position = this._position,
				Type = this.GetDataType()
			};
			switch (result.Type) {
				case PhpSerializerType.Boolean: {
						this.GetDelimiter();
						result.Value = this.GetBoolean();
						this.GetTerminator();
						break;
					}
				case PhpSerializerType.Null: {
						this.GetTerminator();
						break;
					}
				case PhpSerializerType.String: {
						this.GetDelimiter();
						int length = this.GetLength(result.Type);
						this.GetDelimiter();
						this.GetCharacter('"');
						result.Value = this.GetNCharacters(length);
						this.GetCharacter('"');
						this.GetTerminator();
						break;
					}
				case PhpSerializerType.Integer: {
						this.GetDelimiter();
						result.Value = this.GetNumbers(false);
						this.GetTerminator();
						break;
					}
				case PhpSerializerType.Floating: {
						this.GetDelimiter();
						result.Value = this.GetNumbers(true);
						this.GetTerminator();
						break;
					}
				case PhpSerializerType.Array: {
						this.GetDelimiter();
						int length = this.GetLength(result.Type);
						this.GetDelimiter();
						this.GetBracketOpen();
						result.Children = new(length * 2);
						while (this._input[this._position] != '}') {
							result.Children.Add(this.GetToken());
						}
						this.GetBracketClose();
						if (length * 2 != result.Children.Count) {
							throw new DeserializationException(
								$"Array at position {result.Position} should be of length {length}, " +
								$"but actual length is {result.Children.Count / 2}."
							);
						}
						break;
					}
				case PhpSerializerType.Object: {
						this.GetDelimiter();
						int classNamelength = this.GetLength(result.Type);
						this.GetDelimiter();
						this.GetCharacter('"');
						result.Value = this.GetNCharacters(classNamelength);
						this.GetCharacter('"');
						this.GetDelimiter();
						int propertyCount = this.GetLength(result.Type);
						this.GetDelimiter();
						this.GetBracketOpen();
						result.Children = new(propertyCount * 2);
						while (this._input[this._position] != '}') {
							result.Children.Add(this.GetToken());
						}
						this.GetBracketClose();

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
			var result = this.GetToken();
			if (this._position <= this._lastIndex) {
				throw new DeserializationException($"Unexpected token '{(char)this._input[this._position]}' at position {this._position}.");
			}
			return result;
		}
	}
}