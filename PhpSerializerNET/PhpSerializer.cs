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
using System.Text;

namespace PhpSerializerNET
{
	public static class PhpSerializer
	{

		/// <summary>
		/// Deserialize the given string into an object.
		/// </summary>
		/// <param name="input">
		/// PHP serialization data.
		/// </param>
		/// <returns>
		/// The deserialized object of type:
		///
		/// object (null),
		/// <see cref="bool" />,
		/// <see cref="long" />,
		/// <see cref="double" />,
		/// <see cref="string" />,
		/// <see cref="List{object}"/>, or
		/// <see cref="Dictionary{object,object}"/>
		/// </returns>
		public static object Deserialize(string input)
		{
			var tokens = Tokenize(input);
			return tokens[0].ToObject();
		}

		/// <summary>
		/// Try to deserialize the input string into a specific type.
		/// </summary>
		/// <param name="input">
		/// PHP serialization data.
		/// </param>
		/// <typeparam name="T">
		/// The desired output type.
		/// This should be one of the primitives or a class with a public parameterless constructor.
		/// </typeparam>
		/// <returns>
		/// The deserialized object.
		/// </returns>
		public static T Deserialize<T>(string input)
		{
			var tokens = Tokenize(input);
			if (tokens.Count > 1)
			{
				throw new Exception("Can not deserialize loose collection of values into object");
			}
			if (tokens[0].Type == PhpSerializerType.Null)
			{
				return default(T);
			}
			if (tokens[0].Type == PhpSerializerType.Array)
			{

				var collection = tokens[0].ToObject();
				if (collection is IDictionary)
				{
					return (T)ConstructObject(typeof(T), (Dictionary<object, object>)collection);
				}
				else
				{
					return (T)ConstructList(typeof(T), (List<object>)collection);
				}
			}
			else
			{
				if (typeof(T).IsIConvertible())
				{
					var value = tokens[0].ToObject();
					if (value is IConvertible convertible)
					{
						return (T)convertible.ToType(typeof(T), CultureInfo.InvariantCulture);
					}
					return (T)value;
				}
			}
			throw new Exception($"Can not deserialize {Enum.GetName(tokens[0].Type)} into {typeof(T).Name}");
		}

		/// <summary>
		/// Serialize an object into the PHP format.
		/// </summary>
		/// <param name="input">
		/// Object to serialize.
		/// </param>
		/// <param name="seen">
		/// For internal use only. This might be removed in a later release.
		/// (It's to avoid circulare references causing a stackoverflow)
		/// </param>
		/// <returns>
		/// String representation of the input object.
		/// Note that circular references are terminated with "N;"
		/// and that all classes are serialized as associative arrays.
		/// </returns>
		public static string Serialize(object input, List<Object> seen = null)
		{
			StringBuilder output = new StringBuilder();
			if (seen == null)
			{
				seen = new List<object>();
			}

			switch (input)
			{
				case long longValue:
					{
						return $"i:{longValue.ToString()};";
					}
				case int integerValue:
					{
						return $"i:{integerValue.ToString()};";
					}
				case double floatValue:
					{
						if (double.IsPositiveInfinity(floatValue))
						{
							return $"d:INF;";
						}
						if (double.IsNegativeInfinity(floatValue))
						{
							return $"d:-INF;";
						}
						if (double.IsNaN(floatValue))
						{
							return $"d:NAN;";
						}
						return $"d:{floatValue.ToString(CultureInfo.InvariantCulture)};";
					}
				case string stringValue:
					{
						// Use the UTF8 byte count, because that's what the PHP implementation does:
						return $"s:{ASCIIEncoding.UTF8.GetByteCount(stringValue)}:\"{stringValue}\";";
					}
				case bool boolValue:
					{
						return boolValue ? "b:1;" : "b:0;";
					}
				case null:
					{
						return "N;";
					}
				case IDictionary dictionary:
					{
						if (seen.Contains(input))
						{
							// Terminate circular reference:
							// It might be better to make this optional. The PHP implementation seems to
							// throw an exception, from what I can tell
							return "N;";
						}
						seen.Add(input);
						output.Append($"a:{dictionary.Count}:");
						output.Append("{");
						foreach (DictionaryEntry entry in dictionary)
						{
							if (entry.Key is not string && entry.Key is not int)
							{
								throw new System.Exception($"Can not serialize associative array with key type {entry.Key.GetType().FullName}");
							}
							output.Append($"{Serialize(entry.Key)}{Serialize(entry.Value, seen)}");
						}
						output.Append("}");
						return output.ToString();
					}
				case IList collection:
					{
						if (seen.Contains(input))
						{
							return "N;"; // See above.
						}
						seen.Add(input);
						output.Append($"a:{collection.Count}:");
						output.Append("{");
						for (int i = 0; i < collection.Count; i++)
						{
							output.Append($"{Serialize(i, seen)}{Serialize(collection[i], seen)}");
						}
						output.Append("}");
						return output.ToString();
					}
				default:
					{
						if (seen.Contains(input))
						{
							return "N;"; // See above.
						}

						seen.Add(input);
						var properties = input.GetType().GetProperties().Where(x => x.CanRead);
						output.Append($"a:{properties.Count()}:");
						output.Append("{");
						foreach (var property in properties)
						{
							output.Append($"{Serialize(property.Name)}{Serialize(property.GetValue(input), seen)}");
						}
						output.Append("}");
						return output.ToString();
					}
			}

		}

