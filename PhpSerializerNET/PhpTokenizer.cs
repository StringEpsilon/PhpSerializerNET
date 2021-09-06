/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Collections.Generic;
using System.Text;

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

		internal List<PhpSerializeToken> Tokenize() {
			List<PhpSerializeToken> tokens = new();

			for (; _position < _utf8Input.Length; _position++)
			{
				if ((char)_utf8Input[_position] == '}'){
					return tokens;
				}
				if ( _utf8Input.Length-1 <= _position){
					throw new DeserializationException("Unexpected end of data.",  _input, _position+1);
				}
				switch ((char)_utf8Input[_position] )
				{
					case 'N':
						{
							if ( _utf8Input[_position + 1] == ';')
							{
								tokens.Add(new PhpSerializeToken() { Type = PhpSerializerType.Null });
							}
							else
							{
								throw new DeserializationException(
									$"Expected ';' at position {_position+1}.",  
									_input, 
									_position+1
								);
							}
							_position++;
							break;
						}
					case 'b':
					case 'i':
					case 'd':
						{
							if ( _utf8Input[_position + 1] != ':')
							{
								throw new DeserializationException($"Expected ':' around position {_position}.",
									_input, 
									_position
								);
							}
							var tokenClose = Array.IndexOf(_utf8Input, (byte)';', _position);
							if (tokenClose < 0){
								throw new DeserializationException($"Expected ';' around position {_position}.",
									_input, 
									_position
								);
							}
							var token = new PhpSerializeToken()
							{
								Type = (char)_utf8Input[_position] switch
								{
									'b' => PhpSerializerType.Boolean,
									'i' => PhpSerializerType.Integer,
									'd' => PhpSerializerType.Floating,
									_ => throw new Exception("Unknown token type.")
								},
								Value = _utf8Input.Utf8Substring(_position+2, tokenClose - (_position + 2))
							};
							tokens.Add(token);
							_position = tokenClose;
							break;
						}
					case 's':
						{
							var lengthStart = _position+2;
							if ( _utf8Input[_position + 1] != ':')
							{
								throw new DeserializationException($"Expected ':' at position {_position+1}.",
									_input,
									_position+1
								);
							}
							var lengthClose = Array.IndexOf(_utf8Input, (byte)':', lengthStart + 1);
							if (lengthClose < 0)
							{
								throw new DeserializationException($"Expected ':' around position {lengthStart + 1}.",
									_input,
									lengthStart + 1
								);
							}
							var valueStart = Array.IndexOf(_utf8Input, (byte)'"', lengthClose ) + 1;
							Console.WriteLine(valueStart);
							if (valueStart == 0)
							{
								throw new DeserializationException($"Expected opening '\"' around position {lengthClose + 1}.",
									_input, 
									lengthClose + 1
								);
							}else if (valueStart == _utf8Input.Length-1){
								throw new DeserializationException("Unexpected end of data.",  _input, valueStart);
							}
							var length = int.Parse(
								_utf8Input.Utf8Substring(lengthStart, lengthClose - lengthStart)
							);
 
							// TODO: Check for closing '"' and the token terminating ';'
							if (_utf8Input[valueStart + length] != '"')
							{								
								throw new DeserializationException($"Expected closing '\"' at position {valueStart + length}.",
									_input, 
									valueStart + length
								);
							}

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
							if (lengthStart <0)
							{
								throw new DeserializationException(
									$"(Array) Expected ':' around position {_position + 1}",
									_input, 
									_position
								);
							}
							var lengthClose = Array.IndexOf(_utf8Input,(byte)':', lengthStart + 1);
							if (lengthClose < 0)
							{
								throw new DeserializationException(
									$"(Array) Expected ':' after position {lengthStart + 1}",
									_input, 
									_position
								);
							}
							var length = int.Parse(	
								_utf8Input.Utf8Substring(lengthStart, lengthClose - lengthStart)
							);

							_position = Array.IndexOf(_utf8Input, (byte)'{', lengthStart + 1) + 1;
							tokens.Add(new()
							{
								Type = PhpSerializerType.Array,
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