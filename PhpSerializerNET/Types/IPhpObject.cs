/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

namespace PhpSerializerNET;

[PhpClass()]
public interface IPhpObject {
	/// <summary>
	/// Get the class name that was specified in the serialization data of the object.
	/// </summary>
	/// <returns>
	/// The class name.
	/// </returns>
	public string GetClassName();

	/// <summary>
	/// Set the class name of the object for serialization.
	/// </summary>
	/// <param name="className">The class name.</param>
	public void SetClassName(string className);
}