		private static object ConstructObject(Type targetType, Dictionary<object, object> sourceDictionary)
		{
			object result = targetType.GetConstructor(new Type[0]).Invoke(null);
			var targetProperties = targetType.GetProperties();

			if (result is IDictionary resultDictionary)
			{
				return ConstructDictionary(targetType, sourceDictionary);
			}
			foreach (var entry in sourceDictionary)
			{
				if (entry.Key is string propertyName)
				{
					// TODO: This really should be an option. Plus it might create conflicts.
					var property = targetProperties.FirstOrDefault(y => y.Name.ToLower() == propertyName.ToLower());
					if (property != null)
					{
						if (property.PropertyType == entry.Value.GetType())
						{
							property.SetValue(result, entry.Value);
						}
						else if (property.PropertyType.IsIConvertible())
						{
							if (entry.Value is IConvertible convertible)
							{
								property.SetValue(result, convertible.ToType(property.PropertyType, CultureInfo.InvariantCulture));
							}
							else
							{
								throw new Exception(
									$"Can not assign '{entry.Value.ToString()}' to property '{property.Name}' of type '{property.PropertyType.Name}'"
								);
							}
						}
						else
						{
							if (property.PropertyType.IsClass)
							{
								// TODO: handle lists, handle dictionaries.

								if (entry.Value is Dictionary<object, object>)
								{
									property.SetValue(
										result,
										ConstructObject(
											property.PropertyType,
											entry.Value as Dictionary<object, object>
										)
									);
								}
							}
						}
					}
				}
			}
			return result;
		}

		private static object ConstructList(Type targetType, List<object> sourceList)
		{
			IList result = (IList)targetType.GetConstructor(new Type[0]).Invoke(null);

			Type itemType = typeof(object);
			if (targetType.GenericTypeArguments.Length >= 1)
			{
				itemType = targetType.GenericTypeArguments[0];
			}

			foreach (var item in sourceList)
			{
				result.Add(itemType.Convert(item, true));
			}
			return result;
		}

		private static object ConstructDictionary(Type targetType, Dictionary<object, object> dictionary)
		{
			IDictionary resultDictionary = (IDictionary)targetType.GetConstructor(new Type[0]).Invoke(null);
			if (targetType.GenericTypeArguments.Count() == 2)
			{
				var keyType = targetType.GenericTypeArguments[0];
				var valueType = targetType.GenericTypeArguments[1];
				foreach (var entry in dictionary)
				{
					object key = keyType.Convert(entry.Key, true);
					object value = valueType.Convert(entry.Value, true);
					resultDictionary.Add(key, value);
				}
				return resultDictionary;
			}

