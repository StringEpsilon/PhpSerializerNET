/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Text;

namespace PhpSerializerNET
{
	/// <summary>
	/// Available behaviors for dealing with associative arrays and their conversion to Lists.
	/// </summary>
	public enum ListOptions
	{
		/// <summary>
		/// Convert associative array to list when all keys are consecutive integers
		/// </summary>
		Default,

		/// <summary>
		/// Convert associative array to list when all keys are integers, consecutiveh or not.
		/// </summary>
		OnAllIntegerKeys,

		/// <summary>
		/// Always use dictionaries.
		/// </summary>
		Never
	}

	public enum StdClassOption {
		Dictionary,
		Dynamic
	}

	/// <summary>
	/// Options for deserializing PHP data.
	/// </summary>
	public class PhpDeserializationOptions
	{
		/// <summary>
		/// Whether or not properties are matched case sensitive. Default true.
		/// </summary>
		public bool CaseSensitiveProperties { get; set; } = true;

		/// <summary>
		/// If true, keys present in the array but not on the target class will be ignored.
		/// Otherwise an exception will be thrown.
		/// Default is false.
		/// </summary>
		public bool AllowExcessKeys { get; set; } = false;

		/// <summary>
		/// Determines how and when associative arrays are deserialized into <see cref="System.Collections.Generic.List{object}"/>
		/// instead of a dictionary. Defaults to <see cref="ListOptions.Default"/>.
		/// </summary>
		public ListOptions UseLists { get; set; } = ListOptions.Default;

		/// <summary>
		/// Whether or not to convert strings "1"` and "0" to boolean. Default is false.
		/// </summary>
		public bool NumberStringToBool { get; set; } = false;

		/// <summary>
		/// Encoding of the input. Default is UTF-8. Encoding can make a difference in string lenghts and selecting the wrong 
		/// encoding for a given input can cause the deserialization to fail.
		/// </summary>
		public Encoding InputEncoding { get; set; } = Encoding.UTF8;

		/// <summary>
		/// Target datatype for objects of type "stdClass". Default: Dictionary<string, object>.
		/// </summary>
		public StdClassOption StdClass {get;set;} = StdClassOption.Dictionary;

		internal static PhpDeserializationOptions DefaultOptions = new()
		{
			CaseSensitiveProperties = true,
			AllowExcessKeys = false,
			UseLists = ListOptions.Default,
			InputEncoding = Encoding.UTF8,
		};
	}
}