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
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Enums;

/// <summary>
/// Represents the various types of orders that can be placed in the Charles Schwab.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum OrderType
{
    /// <summary>
    /// A market order, executed at the current market price.
    /// </summary>
    [EnumMember(Value = "MARKET")]
    Market = 0,

    /// <summary>
    /// A limit order, executed at a specified price or better.
    /// </summary>
    [EnumMember(Value = "LIMIT")]
    Limit = 1,

    /// <summary>
    /// A stop order, which becomes a market order once a specified price is reached.
    /// </summary>
    [EnumMember(Value = "STOP")]
    Stop = 2,

    /// <summary>
    /// A stop-limit order, executed at a specified price once a stop price is triggered.
    /// </summary>
    [EnumMember(Value = "STOP_LIMIT")]
    StopLimit = 3,

    /// <summary>
    /// A trailing stop order, which follows the stock's price movements and triggers a sale at the stop price.
    /// </summary>
    [EnumMember(Value = "TRAILING_STOP")]
    TrailingStop = 4,

    /// <summary>
    /// A cabinet order, typically used for low-priced options.
    /// </summary>
    [EnumMember(Value = "CABINET")]
    Cabinet = 5,

    /// <summary>
    /// A non-marketable order that may not execute immediately due to price or liquidity.
    /// </summary>
    [EnumMember(Value = "NON_MARKETABLE")]
    NonMarketable = 6,

    /// <summary>
    /// A market-on-close order, executed as close to the market's closing price as possible.
    /// </summary>
    [EnumMember(Value = "MARKET_ON_CLOSE")]
    MarketOnClose = 7,

    /// <summary>
    /// An exercise order for options or other contracts.
    /// </summary>
    [EnumMember(Value = "EXERCISE")]
    Exercise = 8,

    /// <summary>
    /// A trailing stop-limit order that combines features of both stop-limit and trailing stop orders.
    /// </summary>
    [EnumMember(Value = "TRAILING_STOP_LIMIT")]
    TrailingStopLimit = 9,

    /// <summary>
    /// A net debit order, commonly used for options trading.
    /// </summary>
    [EnumMember(Value = "NET_DEBIT")]
    NetDebit = 10,

    /// <summary>
    /// A net credit order, commonly used for options trading.
    /// </summary>
    [EnumMember(Value = "NET_CREDIT")]
    NetCredit = 11,

    /// <summary>
    /// A net zero order, typically used in options trading for zero-cost strategies.
    /// </summary>
    [EnumMember(Value = "NET_ZERO")]
    NetZero = 12,

    /// <summary>
    /// A limit-on-close order, which executes at the closing price or better.
    /// </summary>
    [EnumMember(Value = "LIMIT_ON_CLOSE")]
    LimitOnClose = 13,

    /// <summary>
    /// The order type is unknown or unspecified.
    /// </summary>
    [EnumMember(Value = "UNKNOWN")]
    Unknown = 14
}
