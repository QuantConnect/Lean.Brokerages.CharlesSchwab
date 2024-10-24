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

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Enums;

/// <summary>
/// Represents the various types of orders that can be placed in the Charles Schwab.
/// </summary>
public enum CharlesSchwabOrderType
{
    /// <summary>
    /// A market order, executed at the current market price.
    /// </summary>
    [JsonProperty("MARKET")]
    Market = 0,

    /// <summary>
    /// A limit order, executed at a specified price or better.
    /// </summary>
    [JsonProperty("LIMIT")]
    Limit = 1,

    /// <summary>
    /// A stop order, which becomes a market order once a specified price is reached.
    /// </summary>
    [JsonProperty("STOP")]
    Stop = 2,

    /// <summary>
    /// A stop-limit order, executed at a specified price once a stop price is triggered.
    /// </summary>
    [JsonProperty("STOP_LIMIT")]
    StopLimit = 3,

    /// <summary>
    /// A trailing stop order, which follows the stock's price movements and triggers a sale at the stop price.
    /// </summary>
    [JsonProperty("TRAILING_STOP")]
    TrailingStop = 4,

    /// <summary>
    /// A cabinet order, typically used for low-priced options.
    /// </summary>
    [JsonProperty("CABINET")]
    Cabinet = 5,

    /// <summary>
    /// A non-marketable order that may not execute immediately due to price or liquidity.
    /// </summary>
    [JsonProperty("NON_MARKETABLE")]
    NonMarketable = 6,

    /// <summary>
    /// A market-on-close order, executed as close to the market's closing price as possible.
    /// </summary>
    [JsonProperty("MARKET_ON_CLOSE")]
    MarketOnClose = 7,

    /// <summary>
    /// An exercise order for options or other contracts.
    /// </summary>
    [JsonProperty("EXERCISE")]
    Exercise = 8,

    /// <summary>
    /// A trailing stop-limit order that combines features of both stop-limit and trailing stop orders.
    /// </summary>
    [JsonProperty("TRAILING_STOP_LIMIT")]
    TrailingStopLimit = 9,

    /// <summary>
    /// A net debit order, commonly used for options trading.
    /// </summary>
    [JsonProperty("NET_DEBIT")]
    NetDebit = 10,

    /// <summary>
    /// A net credit order, commonly used for options trading.
    /// </summary>
    [JsonProperty("NET_CREDIT")]
    NetCredit = 11,

    /// <summary>
    /// A net zero order, typically used in options trading for zero-cost strategies.
    /// </summary>
    [JsonProperty("NET_ZERO")]
    NetZero = 12,

    /// <summary>
    /// A limit-on-close order, which executes at the closing price or better.
    /// </summary>
    [JsonProperty("LIMIT_ON_CLOSE")]
    LimitOnClose = 13,

    /// <summary>
    /// The order type is unknown or unspecified.
    /// </summary>
    [JsonProperty("UNKNOWN")]
    Unknown = 14
}
