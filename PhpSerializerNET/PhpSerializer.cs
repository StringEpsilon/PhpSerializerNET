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
		public static object Deserialize(string input, PhpDeserializationOptions options = null)
		{
			if (options == null)
			{
				options = PhpDeserializationOptions.DefaultOptions;
			}
			var tokens = new PhpTokenizer(input, options).Tokenize();
			if (tokens.Count > 1) // TODO: maybe have an option to return a List<> in this case.
			{
				throw new DeserializationException("Can not deserialize loose collection of values into object");
			}

			return tokens[0].ToObject(options);
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
		public static T Deserialize<T>(
			string input,
			PhpDeserializationOptions options = null
		)
		{
			if (options == null)
			{
				options = PhpDeserializationOptions.DefaultOptions;
			}
			var tokens = new PhpTokenizer(input, options).Tokenize();

			if (tokens.Count > 1) // TODO: maybe have an option to return a List<> in this case.
			{
				throw new DeserializationException("Can not deserialize loose collection of values into object");
			}

			if (tokens[0].Type == PhpSerializerType.Null)
			{
				return default(T);
			}

			if (tokens[0].Type == PhpSerializerType.Array)
			{
				var collection = tokens[0].ToObject(options);
				if (collection is IDictionary)
				{
					return (T)ConstructObject(typeof(T), (Dictionary<object, object>)collection, options);
				}
				else
				{
					return (T)ConstructList(typeof(T), (List<object>)collection, options);
				}
			}
			else
			{
				if (typeof(T).IsIConvertible())
				{
					var value = tokens[0].ToObject(options);
					if (value is IConvertible convertible)
					{
						return (T)convertible.ToType(typeof(T), CultureInfo.InvariantCulture);
					}
					return (T)value;
				}
			}
			throw new DeserializationException($"Can not deserialize {Enum.GetName(tokens[0].Type)} into {typeof(T).Name}");
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
		public static string Serialize(object input)
		{
			var seenObjects = new List<object>();
			return Serialize(input, seenObjects);
		}

		private static string Serialize(object input, List<Object> seenObjects)
		{
			StringBuilder output = new StringBuilder();
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
						if (seenObjects.Contains(input))
						{
							// Terminate circular reference:
							// It might be better to make this optional. The PHP implementation seems to
							// throw an exception, from what I can tell
							return "N;";
						}
						seenObjects.Add(input);
						output.Append($"a:{dictionary.Count}:");
						output.Append("{");
						foreach (DictionaryEntry entry in dictionary)
						{
							if (entry.Key is not string && entry.Key is not int)
							{
								throw new Exception($"Can not serialize associative array with key type {entry.Key.GetType().FullName}");
							}
							output.Append($"{Serialize(entry.Key)}{Serialize(entry.Value, seenObjects)}");
						}
						output.Append("}");
						return output.ToString();
					}
				case IList collection:
					{
						if (seenObjects.Contains(input))
						{
							return "N;"; // See above.
						}
						seenObjects.Add(input);
						output.Append($"a:{collection.Count}:");
						output.Append("{");
						for (int i = 0; i < collection.Count; i++)
						{
							output.Append($"{Serialize(i, seenObjects)}{Serialize(collection[i], seenObjects)}");
						}
						output.Append("}");
						return output.ToString();
					}
				default:
					{
						if (seenObjects.Contains(input))
						{
							return "N;"; // See above.
						}

						seenObjects.Add(input);
						var properties = input.GetType().GetProperties().Where(y => y.CanRead && y.GetCustomAttribute<PhpIgnoreAttribute>() == null);
						output.Append($"a:{properties.Count()}:");
						output.Append("{");
						foreach (var property in properties)
						{
							var propertyName = property.GetCustomAttribute<PhpPropertyAttribute>() != null
								? property.GetCustomAttribute<PhpPropertyAttribute>().Name
								: property.Name;
							output.Append($"{Serialize(propertyName)}{Serialize(property.GetValue(input), seenObjects)}");
						}
						output.Append("}");
						return output.ToString();
					}
			}

		}

		private static object ConstructObject(Type targetType, Dictionary<object, object> sourceDictionary, PhpDeserializationOptions options)
		{
			object result = targetType.GetConstructor(new Type[0]).Invoke(null);
			var targetProperties = targetType.GetProperties();

			if (result is IDictionary resultDictionary)
			{
				return ConstructDictionary(targetType, sourceDictionary, options);
			}
			if (result is IList)
			{
				return ConstructList(
					targetType,
					sourceDictionary.Values.ToList(),
					options
				);
			}
			foreach (var entry in sourceDictionary)
			{
				if (entry.Value != null)
				{
					string propertyName = entry.Key is string
						? entry.Key as string
						: entry.Key.ToString();

					var property = targetProperties.FindProperty(propertyName, options);

					if (property == null)
					{
						if (!options.AllowExcessKeys)
						{
							throw new DeserializationException(
								$"Error: Could not bind the key {propertyName} to object of type {targetType.Name}: No such property."
							);
						}
						break;
					}

					if (property.GetCustomAttribute<PhpIgnoreAttribute>() != null)
					{
						break;
					}
					if (property.PropertyType == entry.Value.GetType())
					{
						property.SetValue(result, entry.Value);
					}
					else if (property.PropertyType == typeof(bool) && entry.Value is string value && (value == "1" || value == "0"))
					{
						if (options.NumberStringToBool)
						{
							property.SetValue(result, (string)value == "1" ? true : false);
						}
						else
						{
							throw new DeserializationException(
								$"Can not assign '{entry.Value.ToString()}' to property '{property.Name}' of type '{property.PropertyType.Name}'"
							);
						}
					}
					else if (property.PropertyType.IsIConvertible())
					{
						if (entry.Value is IConvertible convertible)
						{
							property.SetValue(result, convertible.ToType(property.PropertyType, CultureInfo.InvariantCulture));
						}
						else
						{
							throw new DeserializationException(
								$"Can not assign '{entry.Value.ToString()}' to property '{property.Name}' of type '{property.PropertyType.Name}'"
							);
						}
					}
					else if (property.PropertyType.IsClass)
					{
						if (entry.Value is Dictionary<object, object>)
						{
							property.SetValue(
								result,
								ConstructObject(
									property.PropertyType,
									entry.Value as Dictionary<object, object>,
									options
								)
							);
						}
					}
				}
			}
			return result;
		}

		private static object ConstructList(Type targetType, IList sourceList, PhpDeserializationOptions options)
		{
			IList result = (IList)targetType.GetConstructor(new Type[0]).Invoke(null);

			Type itemType = typeof(object);
			if (targetType.GenericTypeArguments.Length >= 1)
			{
				itemType = targetType.GenericTypeArguments[0];
			}

			foreach (var item in sourceList)
			{
				if (item is Dictionary<object, object> dictionary)
				{
					result.Add(ConstructObject(itemType, dictionary, options));
				}
				else
				{
					result.Add(itemType.Convert(item, true));

				}
			}
			return result;
		}

		private static object ConstructDictionary(
			Type targetType,
			Dictionary<object, object> dictionary,
			PhpDeserializationOptions options
		)
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
					if (entry.Value is Dictionary<object, object> entryDictionary)
					{
						resultDictionary.Add(key, ConstructObject(valueType, entryDictionary, options));
					}
					else
					{
						resultDictionary.Add(key, value);
					}

				}
				return resultDictionary;
			}

			foreach (var entry in dictionary)
			{
				resultDictionary.Add(entry.Key, entry.Value);
			}
			return resultDictionary;
		}
	}
}
