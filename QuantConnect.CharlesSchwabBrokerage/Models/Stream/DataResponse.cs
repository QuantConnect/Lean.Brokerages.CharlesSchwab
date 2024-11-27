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
using System.Collections.Generic;
using QuantConnect.Brokerages.CharlesSchwab.Converters;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums.Stream;

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Stream;

/// <summary>
/// Represents a response containing <b>streaming market data</b>.
/// </summary>
/// <param name="Data">A collection of <see cref="Data"/> objects that contain market data details.</param>
public record DataResponse([property: JsonProperty("data")] IReadOnlyCollection<Data> Data) : IStreamBaseResponse;

/// <summary>
/// Represents market data with details about the service, timestamp, command, and account content.
/// </summary>
/// <param name="Service">The type of service providing the market data.</param>
/// <param name="Timestamp">The Unix timestamp representing when the data was generated.</param>
/// <param name="Command">The command associated with the data, such as an update or response type.</param>
/// <param name="Content">A collection of <see cref="AccountContent"/> objects containing account-specific details.</param>
[JsonConverter(typeof(StreamDataConverter))]
public record Data(
    [property: JsonProperty("service")] Service Service,
    [property: JsonProperty("timestamp")] long Timestamp,
    [property: JsonProperty("command")] Command Command,
    [property: JsonProperty("content")] IReadOnlyCollection<BaseContent> Content);

/// <summary>
/// Represents the base content that contains a key used to identify a subscription or message.
/// </summary>
/// <param name="Key">A unique identifier for the content, passed back to the client to identify the subscription this response belongs to.</param>
public record BaseContent([property: JsonProperty("key")] string Key);

/// <summary>
/// Represents the content of an account-related message.
/// </summary>
/// <param name="Sequence">This field identifies the message number. If client reconnects and receives the same seq number again, it can choose to ignore the duplicate.</param>
/// <param name="Key">Passed back to the client from the request to identify a subscription this response belongs to.</param>
/// <param name="Account">Account Number that the activity occurred on.</param>
/// <param name="MessageType">Message Type that dictates the format of the Message Data field.</param>
/// <param name="MessageData">The core data for the message. Either JSON-formatted data describing the update, NULL in some cases, or plain text in case of ERROR.</param>
public record AccountContent(
    [property: JsonProperty("seq")] int Sequence,
    string Key,
    [property: JsonProperty("1")] string Account,
    [property: JsonProperty("2")] MessageType MessageType,
    [property: JsonProperty("3")] string MessageData) : BaseContent(Key);

/// <summary>
/// Represents the basic level one content data, including pricing, sizes, and trade time for an equity instrument.
/// </summary>
/// <param name="Symbol">The symbol (ticker symbol in uppercase) representing the equity instrument.</param>
/// <param name="Delayed">Indicates whether the data is delayed. A value of 'true' means the data is delayed.</param>
/// <param name="assetMainType">The <see cref="AssetType"/> representing the type of asset, such as stocks, options, etc.</param>
/// <param name="BidPrice">The current bid price for the equity instrument.</param>
/// <param name="AskPrice">The current ask price for the equity instrument.</param>
/// <param name="LastPrice">The price at which the last trade was matched for the equity.</param>
/// <param name="BidSize">The number of shares for the bid; typically measured in lots (100 shares per lot).</param>
/// <param name="AskSize">The number of shares for the ask; typically measured in lots (100 shares per lot).</param>
/// <param name="LastSize">The number of shares traded in the last transaction; typically measured in shares.</param>
/// <param name="TradeTime">The timestamp of the last trade, expressed in UTC DateTime.</param>
public record LevelOneContent(
    [property: JsonProperty("key")] string Symbol,
    [property: JsonProperty("delayed")] bool Delayed,
    [property: JsonProperty("assetMainType")] AssetType assetMainType,
    decimal BidPrice,
    decimal AskPrice,
    decimal LastPrice,
    decimal BidSize,
    decimal AskSize,
    decimal LastSize,
    DateTime TradeTime) : BaseContent(Symbol);

