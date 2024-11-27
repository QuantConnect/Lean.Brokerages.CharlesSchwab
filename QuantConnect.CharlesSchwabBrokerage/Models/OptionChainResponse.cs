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

using Newtonsoft.Json;
using System.Collections.Generic;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums;

namespace QuantConnect.Brokerages.CharlesSchwab.Models;

/// <summary>
/// Represents the response for an option chain request.
/// </summary>
/// <param name="symbol">The symbol of the option chain.</param>
/// <param name="AssetType">The main type of the asset (e.g., "Equity", "Option").</param>
/// <param name="CallExpDateMap">
/// The call option expiration date map.
/// The map is structured with expiration dates as keys and a nested dictionary containing strike prices and collections of option metadata.</param>
/// <param name="PutExpDateMap">
/// the put option expiration date map.
/// The map is structured similarly to the call map, with expiration dates as keys and strike price dictionaries.
/// </param>
public record OptionChainResponse(
   [JsonProperty("symbol")] string symbol,
   [JsonProperty("assetMainType")] AssetType AssetType,
   [JsonProperty("callExpDateMap")] IReadOnlyDictionary<string, IReadOnlyDictionary<decimal, IReadOnlyCollection<OptionMetaData>>> CallExpDateMap,
   [JsonProperty("putExpDateMap")] IReadOnlyDictionary<string, IReadOnlyDictionary<decimal, IReadOnlyCollection<OptionMetaData>>> PutExpDateMap);

/// <summary>
/// Represents metadata for an individual option.
/// </summary>
/// <param name="Symbol">The symbol for the specific option contract.</param>
/// <param name="ExchangeName">The name of the exchange where the option is listed.</param>
public record struct OptionMetaData(
    [JsonProperty("symbol")] string Symbol,
    [JsonProperty("exchangeName")] string ExchangeName);

