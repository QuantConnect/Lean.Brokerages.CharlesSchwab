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
public class OptionChainResponse
{
    /// <summary>
    /// The symbol of the option chain.
    /// </summary>
    public string Symbol { get; }

    /// <summary>
    /// The main type of the asset (e.g., "Equity", "Option").
    /// </summary>
    [JsonProperty("assetMainType")]
    public AssetType AssetType { get; }

    /// <summary>
    /// The call option expiration date map.
    /// The map is structured with expiration dates as keys and a nested dictionary containing strike prices and collections of option metadata.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyDictionary<decimal, IReadOnlyCollection<OptionMetaData>>> CallExpDateMap { get; }

    /// <summary>
    /// The put option expiration date map.
    /// The map is structured similarly to the call map, with expiration dates as keys and strike price dictionaries.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyDictionary<decimal, IReadOnlyCollection<OptionMetaData>>> PutExpDateMap { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionChainResponse"/> class with the specified parameters.
    /// </summary>
    /// <param name="symbol">The symbol of the option chain.</param>
    /// <param name="assetType">The main type of the asset (e.g., "Equity", "Option").</param>
    /// <param name="callExpDateMap">
    /// The call option expiration date map. 
    /// The map is structured with expiration dates as keys and a nested dictionary containing strike prices and collections of option metadata.
    /// </param>
    /// <param name="putExpDateMap">
    /// The put option expiration date map. 
    /// The map is structured similarly to the call map, with expiration dates as keys and strike price dictionaries.
    /// </param>
    [JsonConstructor]
    public OptionChainResponse(
        string symbol,
        AssetType assetType,
        IReadOnlyDictionary<string, IReadOnlyDictionary<decimal, IReadOnlyCollection<OptionMetaData>>> callExpDateMap,
        IReadOnlyDictionary<string, IReadOnlyDictionary<decimal, IReadOnlyCollection<OptionMetaData>>> putExpDateMap)
    {
        Symbol = symbol;
        AssetType = assetType;
        CallExpDateMap = callExpDateMap;
        PutExpDateMap = putExpDateMap;
    }
}

/// <summary>
/// Represents metadata for an individual option.
/// </summary>
public readonly struct OptionMetaData
{
    /// <summary>
    /// The symbol for the specific option contract.
    /// </summary>
    public string Symbol { get; }

    /// <summary>
    /// The name of the exchange where the option is listed.
    /// </summary>
    public string ExchangeName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionMetaData"/> struct with the specified parameters.
    /// </summary>
    /// <param name="symbol">The symbol for the specific option contract.</param>
    /// <param name="exchangeName">The name of the exchange where the option is listed.</param>
    [JsonConstructor]
    public OptionMetaData(string symbol, string exchangeName) => (Symbol, ExchangeName) = (symbol, exchangeName);
}

