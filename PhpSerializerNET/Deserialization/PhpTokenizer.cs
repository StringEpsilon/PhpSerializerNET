/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;

#nullable enable

namespace PhpSerializerNET;

public ref struct PhpTokenizer {
	private readonly Span<PhpToken> _tokens;
	private readonly ReadOnlySpan<byte> _input;
	private int _position;
	private int _tokenPosition;
	private int _reference = 0;

	private PhpTokenizer(in ReadOnlySpan<byte> input, Span<PhpToken> array) {
		this._input = input;
		this._tokens = array;
		this._position = 0;
		this._tokenPosition = 0;
	}

	private void Advance(int positions) {
		this._position += positions;
	}

	private ValueSpan GetNumbers() {
		int start = this._position;
		while (this._input[++this._position] != (byte)';') { }
		return new ValueSpan(start, this._position - start);
	}

	private int GetLength() {
		int result = 0;
		for (; this._input[this._position] != (byte)':'; this._position++) {
			result = result * 10 + (this._input[this._position] - 48);
		}
		return result;
	}

	private void GetToken(bool countReference) {
		switch (this._input[this._position++]) {
			case (byte)'r':
			case (byte)'R':
				this.GetReferenceToken();
				break;
			case (byte)'b':
				this.GetBooleanToken(countReference);
				break;
			case (byte)'N':
				this._tokens[this._tokenPosition++] = new PhpToken(
					PhpDataType.Null,
					this._position - 1,
					ValueSpan.Empty,
					0
				);
				this._position++;
				break;
			case (byte)'s':
				this.GetStringToken(countReference);
				break;
			case (byte)'i':
				this.GetIntegerToken(countReference);
				break;
			case (byte)'d':
				this.GetFloatingToken(countReference);
				break;
			case (byte)'a':
				this.GetArrayToken(countReference);
				break;
			case (byte)'O':
				this.GetObjectToken(countReference);
				break;
		}
	}

	private void GetBooleanToken(bool reference) {
		this._position++;
		this._tokens[this._tokenPosition++] = new PhpToken(
			PhpDataType.Boolean,
			this._position - 2,
			new ValueSpan(this._position++, 1),
			reference ? ++this._reference : 0
		);
		this._position++;
	}

	private void GetStringToken(bool reference) {
		int position = this._position - 1;
		this._position++;
		int length = this.GetLength();
		this._position += 2;
		this._tokens[this._tokenPosition++] = new PhpToken(
			PhpDataType.String,
			position,
			new ValueSpan(this._position, length),
			reference ? ++this._reference : 0
		);
		this._position += 2 + length;
	}

	private void GetIntegerToken(bool reference) {
		this._position++;
		this._tokens[this._tokenPosition++] = new PhpToken(
			PhpDataType.Integer,
			this._position - 2,
			this.GetNumbers(),
			reference ? ++this._reference : 0
		);
		this._position++;
	}

	private void GetReferenceToken() {
		this._position++;
		int index = this.GetNumbers().GetInt(this._input);
		if (index <= 0 || index > this._reference) {
			throw new DeserializationException($"Invalid reference: '{index}' can not be resolved.");
		}
		this._tokens[this._tokenPosition++] = new PhpToken(PhpDataType.Reference, this._position, ValueSpan.Empty, index);
		this._position++;
	}

	private void GetFloatingToken(bool reference) {
		this._position++;
		this._tokens[this._tokenPosition++] = new PhpToken(
			PhpDataType.Floating,
			this._position - 2,
			this.GetNumbers(),
			reference ? ++this._reference : 0
		);
		this._position++;
	}

	private void GetArrayToken(bool reference) {
		var tokenPosition = this._tokenPosition++;
		int position = this._position - 1;
		int referenceIndex = reference ? ++this._reference : 0;
		this._position++;
		int length = this.GetLength();
		this.Advance(2);
		for (int i = 0; i < length; i++) {
			this.GetToken(false);
			this.GetToken(true);
		}
		this._tokens[tokenPosition] = new PhpToken(
			PhpDataType.Array,
			position,
			ValueSpan.Empty,
			referenceIndex,
			length,
			lastValuePosition: this._tokenPosition -1
		);
		this._position++;
	}

	private void GetObjectToken(bool reference) {
		var tokenPosition = this._tokenPosition++;
		int referenceIndex = reference ? ++this._reference : 0;
		int position = this._position - 1;
		this._position++;
		int classNameLength = this.GetLength();
		this.Advance(2);
		ValueSpan classNameSpan = new ValueSpan(this._position, classNameLength);
		this.Advance(2 + classNameLength);
		int propertyCount = this.GetLength();
		this.Advance(2);
		for (int i = 0; i < propertyCount; i++) {
			this.GetToken(false);
			this.GetToken(true);
		}
		this._tokens[tokenPosition] = new PhpToken(
			PhpDataType.Object,
			position,
			value: classNameSpan,
			reference: referenceIndex,
			propertyCount,
			lastValuePosition: this._tokenPosition -1
		);
		this._position++;
	}

	internal static void Tokenize(ReadOnlySpan<byte> inputBytes, in Span<PhpToken> tokens) {
		new PhpTokenizer(inputBytes, tokens).GetToken(true);
	}
}