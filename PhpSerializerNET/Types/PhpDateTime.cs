/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;

namespace PhpSerializerNET;

[PhpClass("DateTime")]
public class PhpDateTime : IPhpObject {
	[PhpProperty("date")]
	public string Date { get; set; }

	[PhpProperty("timezone_type")]
	public int TimezoneType { get; set; }

	[PhpProperty("timezone")]
	public string Timezone { get; set; }

	public string GetClassName() {
		return "DateTime";
	}

	public void SetClassName(string className) {
		throw new InvalidOperationException("Cannot set name on object of type " + nameof(PhpDateTime) + " name is of constant " + this.GetClassName());
	}
}
