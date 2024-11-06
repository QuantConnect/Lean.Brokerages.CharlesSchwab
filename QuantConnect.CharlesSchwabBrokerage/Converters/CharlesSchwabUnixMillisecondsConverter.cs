/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace QuantConnect.Brokerages.CharlesSchwab.Converters;

/// <summary>
/// Converts a <see cref="DateTime"/> to and from Unix epoch time
/// </summary>
public class CharlesSchwabUnixMillisecondsConverter : DateTimeConverterBase
{
    /// <summary>
    /// Represents the Unix epoch time (1970-01-01 00:00:00 UTC) as a reference point 
    /// for conversions between <see cref="DateTime"/> and Unix time.
    /// </summary>
    internal static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Determines whether this converter can handle conversion for a specified type.
    /// </summary>
    /// <param name="objectType">The type to check for conversion compatibility.</param>
    /// <returns>
    /// <c>true</c> if the <paramref name="objectType"/> is either <see cref="DateTime"/> or <see cref="DateTimeOffset"/>;
    /// otherwise, <c>false</c>.
    /// </returns>
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(DateTime) || objectType == typeof(DateTimeOffset);
    }

    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        long milliseconds;

        if (value is DateTime dateTime)
        {
            milliseconds = (long)(dateTime.ToUniversalTime() - UnixEpoch).TotalMilliseconds;
        }
        else if (value is DateTimeOffset dateTimeOffset)
        {
            milliseconds = (long)(dateTimeOffset.ToUniversalTime() - UnixEpoch).TotalMilliseconds;
        }
        else
        {
            throw new JsonSerializationException("Expected date object value.");
        }

        if (milliseconds < 0)
        {
            throw new JsonSerializationException("Cannot convert date value that is before Unix epoch of 00:00:00 UTC on 1 January 1970.");
        }

        writer.WriteValue(milliseconds);
    }

    /// <summary>
    /// Reads the JSON representation of the object.
    /// </summary>
    /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing property value of the JSON that is being converted.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>The object value.</returns>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        bool nullable = Nullable.GetUnderlyingType(objectType) != null;
        if (reader.TokenType == JsonToken.Null)
        {
            if (!nullable)
            {
                throw new JsonSerializationException($"Cannot convert null value to {objectType}.");
            }

            return null;
        }

        long milliseconds;

        if (reader.TokenType == JsonToken.Integer)
        {
            milliseconds = (long)reader.Value;
        }
        else if (reader.TokenType == JsonToken.String)
        {
            if (!long.TryParse((string)reader.Value, out milliseconds))
            {
                throw new JsonSerializationException($"Cannot convert invalid value to {objectType}.");
            }
        }
        else
        {
            throw new JsonSerializationException($"Unexpected token parsing date. Expected Integer or String, got {reader.TokenType}.");
        }

        if (milliseconds >= 0)
        {
            DateTime d = UnixEpoch.AddMilliseconds(milliseconds);

            Type t = (nullable)
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;
            if (t == typeof(DateTimeOffset))
            {
                return new DateTimeOffset(d, TimeSpan.Zero);
            }
            return d;
        }
        else
        {
            throw new JsonSerializationException($"Cannot convert value that is before Unix epoch of 00:00:00 UTC on 1 January 1970 to {objectType}.");
        }
    }
}