/// <summary>
/// Represents detailed information for a level one equity, including various pricing, trading size, and trade time details.
/// </summary>
/// <param name="Symbol">The symbol (ticker symbol in upper case) representing the equity instrument.</param>
/// <param name="Delayed">Indicates whether the data is from the SIP (Securities Information Processor) or NFL (non-firm liquidity).</param>
/// <param name="AssetMainType">The <see cref="AssetType"/> that represents the type of asset, such as stocks, options, etc.</param>
/// <param name="BidPrice">The current bid price for the equity instrument.</param>
/// <param name="AskPrice">The current ask price for the equity instrument.</param>
/// <param name="LastPrice">The price at which the last trade was matched for the equity.</param>
/// <param name="BidSize">The number of shares for the bid; units are in lots (typically 100 shares per lot).</param>
/// <param name="AskSize">The number of shares for the ask; units are in lots (typically 100 shares per lot).</param>
/// <param name="LastSize">The number of shares traded in the last transaction; units are in shares.</param>
/// <param name="TradeTime">The timestamp of the last trade, expressed in milliseconds since the Unix epoch.</param>
public record LevelOneEquityContent(
    string Symbol,
    bool Delayed,
    AssetType AssetMainType,
    [JsonProperty("1")] decimal BidPrice,
    [JsonProperty("2")] decimal AskPrice,
    [JsonProperty("3")] decimal LastPrice,
    [JsonProperty("4")] decimal BidSize,
    [JsonProperty("5")] decimal AskSize,
    [JsonProperty("9")] decimal LastSize,
    [JsonProperty("35"), JsonConverter(typeof(CharlesSchwabUnixMillisecondsConverter))] DateTime TradeTime
    ) : LevelOneContent(Symbol, Delayed, AssetMainType, BidPrice, AskPrice, LastPrice, BidSize, AskSize, LastSize, TradeTime);

/// <summary>
/// Represents detailed information for a level one option, including various pricing, contract, and expiration details.
/// </summary>
/// <param name="Symbol">The symbol (ticker symbol in upper case) representing the option instrument.</param>
/// <param name="Delayed">Indicates whether the data is from the SIP (Securities Information Processor) or NFL (non-firm liquidity).</param>
/// <param name="AssetMainType">The <see cref="AssetType"/> that represents the type of asset, such as options, stocks, etc.</param>
/// <param name="BidPrice">The current bid price for the option contract.</param>
/// <param name="AskPrice">The current ask price for the option contract.</param>
/// <param name="LastPrice">The price at which the last trade was matched for this option.</param>
/// <param name="OpenInterest">The total number of outstanding contracts that have not been exercised or closed.</param>
/// <param name="BidSize">The size of the bid (number of contracts available for purchase).</param>
/// <param name="AskSize">The size of the ask (number of contracts available for sale).</param>
/// <param name="LastSize">The size of the last trade (number of contracts traded in the last transaction).</param>
/// <param name="TradeTime">The timestamp of the last trade, expressed in milliseconds since the Unix epoch.</param>
/// <param name="IndicativeAskPrice">The indicative ask price, which is only valid for index options (set to 0 for all other options).</param>
/// <param name="IndicativeBidPrice">The indicative bid price, which is only valid for index options (set to 0 for all other options).</param>
/// <param name="IndicativeQuoteTime">The time of the last update to the indicative bid/ask prices, expressed in milliseconds since the Unix epoch. Only valid for index options (set to 0 for all other options).</param>
public record LevelOneOptionContent(
    string Symbol,
    bool Delayed,
    AssetType AssetMainType,
    [JsonProperty("2")] decimal BidPrice,
    [JsonProperty("3")] decimal AskPrice,
    [JsonProperty("4")] decimal LastPrice,
    [JsonProperty("9")] decimal OpenInterest,
    [JsonProperty("16")] decimal BidSize,
    [JsonProperty("17")] decimal AskSize,
    [JsonProperty("18")] decimal LastSize,
    [JsonProperty("39"), JsonConverter(typeof(CharlesSchwabUnixMillisecondsConverter))] DateTime TradeTime,
    [JsonProperty("52")] decimal IndicativeAskPrice,
    [JsonProperty("53")] decimal IndicativeBidPrice,
    [JsonProperty("54"), JsonConverter(typeof(CharlesSchwabUnixMillisecondsConverter))] DateTime IndicativeQuoteTime
    ) : LevelOneContent(Symbol, Delayed, AssetMainType, BidPrice, AskPrice, LastPrice, BidSize, AskSize, LastSize, TradeTime);