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
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using QuantConnect.Brokerages.CharlesSchwab.Models.Stream;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums.Stream;
using StreamDataResponse = QuantConnect.Brokerages.CharlesSchwab.Models.Stream.Data;

namespace QuantConnect.Brokerages.CharlesSchwab.Converters;

public class StreamDataConverter : JsonConverter<StreamDataResponse>
{
    /// <summary>
    /// Gets a value indicating whether this <see cref="JsonConverter"/> can write JSON.
    /// </summary>
    /// <value><c>true</c> if this <see cref="JsonConverter"/> can write JSON; otherwise, <c>false</c>.</value>
    public override bool CanWrite => false;

    /// <summary>
    /// Gets a value indicating whether this <see cref="JsonConverter"/> can read JSON.
    /// </summary>
    /// <value><c>true</c> if this <see cref="JsonConverter"/> can read JSON; otherwise, <c>false</c>.</value>
    public override bool CanRead => true;

    /// <summary>
    /// Reads the JSON representation of the object.
    /// </summary>
    /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing property value of the JSON that is being converted.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>The object value.</returns>
    public override StreamDataResponse ReadJson(JsonReader reader, Type objectType, StreamDataResponse existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var jToken = JToken.Load(reader);

        var service = jToken["service"].ToObject<Service>();

        switch (service)
        {
            case Service.Account:
                return CreateStreamDataResponse<AccountContent>(service, jToken);
            case Service.LevelOneEquities:
                return CreateStreamDataResponse<LevelOneEquityContent>(service, jToken);
            case Service.LevelOneOptions:
                return CreateStreamDataResponse<LevelOneOptionContent>(service, jToken);
            default:
                throw new NotImplementedException($"{nameof(StreamDataConverter)}.{nameof(ReadJson)}: The service '{service}' is not supported or implemented.");
        }
    }

    /// <summary>
    /// Creates a <see cref="StreamDataResponse"/> for the specified service by deserializing the provided JSON token into a collection of content objects.
    /// </summary>
    /// <typeparam name="TContent">The type of content to deserialize, which must inherit from <see cref="BaseContent"/>.</typeparam>
    /// <param name="service">The service type associated with the response.</param>
    /// <param name="jToken">The JSON token containing the data for the response, including the command and content.</param>
    /// <returns>
    /// A <see cref="StreamDataResponse"/> containing the deserialized content and metadata such as timestamp and command.
    /// </returns>
    /// <exception cref="JsonSerializationException">Thrown if deserialization of the content fails.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="jToken"/> is null.</exception>
    private StreamDataResponse CreateStreamDataResponse<TContent>(Service service, JToken jToken) where TContent : BaseContent
    {
        var command = jToken["command"].ToObject<Command>();
        var deserializedContent = JsonConvert.DeserializeObject<IReadOnlyCollection<TContent>>(jToken["content"].ToString());
        return new StreamDataResponse(service, jToken.Value<long>("timestamp"), command, deserializedContent);
    }

    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, StreamDataResponse value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
