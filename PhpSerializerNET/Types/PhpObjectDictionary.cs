/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;

namespace PhpSerializerNET;

/// <summary>
/// Represents a php object as a <see cref="Dictionary{string,object}"/> where TKey = <see cref="string"/> and TValue = <see cref="object"/>.
/// </summary>
public class PhpObjectDictionary : Dictionary<string, object>, IPhpObject {
	private string _className;

	public string GetClassName() {
		return this._className;
	}

	public void SetClassName(string className) {
		this._className = className;
	}
}
