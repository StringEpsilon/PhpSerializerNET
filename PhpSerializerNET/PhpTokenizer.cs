/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PhpSerializerNET {
	public class PhpTokenizer {
		private string _input;
		private byte[] _inputBytes;
		private int _position;
		private PhpDeserializationOptions _options;

		public PhpTokenizer(string input, PhpDeserializationOptions options) {
			this._input = input;
			this._position = 0;
			this._options = options;
			int position = 0;
			this.ValidateFormat(ref position);
			this._inputBytes = Encoding.Convert(Encoding.Default, options.InputEncoding, Encoding.Default.GetBytes(input));
		}

		internal bool ValidateFormat(ref int position, bool inArray = false) {
			for (; position < _input.Length; position++) {
				switch (_input[position]) {
					case 'N': {
							var match = new Regex(@"N;").Match(_input, position);
							if (!match.Success || match.Index != position) {
								throw new DeserializationException($"Malformed null at position {position}");
							}
							position += match.Length - 1;
							break;
						}
					case 'b': {
							var match = new Regex(@"b:[10];").Match(_input, position);
							if (!match.Success || match.Index != position) {
								throw new DeserializationException($"Malformed boolean at position {position}");
							}
							position += match.Length - 1;
							break;
						}
					case 'i': {
							var match = new Regex(@"i:[+-]?\d+;").Match(_input, position);
							if (!match.Success || match.Index != position) {
								throw new DeserializationException($"Malformed integer at position {position}");
							}
							position += match.Length - 1;
							break;
						}
					case 'd': {
							// Validate the correctness of the actual value in the proper parsing step:
							var match = new Regex(@"d:.+;").Match(_input, position);
							if (!match.Success || match.Index != position) {
								throw new DeserializationException($"Malformed double at position {position}");
							}
							position += match.Length - 1;
							break;
						}
					case 's': {
							var match = new Regex(@"s:\d+:"".*?"";").Match(_input, position);
							if (!match.Success) {
								throw new DeserializationException($"Malformed string at position {position}");
							}
							position += match.Length - 1;
							break;
						}
					case 'a': {
							var match = new Regex(@"a:\d+:{").Match(_input, position);
							if (!match.Success || match.Index != position) {
								throw new DeserializationException($"Malformed array at position {position}");
							}
							position += match.Length;
							ValidateFormat(ref position, true);

							break;
						}
					case 'O': {
							var match = new Regex(@"O:\d+:""[a-zA-Z_\\\-0-9]+"":\d+:{").Match(_input, position);
							if (!match.Success || match.Index != position) {
								throw new DeserializationException($"Malformed object at position {position}");
							}
							position += match.Length;
							ValidateFormat(ref position, true);

							break;
					}
					case '}': {
							if (inArray) {
								return true;
							} else {
								throw new DeserializationException($"Unexpected token '{_input[position]}' at position {position}.");
							}
						}
					default: {
							throw new DeserializationException($"Unexpected token '{_input[position]}' at position {position}.");
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
							if (stringEnd != '"') {
								throw new DeserializationException(
									$"String at position {_position} has an incorrect length of {length}: "+
									$"Expected double quote at position {valueStart + length}, found '{stringEnd}' instead."
								);
							}

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

							var typename = _inputBytes.Utf8Substring(_position+2, typeLength, _options.InputEncoding);
							_position += typeLength +2;

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