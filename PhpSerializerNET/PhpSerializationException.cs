/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Runtime.Serialization;

namespace PhpSerializerNET
{
	public class DeserializationException : Exception
	{
		public string Input {get; private set;}
		public long Position {get; private set;}

		public DeserializationException()
		{
		}

		public DeserializationException(string message) : base(message)
		{
		}

		public DeserializationException(string message, string input, long position) : base(message)
		{
			this.Input = input;
			this.Position = position;
		}

		public DeserializationException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected DeserializationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}