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
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums;
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
        var command = jToken["command"].ToObject<Command>();

        switch (service)
        {
            case Service.Account:
                var accountContents = new List<AccountContent>();
                foreach (var accountContent in jToken["content"])
                {
                    var con = new AccountContent(
                        accountContent.Value<int>("seq"),
                        accountContent.Value<string>("key"),
                        accountContent.Value<string>("1"),
                        accountContent["2"].ToObject<MessageType>(),
                        accountContent.Value<string>("3"));
                    accountContents.Add(con);
                }
                return new StreamDataResponse(service, jToken.Value<long>("timestamp"), command, accountContents);
            case Service.LevelOneEquities:
                var levelOneEquities = new List<LevelOneEquityContent>();
                foreach (JObject content in jToken["content"])
                {
                    var equity = new LevelOneEquityContent(
                        content.Value<string>("key"),
                        content.Value<bool>("delayed"),
                        content.TryGetValue("assetMainType", StringComparison.InvariantCultureIgnoreCase, out var assetType) ? assetType.ToObject<AssetType>() : default,
                        content.Value<decimal>("1"),
                        content.Value<decimal>("2"),
                        content.Value<decimal>("3"),
                        content.Value<decimal>("4"),
                        content.Value<decimal>("5"),
                        content.Value<decimal>("9"),
                        Time.UnixMillisecondTimeStampToDateTime(content.Value<long>("35"))
                        );
                    levelOneEquities.Add(equity);
                }
                return new StreamDataResponse(service, jToken.Value<long>("timestamp"), command, levelOneEquities);
            case Service.LevelOneOptions:
                var levelOneOptions = new List<LevelOneOptionContent>();
                foreach (JObject content in jToken["content"])
                {
                    var option = new LevelOneOptionContent(
                        content.Value<string>("key"),
                        content.Value<bool>("delayed"),
                        content.TryGetValue("assetMainType", StringComparison.InvariantCultureIgnoreCase, out var assetType) ? assetType.ToObject<AssetType>() : default,
                        content.Value<decimal>("2"),
                        content.Value<decimal>("3"),
                        content.Value<decimal>("4"),
                        content.Value<decimal>("9"),
                        content.Value<decimal>("16"),
                        content.Value<decimal>("17"),
                        content.Value<decimal>("18"),
                        Time.UnixMillisecondTimeStampToDateTime(content.Value<long>("39")),
                        content.Value<decimal>("52"),
                        content.Value<decimal>("53"),
                        Time.UnixMillisecondTimeStampToDateTime(content.Value<long>("54")));
                    levelOneOptions.Add(option);
                }
                return new StreamDataResponse(service, jToken.Value<long>("timestamp"), command, levelOneOptions);
            default:
                throw new NotImplementedException($"{nameof(StreamDataConverter)}.{nameof(ReadJson)}: The service '{service}' is not supported or implemented.");
        }
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
