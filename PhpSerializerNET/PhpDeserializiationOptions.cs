/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

namespace PhpSerializerNET
{
	public enum ListOptions {
		/// <summary>
		/// Convert associative array to list when all keys are consequetive integers
		/// </summary>
		Default,
		/// <summary>
		/// Convert associative array to list when all keys are integers, consequetive or not.
		/// </summary>
		OnAllIntegerKeys,
		/// <summary>
		/// Always use dictionaries.
		/// </summary>
		Never
	}

	public class PhpDeserializationOptions
	{
		public bool CaseSensitiveProperties = true;
		public bool AllowExcessKeys = false;
		public ListOptions UseLists = ListOptions.Default;

		internal static PhpDeserializationOptions DefaultOptions = new(){
			CaseSensitiveProperties = true,
			AllowExcessKeys = false,
			UseLists = ListOptions.Default,
		};
	}
}