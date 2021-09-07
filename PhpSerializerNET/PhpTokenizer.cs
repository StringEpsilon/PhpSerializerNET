/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PhpSerializerNET
{
	public class PhpTokenizer
	{
		private string _input;
		private byte[] _utf8Input;
		private int _position;
		private PhpDeserializationOptions _options;

		public PhpTokenizer(string input, PhpDeserializationOptions options)
		{
			this._input = input;
			this._position = 0;
			this._options = options;
			this._utf8Input = Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(input));
		}

		internal bool ValidateFormat(ref int position, bool inArray = false)
		{
			for (; position < _input.Length; position++)
			{
				switch (_input[position])
				{
					case 'N':
						{
							var match = new Regex(@"N;").Match(_input, position);
							if (!match.Success || match.Index != position)
							{
								throw new DeserializationException($"Malformed null at position {position}");
							}
							position += match.Length - 1;
							break;
						}
					case 'b':
						{
							var match = new Regex(@"b:[10];").Match(_input, position);
							if (!match.Success || match.Index != position)
							{
								throw new DeserializationException($"Malformed boolean at position {position}");
							}
							position += match.Length - 1;
							break;
						}
					case 'i':
						{
							var match = new Regex(@"i:[+-]?\d+;").Match(_input, position);
							if (!match.Success || match.Index != position)
							{
								throw new DeserializationException($"Malformed integer at position {position}");
							}
							position += match.Length - 1;
							break;
						}
					case 'd':
						{
							// Validate the correctness of the actual value in the proper parsing step:
							var match = new Regex(@"d:.+;").Match(_input, position);
							if (!match.Success || match.Index != position)
							{
								throw new DeserializationException($"Malformed double at position {position}");
							}
							position += match.Length - 1;
							break;
						}
					case 's':
						{
							var match = new Regex(@"s:\d+:"".*?"";").Match(_input, position);
							if (!match.Success)
							{
								throw new DeserializationException($"Malformed string at position {position}");
							}
							position += match.Length - 1;
							break;
						}
					case 'a':
						{
							var match = new Regex(@"a:\d+:{").Match(_input, position);
							if (!match.Success || match.Index != position)
							{
								throw new DeserializationException($"Malformed array at position {position}");
							}
							position += match.Length;
							ValidateFormat(ref position, true);

							break;
						}
					case '}':
						{
							if (inArray)
							{
								return true;
							}
							else
							{
								throw new DeserializationException($"Unexpected token '{_input[position]}' at position {position}.");
							}
						}
					default:
						{
							throw new DeserializationException($"Unexpected token '{_input[position]}' at position {position}.");
						}
				}
			}
			return true;
		}

		internal List<PhpSerializeToken> Tokenize()
		{
			int position = 0;
			this.ValidateFormat(ref position);
			List<PhpSerializeToken> tokens = new();

			for (; _position < _utf8Input.Length; _position++)
			{
				if ((char)_utf8Input[_position] == '}')
				{
					return tokens;
				}
				if (_utf8Input.Length - 1 <= _position)
				{
					throw new DeserializationException($"Unexpected end of data at position { _position + 1}");
				}
				switch ((char)_utf8Input[_position])
				{
					case 'N':
						{
							tokens.Add(new PhpSerializeToken() { Type = PhpSerializerType.Null });
							_position++;
							break;
						}
					case 'b':
					case 'i':
					case 'd':
						{
							var tokenClose = Array.IndexOf(_utf8Input, (byte)';', _position + 1);
							var token = new PhpSerializeToken()
							{
								Type = (char)_utf8Input[_position] switch
								{
									'b' => PhpSerializerType.Boolean,
									'i' => PhpSerializerType.Integer,
									'd' => PhpSerializerType.Floating,
									_ => throw new Exception("This branch should be impossible to hit.")
								},
								Value = _utf8Input.Utf8Substring(_position + 2, tokenClose - (_position + 2))
							};
							tokens.Add(token);
							_position = tokenClose;
							break;
						}
					case 's':
						{
							var lengthStart = _position + 2;
							var lengthClose = Array.IndexOf(_utf8Input, (byte)':', lengthStart + 1);
							var valueStart = Array.IndexOf(_utf8Input, (byte)'"', lengthClose) + 1;

							var length = int.Parse(
								_utf8Input.Utf8Substring(lengthStart, lengthClose - lengthStart)
							);

							tokens.Add(new PhpSerializeToken()
							{
								Type = PhpSerializerType.String,
								Length = length,
								Value = _utf8Input.Utf8Substring(valueStart, length)
							});
							_position = valueStart + length + 1;
							break;
						}
					case 'a':
						{
							var lengthStart = _position + 2;
							var lengthClose = Array.IndexOf(_utf8Input, (byte)':', lengthStart + 1);
							var length = int.Parse(
								_utf8Input.Utf8Substring(lengthStart, lengthClose - lengthStart)
							);

							_position = Array.IndexOf(_utf8Input, (byte)'{', lengthStart + 1) + 1;
							tokens.Add(new()
							{
								Type = PhpSerializerType.Array,
								Position = _position,
								Length = length,
								Children = this.Tokenize()
							});
							_position++;
							_position++;
							break;
						}
				}
			}
			return tokens;
		}
	}
}