			foreach (var entry in dictionary)
			{
				resultDictionary.Add(entry.Key, entry.Value);
			}
			return resultDictionary;
		}

		private static List<PhpSerializeToken> Tokenize(string input)
		{
			List<PhpSerializeToken> tokens = new();

			for (int i = 0; i < input.Length; i++)
			{
				switch (input[i])
				{
					case 'N':
						{
							if (input[i + 1] == ';')
							{
								tokens.Add(new PhpSerializeToken() { Type = PhpSerializerType.Null });
							}
							else
							{
								throw new Exception($"Expected ';' around position {i} in '{input}'");
							}
							i++;
							break;
						}
					case 'b':
					case 'i':
					case 'd':
						{
							var valueEnd = input.IndexOf(':', i + 1);
							if (valueEnd < 0)
							{
								throw new Exception($"{input[i]} Expected ':' around position {i} in '{input}'");
							}
							var tokenClose = input.IndexOf(';', i);
							var token = new PhpSerializeToken()
							{
								Type = input[i] switch
								{
									'b' => PhpSerializerType.Boolean,
									'i' => PhpSerializerType.Integer,
									'd' => PhpSerializerType.Floating,
									_ => throw new Exception("Unknown token type.")
								},
								Value = input.Substring(i + 2, tokenClose - (i + 2))
							};
							i = tokenClose;
							tokens.Add(token);
							break;
						}
					case 's':
						{
							// TODO: This is technically too lenient, as the IndexOf() might skip over forbidden and out of place characters.
							var lengthStart = input.IndexOf(':', i) + 1;
							if (lengthStart < 0)
							{
								throw new Exception($"(String) Expected ':' around position {i + 1} in '{input}'");
							}
							var lengthClose = input.IndexOf(':', lengthStart + 1);
							if (lengthClose < 0)
							{
								throw new Exception($"(String) Expected ':' after position {lengthStart + 1} in '{input}'");
							}
							var valueStart = input.IndexOf('"', lengthClose + 1) + 1;
							if (valueStart < 0)
							{
								throw new Exception($"(String) Expected '\"' around position {lengthClose + 1} in '{input}'");
							}
							var length = int.Parse(input.Substring(lengthStart, lengthClose - lengthStart));

							// TODO: Check for closing '"' and the token terminating ';'
							if (input[valueStart + length] != '"')
							{
								throw new Exception($"(String) Expected '\"' around position {valueStart + length} in '{input}'");
							}

							tokens.Add(new PhpSerializeToken()
							{
								Type = PhpSerializerType.String,
								Length = length,
								// +2 for the : and the "
								Value = input.Substring(valueStart, length)
							});
							i = valueStart + length;
							break;
						}
					case 'a':
						{
							// TODO: This is technically too lenient, as the IndexOf() might skip over forbidden and out of place characters.
							var lengthStart = input.IndexOf(':', i) + 1;
							if (lengthStart < 0)
							{
								throw new Exception($"(Array) Expected ':' around position {i + 1} in '{input}'");
							}
							var lengthClose = input.IndexOf(':', lengthStart + 1);
							if (lengthClose < 0)
							{
								throw new Exception($"(Array) Expected ':' after position {lengthStart + 1} in '{input}'");
							}
							var length = int.Parse(input.Substring(lengthStart, lengthClose - lengthStart));

							var arrayStart = input.IndexOf('{', lengthStart + 1) + 1;
							var arrayEnd = arrayStart;

							// TODO: more robust end-finding:
							arrayEnd = input.IndexOf(";}", arrayStart) + 1;
							Console.WriteLine(input.Substring(arrayStart, arrayEnd - arrayStart));
							tokens.Add(new()
							{
								Type = PhpSerializerType.Array,
								Children = Tokenize(input.Substring(arrayStart, arrayEnd - arrayStart))
							});
							i = arrayEnd + 2;
							break;
						}

				}
			}
			return tokens;
		}
	}
}
