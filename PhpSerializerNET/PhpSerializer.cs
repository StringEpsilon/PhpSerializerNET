/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PhpSerializerNET {
	internal class PhpSerializer {
		private PhpSerializiationOptions _options;
		private List<object> _seenObjects;

		public PhpSerializer(PhpSerializiationOptions options = null) {
			_options = options ?? PhpSerializiationOptions.DefaultOptions;
			
			_seenObjects = new();
		}

		public string Serialize(object input) {
			StringBuilder output = new StringBuilder();
			switch (input) {
				case long longValue: {
						return $"i:{longValue.ToString()};";
					}
				case int integerValue: {
						return $"i:{integerValue.ToString()};";
					}
				case double floatValue: {
						if (double.IsPositiveInfinity(floatValue)) {
							return $"d:INF;";
						}
						if (double.IsNegativeInfinity(floatValue)) {
							return $"d:-INF;";
						}
						if (double.IsNaN(floatValue)) {
							return $"d:NAN;";
						}
						return $"d:{floatValue.ToString(CultureInfo.InvariantCulture)};";
					}
				case string stringValue: {
						// Use the UTF8 byte count, because that's what the PHP implementation does:
						return $"s:{ASCIIEncoding.UTF8.GetByteCount(stringValue)}:\"{stringValue}\";";
					}
				case bool boolValue: {
						return boolValue ? "b:1;" : "b:0;";
					}
				case null: {
						return "N;";
					}
				// TODO: There's enough shared code here to warrant refactoring.
				// Probably doing this in a default and then have another method for serializing the 3 options.
				case IDictionary dictionary: { 
						if (_seenObjects.Contains(input)) {
							if (_options.ThrowOnCircularReferences){
								throw new ArgumentException("Input object has a circular reference.");
							}
							return "N;";
						}
						if (dictionary.GetType().GenericTypeArguments.Count() > 0) {
							var keyType = dictionary.GetType().GenericTypeArguments[0];
							if (!keyType.IsIConvertible() && keyType != typeof(object)) {
								throw new Exception($"Can not serialize into associative array with key type {keyType.FullName}");
							}
						}
						_seenObjects.Add(input);
						output.Append($"a:{dictionary.Count}:");
						output.Append("{");


						foreach (DictionaryEntry entry in dictionary) {

							output.Append($"{this.Serialize(entry.Key)}{Serialize(entry.Value)}");
						}
						output.Append("}");
						return output.ToString();
					}
				case IList collection: {
						if (_seenObjects.Contains(input)) {
							if (_options.ThrowOnCircularReferences){
								throw new ArgumentException("Input object has a circular reference.");
							}
							return "N;";
						}
						_seenObjects.Add(input);
						output.Append($"a:{collection.Count}:");
						output.Append("{");
						for (int i = 0; i < collection.Count; i++) {
							output.Append(Serialize(i));
							output.Append(Serialize(collection[i]));
						}
						output.Append("}");
						return output.ToString();
					}
				default: {
						if (_seenObjects.Contains(input)) {
							if (_options.ThrowOnCircularReferences){
								throw new ArgumentException("Input object has a circular reference.");
							}
							return "N;"; // See above.
						}
						_seenObjects.Add(input);
						var inputType = input.GetType();
						
						if (inputType.GetCustomAttribute<PhpClass>() != null) // TODO: add option.
						{
							return this.SerializeToObject(input);
						}

						IEnumerable<MemberInfo> members = inputType.IsValueType
							? inputType.GetFields().Where(y => y.IsPublic && y.GetCustomAttribute<PhpIgnoreAttribute>() == null)
							: inputType.GetProperties().Where(y => y.CanRead && y.GetCustomAttribute<PhpIgnoreAttribute>() == null);

						output.Append($"a:{members.Count()}:");
						output.Append("{");
						foreach (var member in members) {
							output.Append(this.SerializeMember(member, input));
						}
						output.Append("}");
						return output.ToString();
					}
			}
		}

		private string SerializeToObject(object input) {
			var className = input.GetType().GetCustomAttribute<PhpClass>().Name;
			if (string.IsNullOrEmpty(className)) {
				className = "stdClass";
			}
			StringBuilder output = new StringBuilder();
			var properties = input.GetType().GetProperties().Where(y => y.CanRead && y.GetCustomAttribute<PhpIgnoreAttribute>() == null);

			output.Append("O:");
			output.Append(className.Length);
			output.Append(":\"");
			output.Append(className);
			output.Append("\":");
			output.Append(properties.Count());
			output.Append(":{");
			foreach (PropertyInfo property in properties) {
				output.Append(this.SerializeMember(property, input));
			}
			output.Append("}");
			return output.ToString();
		}

		private string SerializeMember(MemberInfo member, object input) {
			var propertyName = member.GetCustomAttribute<PhpPropertyAttribute>() != null
				? member.GetCustomAttribute<PhpPropertyAttribute>().Name
				: member.Name;
			return $"{this.Serialize(propertyName)}{this.Serialize(member.GetValue(input))}";
		}
	}
}