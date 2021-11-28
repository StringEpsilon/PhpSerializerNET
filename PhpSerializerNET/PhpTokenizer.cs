/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Collections.Generic;
using System.Text;

namespace PhpSerializerNET {
	public class PhpTokenizer {

		private byte[] _inputBytes;

		private int _position;
		private PhpDeserializationOptions _options;


		public PhpTokenizer(string input, PhpDeserializationOptions options) {
			this._position = 0;
			this._options = options;
			int position = 0;
			this._inputBytes = Encoding.Convert(Encoding.Default, options.InputEncoding, Encoding.Default.GetBytes(input));
			this.ValidateFormat(ref position);
		}

		internal bool ValidateFormat(ref int position, bool inArray = false) {
			bool isTop = position == 0;
			bool expectClosingBracket = false;
			for (; position < _inputBytes.Length; position++) {
				switch ((char)_inputBytes[position]) {
					case 'N':
						if (position == _inputBytes.Length - 1 || _inputBytes[position + 1] != ';') {
							throw new DeserializationException($"Malformed null at position {position}: Expected ';'");
						}
						position += 1;
						break;
					case 'b':
						if (position + 3 >= _inputBytes.Length) {
							throw new DeserializationException($"Malformed boolean at position {position}: Unexpected end of input sequence.");
						}
						if (_inputBytes[position + 2] != '0' && _inputBytes[position + 2] != '1') {
							throw new DeserializationException($"Malformed boolean at position {position}: Only '1' and '0' are allowed.");
						}
						position += 3;
						break;
					case 'i':
						if (position + 3 >= _inputBytes.Length || _inputBytes[position + 1] != ':') {
							throw new DeserializationException($"Malformed integer at position {position}");
						}
						position += 2;
						for (; _inputBytes[position] != ';' && position < _inputBytes.Length - 1; position++) {
							bool valid = (char)_inputBytes[position] switch {
								>= '0' and <= '9' => true,
								'+' => true,
								'-' => true,
								_ => false,
							};
							if (!valid) {
								throw new DeserializationException($"Malformed integer at position {position}");
							}
						}
						if (_inputBytes[position] != ';') {
							throw new DeserializationException(
								$"Malformed integer at position {position}: Expected token ';', found '{(char)_inputBytes[position]}' instead."
							);
						}
						break;
					case 'd': {
							// smallest double: d:0;
							if (position + 3 >= _inputBytes.Length || _inputBytes[position + 1] != ':') {
								throw new DeserializationException($"Malformed double at position {position}");
							}
							position += 2;
							for (; _inputBytes[position] != ';' && position < _inputBytes.Length - 1; position++) {
								bool valid = (char)_inputBytes[position] switch {
									>= '0' and <= '9' => true,
									'+' => true,
									'-' => true,
									'.' => true,
									'E' or 'e' => true, // exponents.
									'I' or 'N' or 'F' => true, // infinity.
									'N' or 'A' => true, // NaN.
									_ => false,
								};
								if (!valid) {
									throw new DeserializationException($"Malformed integer at position {position}");
								}
							}
							if (_inputBytes[position] != ';') {
								throw new DeserializationException(
									$"Malformed double at position {position}: Expected token ';', found '{(char)_inputBytes[position]}' instead."
								);
							}
							break;
						}
					case 's': {
							// smallest string: s:0:"";
							if (position + 6 >= _inputBytes.Length) {
								throw new DeserializationException($"Malformed string at position {position}");
							}
							position += 2;
							bool valid = true;
							string lengthString = "";
							for (; _inputBytes[position] != ':' && position < _inputBytes.Length - 1 && valid; position++) {
								valid = (char)_inputBytes[position] switch {
									>= '0' and <= '9' => true,
									_ => false,
								};
								lengthString += (char)_inputBytes[position];
							}
							if (!valid) {
								throw new DeserializationException(
									$"String at position {position} has illegal, missing or malformed length."
								);
							}
							var length = int.Parse(lengthString);
							if (_inputBytes.Length < position + length + 2) {
								throw new DeserializationException(
									$"Illegal length of {length}. The string at position {position} points to out of bounds index {position + 2 + length}."
								);
							}

							if (_inputBytes[position + 1] != '"') {
								throw new DeserializationException(
									$"String at position {position} has an incorrect length of {length}: " +
									$"Expected double quote at position {position + 1}, found '{(char)_inputBytes[position + 1]}' instead."
								);
							} else {
								position++;
							}
							if (_inputBytes[position + 1 + length] != '"') {
								throw new DeserializationException(
									$"String at position {position} has an incorrect length of {length}: " +
									$"Expected double quote at position {position + 1 + length}, found '{(char)_inputBytes[position + 1 + length]}' instead."
								);
							} else {
								position += length + 2;
							}
							if (_inputBytes[position] != ';') {
								throw new DeserializationException($"Malformed string at position {position}");
							}

							break;
						}
					case 'a': {
							// smallest array: a:0:{};
							if (position + 6 >= _inputBytes.Length) {
								throw new DeserializationException($"Malformed array at position {position}");
							}
							position += 2;
							bool valid = true;
							string lengthString = "";
							for (; _inputBytes[position] != ':' && position < _inputBytes.Length - 1 && valid; position++) {
								valid = (char)_inputBytes[position] switch {
									>= '0' and <= '9' => true,
									_ => false,
								};
								lengthString += (char)_inputBytes[position];
							}
							if (!valid) {
								throw new DeserializationException(
									$"Array at position {position} has illegal, missing or malformed length."
								);
							}
							if (_inputBytes[position] != ':') {
								throw new DeserializationException(
									$"Array at position {position}: Expected token ':', found '{(char)_inputBytes[position]}'"
								);
							}
							if (_inputBytes[position+1] != '{') {
								throw new DeserializationException(
									$"Array at position {position}: Expected token '{{', found '{(char)_inputBytes[position]}'"
								);
							}
							position += 2;
							if (isTop) {
								expectClosingBracket = true;
							}
							ValidateFormat(ref position, true);
							break;
						}
					case 'O': {
							// smallest object: O:1:"a":0{};
							if (position + 12 >= _inputBytes.Length) {
								throw new DeserializationException($"Malformed object at position {position}");
							}
							position += 2;
							bool valid = true;
							string lengthString = "";
							for (; _inputBytes[position] != ':' && position < _inputBytes.Length - 1 && valid; position++) {
								valid = (char)_inputBytes[position] switch {
									>= '0' and <= '9' => true,
									_ => false,
								};
								lengthString += (char)_inputBytes[position];
							}
							if (!valid) {
								throw new DeserializationException(
									$"Object at position {position} has illegal, missing or malformed length for classname."
								);
							}
							var length = int.Parse(lengthString);
							if (_inputBytes.Length < position + length + 2) {
								throw new DeserializationException(
									$"Object class name: Illegal length of {length}. The string at position {position} points to out of bounds index {position + 2 + length}."
								);
							}

							if (_inputBytes[position + 1] != '"') {
								throw new DeserializationException(
									$"String at position {position} has an incorrect length of {length}: " +
									$"Expected double quote at position {position + 1}, found '{(char)_inputBytes[position + 1]}' instead."
								);
							} else {
								position++;
							}
							if (_inputBytes[position + 1 + length] != '"') {
								throw new DeserializationException(
									$"String at position {position} has an incorrect length of {length}: " +
									$"Expected double quote at position {position + 1 + length}, found '{(char)_inputBytes[position + 1 + length]}' instead."
								);
							} else {
								position += length + 2;
							}
							if (_inputBytes[position] != ':') {
								throw new DeserializationException($"Malformed object at position {position}: Expected ':', found {(char)_inputBytes[position]} instead.");
							}
							position++;
							for (; _inputBytes[position] != ':' && position < _inputBytes.Length - 1 && valid; position++) {
								valid = (char)_inputBytes[position] switch {
									>= '0' and <= '9' => true,
									_ => false,
								};
							}
							if (_inputBytes[position] != ':') {
								throw new DeserializationException(
									$"Object at position {position}: Expected token ':', found '{(char)_inputBytes[position]}'"
								);
							}
							if (_inputBytes[position+1] != '{') {
								throw new DeserializationException(
									$"Object at position {position}: Expected token '{{', found '{(char)_inputBytes[position]}'"
								);
							}
							position += 2;
							if (isTop) {
								expectClosingBracket = true;
							}
							ValidateFormat(ref position, true);

							break;
						}
					case '}': {
							if (expectClosingBracket) {
								expectClosingBracket = false;
							} else {
								if (inArray) {
									return true;
								} else {
									throw new DeserializationException($"Unexpected token '{(char)_inputBytes[position]}' at position {position}.");
								}
							}
							break;
						}
					default: {
							throw new DeserializationException($"Unexpected token '{(char)_inputBytes[position]}' at position {position}.");
						}
				}
			}
			return true;
		}

