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
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums;
using QuantConnect.Brokerages.CharlesSchwab.Models.Stream;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums.Stream;

namespace QuantConnect.Brokerages.CharlesSchwab.Converters;

public class StreamDataContentConverter : JsonConverter<BaseContent>
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

    public override BaseContent ReadJson(JsonReader reader, Type objectType, BaseContent existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);

        if (token.SelectToken("key").Value<string>().Equals("Account Activity", StringComparison.InvariantCultureIgnoreCase))
        {
            return new AccountContent(
                token["seq"].Value<int>(),
                token["key"].Value<string>(),
                token["1"].Value<string>(),
                Enum.Parse<MessageType>(token["2"].Value<string>(), ignoreCase: true),
                token["3"].Value<string>()
                );
        }

        return new LevelOneContent(
            token.Value<string>("key"),
            token.Value<bool>("delayed"),
            Enum.TryParse<AssetType>(token.Value<string>("assetMainType"), ignoreCase: true, out var assetType) ? assetType : default,
            token.Value<string>("assetSubType"),
            token.Value<string>("cusip"),
            token.Value<decimal>("1"),
            token.Value<decimal>("2"),
            token.Value<decimal>("3"),
            token.Value<decimal>("4"),
            token.Value<decimal>("5"),
            token.Value<decimal>("9"),
            Time.UnixMillisecondTimeStampToDateTime(token.Value<long>("35"))
            );
    }

    public override void WriteJson(JsonWriter writer, BaseContent value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
