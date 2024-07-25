/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable

namespace PhpSerializerNET;

public ref struct PhpTokenizer {
	private readonly Encoding _inputEncoding;
	private readonly ReadOnlySpan<byte> _input;
	private Span<PhpToken> _tokens;
	private int _position;
	private int _tokenPosition;

	private PhpTokenizer(ReadOnlySpan<byte> input, Encoding inputEncoding, Span<PhpToken> array) {
		this._inputEncoding = inputEncoding;
		this._input = input;
		this._tokens = array;
		this._position = 0;
		this._tokenPosition = 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Advance() {
		this._position++;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Advance(int positons) {
		this._position += positons;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private string GetNumbers() {
		int start = this._position;
		while (this._input[this._position] != (byte)';') {
			this._position++;
		}
		return this._inputEncoding.GetString(this._input.Slice(start, this._position - start));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int GetLength() {
		if (this._input[this._position + 1] == ':') {
			return _input[_position++] - 48;
		}
		int start = this._position;
		while (this._input[this._position] != (byte)':') {
			this._position++;
		}
		return int.Parse(this._input.Slice(start, this._position - start), CultureInfo.InvariantCulture);
	}

	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	internal void GetToken() {
		switch (this._input[this._position++]) {
			case (byte)'b':
				this.GetBooleanToken();
				break;
			case (byte)'N':
				this._tokens[this._tokenPosition++] = new PhpToken(PhpDataType.Null, _position - 1);
				this.Advance();
				break;
			case (byte)'s':
				this.GetStringToken();
				break;
			case (byte)'i':
				this.GetIntegerToken();
				break;
			case (byte)'d':
				this.GetFloatingToken();
				break;
			case (byte)'a':
				this.GetArrayToken();
				break;
			case (byte)'O':
				this.GetObjectToken();
				break;
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetNullToken() {
		this._tokens[this._tokenPosition++] = new PhpToken(PhpDataType.Null, _position - 1);
		this.Advance();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetBooleanToken() {
		this.Advance();
		this._tokens[this._tokenPosition++] = new PhpToken(
			PhpDataType.Boolean,
			_position - 2,
			this._input[this._position++] == (byte)'1'
				? "1"
				: "0"
		);
		this.Advance();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetStringToken() {
		int position = _position - 1;
		this.Advance();
		int length = this.GetLength();
		this.Advance(2);
		this._tokens[this._tokenPosition++] = new PhpToken(
			PhpDataType.String,
			position,
			_inputEncoding.GetString(this._input.Slice(this._position, length))
		);
		this.Advance(2 + length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetIntegerToken() {
		this.Advance();
		this._tokens[this._tokenPosition++] = new PhpToken(
			PhpDataType.Integer,
			this._position - 2,
			this.GetNumbers()
		);
		this.Advance();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetFloatingToken() {
		this.Advance();
		this._tokens[this._tokenPosition++] = new PhpToken(
			PhpDataType.Floating,
			this._position - 2,
			this.GetNumbers()
		);
		this.Advance();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetArrayToken() {
		int position = _position - 1;
		this.Advance();
		int length = this.GetLength();
		this._tokens[this._tokenPosition++] = new PhpToken(
			PhpDataType.Array,
			position,
			null,
			length
		);
		this.Advance(2);
		for (int i = 0; i < length * 2; i++) {
			this.GetToken();
		}
		this.Advance();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetObjectToken() {
		int position = _position - 1;
		this.Advance();
		int classNameLength = this.GetLength();
		this.Advance(2);
		string className = _inputEncoding.GetString(this._input.Slice(this._position, classNameLength));
		this.Advance(2 + classNameLength);
		int propertyCount = this.GetLength();
		this._tokens[this._tokenPosition++] = new PhpToken(
			PhpDataType.Object,
			position,
			className,
			propertyCount
		);
		this.Advance(2);
		for (int i = 0; i < propertyCount * 2; i++) {
			this.GetToken();
		}
		this.Advance();
	}

	internal static void Tokenize(ReadOnlySpan<byte> inputBytes, Encoding inputEncoding, Span<PhpToken> tokens) {
		new PhpTokenizer(inputBytes, inputEncoding, tokens).GetToken();
	}
}