		internal List<PhpSerializeToken> Tokenize() {

			List<PhpSerializeToken> tokens = new();
			for (; _position < _inputBytes.Length; _position++) {
				if ((char)_inputBytes[_position] == '}') {
					return tokens;
				}
				if (_inputBytes.Length - 1 <= _position) {
					throw new DeserializationException($"Unexpected end of data at position { _position}");
				}
				switch ((char)_inputBytes[_position]) {
					case 'N': {
							tokens.Add(new PhpSerializeToken() { Type = PhpSerializerType.Null });
							_position++;
							break;
						}
					case 'b':
					case 'i':
					case 'd': {
							var tokenClose = Array.IndexOf(_inputBytes, (byte)';', _position + 1);
							var token = new PhpSerializeToken() {
								Type = (char)_inputBytes[_position] switch {
									'b' => PhpSerializerType.Boolean,
									'i' => PhpSerializerType.Integer,
									'd' => PhpSerializerType.Floating,
									_ => throw new Exception("This branch should be impossible to hit.")
								},
								Value = _inputBytes.Utf8Substring(
									_position + 2,
									tokenClose - (_position + 2),
									_options.InputEncoding
								)
							};
							tokens.Add(token);
							_position = tokenClose;
							break;
						}
					case 's': {
							var lengthStart = _position + 2;
							var lengthClose = Array.IndexOf(_inputBytes, (byte)':', lengthStart + 1);
							var valueStart = Array.IndexOf(_inputBytes, (byte)'"', lengthClose) + 1;

							var length = int.Parse(
								_inputBytes.Utf8Substring(
									lengthStart,
									lengthClose - lengthStart,
									_options.InputEncoding
								)
							);

							var value = _inputBytes.Utf8Substring(
								valueStart,
								length,
								_options.InputEncoding
							);

							if (valueStart + length >= _inputBytes.Length) {
								throw new DeserializationException(
									$"Illegal length of {length}. " +
									$"The string at position {_position} points to out of bounds index {valueStart + length}."
								);
							}
							char stringEnd = (char)_inputBytes[valueStart + length];

							tokens.Add(new PhpSerializeToken() {
								Type = PhpSerializerType.String,
								Length = length,
								Value = _inputBytes.Utf8Substring(valueStart, length, _options.InputEncoding)
							});
							_position = valueStart + length + 1;
							break;
						}
					case 'O': {
							var typeLenghtStart = _position + 2;
							var typeLengthClose = Array.IndexOf(_inputBytes, (byte)':', typeLenghtStart + 1);
							var typeLength = int.Parse(
								_inputBytes.Utf8Substring(typeLenghtStart, typeLengthClose - typeLenghtStart, _options.InputEncoding)
							);
							_position = typeLengthClose;

							var typename = _inputBytes.Utf8Substring(_position + 2, typeLength, _options.InputEncoding);
							_position += typeLength + 2;

							var lengthStart = _position + 2;
							var lengthClose = Array.IndexOf(_inputBytes, (byte)':', lengthStart + 1);
							var length = int.Parse(
								_inputBytes.Utf8Substring(lengthStart, lengthClose - lengthStart, _options.InputEncoding)
							);

							var objectToken = new PhpSerializeToken() {
								Type = PhpSerializerType.Object,
								Position = _position,
								Value = typename,
								Length = length,
								Children = this.Tokenize()
							};
							tokens.Add(objectToken);
							break;
						}
					case 'a': {
							var lengthStart = _position + 2;
							var lengthClose = Array.IndexOf(_inputBytes, (byte)':', lengthStart + 1);
							var length = int.Parse(
								_inputBytes.Utf8Substring(lengthStart, lengthClose - lengthStart, _options.InputEncoding)
							);

							_position = Array.IndexOf(_inputBytes, (byte)'{', lengthStart + 1) + 1;
							var arrayToken = new PhpSerializeToken() {
								Type = PhpSerializerType.Array,
								Position = _position,
								Length = length,
								Children = this.Tokenize()
							};
							if (arrayToken.Length != arrayToken.Children.Count / 2) {
								throw new DeserializationException(
									$"Array at position {arrayToken.Position} should be of length {arrayToken.Length}, " +
									$"but actual length is {arrayToken.Children.Count / 2}."
								);
							}
							tokens.Add(arrayToken);
							break;
						}
				}
			}
			return tokens;
		}
	}
}