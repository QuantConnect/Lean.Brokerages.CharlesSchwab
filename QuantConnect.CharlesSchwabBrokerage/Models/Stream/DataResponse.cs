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
/// <param name="Data"</param>
public class DataResponse : IStreamBaseResponse
{
    /// <summary>
    /// A collection of <see cref="Data"/> objects that contain market data details.
    /// </summary>
    public IReadOnlyCollection<Data> Data { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataResponse"/> class with the specified market data.
    /// </summary>
    /// <param name="data">A collection of <see cref="Data"/> objects representing market data details.</param>
    [JsonConstructor]
    public DataResponse(IReadOnlyCollection<Data> data) => Data = data;
}

/// <summary>
/// Represents market data with details about the service, timestamp, command, and account content.
/// </summary>
[JsonConverter(typeof(StreamDataConverter))]
public class Data
{
    /// <summary>
    /// The type of service providing the market data.
    /// </summary>
    public Service Service { get; }

    /// <summary>
    /// The Unix timestamp representing when the data was generated.
    /// </summary>
    public long Timestamp { get; }

    /// <summary>
    /// The command associated with the data, such as an update or response type.
    /// </summary>
    public Command Command { get; }

    /// <summary>
    /// A collection of <see cref="BaseContent"/> objects containing account-specific details.
    /// </summary>
    public IReadOnlyCollection<BaseContent> Content { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Data"/> class with the specified service, timestamp, command, and content.
    /// </summary>
    /// <param name="service">The type of service providing the market data.</param>
    /// <param name="timestamp">The Unix timestamp representing when the data was generated.</param>
    /// <param name="command">The command associated with the data.</param>
    /// <param name="content">A collection of <see cref="BaseContent"/> objects containing account-specific details.</param>
    [JsonConstructor]
    public Data(Service service, long timestamp, Command command, IReadOnlyCollection<BaseContent> content)
        => (Service, Timestamp, Command, Content) = (service, timestamp, command, content);
}

/// <summary>
/// Represents the base content containing a unique key used to identify a subscription or message.
/// </summary>
public class BaseContent
{
    /// <summary>
    /// A unique identifier for the content, passed back to the client to identify the subscription this response belongs to.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseContent"/> class with the specified key.
    /// </summary>
    /// <param name="key">A unique identifier for the content.</param>
    [JsonConstructor]
    public BaseContent(string key) => Key = key;
}

/// <summary>
/// Represents the content of an account-related message.
/// </summary>
public class AccountContent : BaseContent
{
    /// <summary>
    /// Identifies the message sequence number. If the client reconnects and receives the same sequence number again, it can ignore the duplicate message.
    /// </summary>
    [JsonProperty("seq")]
    public int Sequence { get; }

    /// <summary>
    /// The account number associated with the activity.
    /// </summary>
    [JsonProperty("1")]
    public string Account { get; }

    /// <summary>
    /// The type of the message, which dictates the format of the <see cref="MessageData"/> field.
    /// </summary>
    [JsonProperty("2")]
    public MessageType MessageType { get; }

    /// <summary>
    /// The core data of the message. This can be JSON-formatted data, plain text (in case of an error), or NULL in certain cases.
    /// </summary>
    [JsonProperty("3")]
    public string MessageData { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountContent"/> class.
    /// </summary>
    /// <param name="sequence">The sequence number of the message.</param>
    /// <param name="key">The unique key identifying the subscription this response belongs to.</param>
    /// <param name="account">The account number associated with the activity.</param>
    /// <param name="messageType">The type of the message.</param>
    /// <param name="messageData">The core data of the message.</param>
    [JsonConstructor]
    public AccountContent(int sequence, string key, string account, MessageType messageType, string messageData) : base(key)
        => (Sequence, Account, MessageType, MessageData) = (sequence, account, messageType, messageData);
}

/// <summary>
/// Represents the basic level one content data, including pricing, sizes, and trade time for an equity instrument.
/// </summary>
public class LevelOneContent : BaseContent
{
    /// <summary>
    /// The symbol (ticker symbol in uppercase) representing the equity instrument.
    /// </summary>
    [JsonProperty("key")]
    public string Symbol { get; }

    /// <summary>
    /// Indicates whether the data is delayed. A value of 'true' means the data is delayed.
    /// </summary>
    public bool Delayed { get; }

    /// <summary>
    /// The <see cref="AssetType"/> representing the type of asset, such as stocks, options, etc.
    /// </summary>
    public AssetType AssetMainType { get; }

    /// <summary>
    /// The current bid price for the equity instrument.
    /// </summary>
    public decimal BidPrice { get; }

    /// <summary>
    /// The current ask price for the equity instrument.
    /// </summary>
    public decimal AskPrice { get; }

    /// <summary>
    /// The price at which the last trade was matched for the equity.
    /// </summary>
    public decimal LastPrice { get; }

    /// <summary>
    /// The number of shares for the bid; typically measured in lots (100 shares per lot).
    /// </summary>
    public decimal BidSize { get; }

    /// <summary>
    /// The number of shares for the ask; typically measured in lots (100 shares per lot).
    /// </summary>
    public decimal AskSize { get; }

    /// <summary>
    /// The number of shares traded in the last transaction; typically measured in shares.
    /// </summary>
    public decimal LastSize { get; }

    /// <summary>
    /// The timestamp of the last trade, expressed in UTC DateTime.
    /// </summary>
    public DateTime TradeTime { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LevelOneContent"/> class.
    /// </summary>
    /// <param name="symbol">The symbol representing the equity instrument.</param>
    /// <param name="delayed">Indicates whether the data is delayed.</param>
    /// <param name="assetMainType">The type of asset.</param>
    /// <param name="bidPrice">The current bid price.</param>
    /// <param name="askPrice">The current ask price.</param>
    /// <param name="lastPrice">The last traded price.</param>
    /// <param name="bidSize">The size of the bid in lots.</param>
    /// <param name="askSize">The size of the ask in lots.</param>
    /// <param name="lastSize">The size of the last trade in shares.</param>
    /// <param name="tradeTime">The timestamp of the last trade.</param>
    [JsonConstructor]
    public LevelOneContent(string symbol, bool delayed, AssetType assetMainType, decimal bidPrice, decimal askPrice,
        decimal lastPrice, decimal bidSize, decimal askSize, decimal lastSize, DateTime tradeTime) : base(symbol)
    {
        Symbol = symbol;
        Delayed = delayed;
        AssetMainType = assetMainType;
        BidPrice = bidPrice;
        AskPrice = askPrice;
        LastPrice = lastPrice;
        BidSize = bidSize;
        AskSize = askSize;
        LastSize = lastSize;
        TradeTime = tradeTime;
    }
}

/// <summary>
/// Represents detailed information for a level one equity, including various pricing, trading size, and trade time details.
/// </summary>
public class LevelOneEquityContent : LevelOneContent
{
    /// <summary>
    /// The current bid price for the equity instrument.
    /// </summary>
    [JsonProperty("1")]
    public new decimal BidPrice { get; }

    /// <summary>
    /// The current ask price for the equity instrument.
    /// </summary>
    [JsonProperty("2")]
    public new decimal AskPrice { get; }

    /// <summary>
    /// The price at which the last trade was matched for the equity.
    /// </summary>
    [JsonProperty("3")]
    public new decimal LastPrice { get; }

    /// <summary>
    /// The number of shares for the bid; units are in lots (typically 100 shares per lot).
    /// </summary>
    [JsonProperty("4")]
    public new decimal BidSize { get; }

    /// <summary>
    /// The number of shares for the ask; units are in lots (typically 100 shares per lot).
    /// </summary>
    [JsonProperty("5")]
    public new decimal AskSize { get; }

    /// <summary>
    /// The number of shares traded in the last transaction; units are in shares.
    /// </summary>
    [JsonProperty("9")]
    public new decimal LastSize { get; }

    /// <summary>
    /// The timestamp of the last trade, expressed in milliseconds since the Unix epoch.
    /// </summary>
    [JsonProperty("35"), JsonConverter(typeof(CharlesSchwabUnixMillisecondsConverter))]
    public new DateTime TradeTime { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LevelOneEquityContent"/> class.
    /// </summary>
    /// <param name="symbol">The symbol representing the equity instrument.</param>
    /// <param name="delayed">Indicates whether the data is from the SIP or NFL.</param>
    /// <param name="assetMainType">The type of asset, such as stocks or options.</param>
    /// <param name="bidPrice">The current bid price.</param>
    /// <param name="askPrice">The current ask price.</param>
    /// <param name="lastPrice">The last traded price.</param>
    /// <param name="bidSize">The size of the bid in lots.</param>
    /// <param name="askSize">The size of the ask in lots.</param>
    /// <param name="lastSize">The size of the last trade in shares.</param>
    /// <param name="tradeTime">The timestamp of the last trade.</param>
    public LevelOneEquityContent(string symbol, bool delayed, AssetType assetMainType, decimal bidPrice, decimal askPrice,
        decimal lastPrice, decimal bidSize, decimal askSize, decimal lastSize, DateTime tradeTime)
        : base(symbol, delayed, assetMainType, bidPrice, askPrice, lastPrice, bidSize, askSize, lastSize, tradeTime)
    {

    }
}

/// <summary>
/// Represents detailed information for a level one option, including various pricing, contract, and expiration details.
/// </summary>
public class LevelOneOptionContent : LevelOneContent
{
    /// <summary>
    /// The current bid price for the option contract.
    /// </summary>
    [JsonProperty("2")]
    public new decimal BidPrice { get; }

    /// <summary>
    /// The current ask price for the option contract.
    /// </summary>
    [JsonProperty("3")]
    public new decimal AskPrice { get; }

    /// <summary>
    /// The price at which the last trade was matched for this option.
    /// </summary>
    [JsonProperty("4")]
    public new decimal LastPrice { get; }

    /// <summary>
    /// The total number of outstanding contracts that have not been exercised or closed.
    /// </summary>
    [JsonProperty("9")]
    public decimal OpenInterest { get; }

    /// <summary>
    /// The size of the bid (number of contracts available for purchase).
    /// </summary>
    [JsonProperty("16")]
    public new decimal BidSize { get; }

    /// <summary>
    /// The size of the ask (number of contracts available for sale).
    /// </summary>
    [JsonProperty("17")]
    public new decimal AskSize { get; }

    /// <summary>
    /// The size of the last trade (number of contracts traded in the last transaction).
    /// </summary>
    [JsonProperty("18")]
    public new decimal LastSize { get; }

    /// <summary>
    /// The timestamp of the last trade, expressed in milliseconds since the Unix epoch.
    /// </summary>
    [JsonProperty("39"), JsonConverter(typeof(CharlesSchwabUnixMillisecondsConverter))]
    public new DateTime TradeTime { get; }

    /// <summary>
    /// The indicative ask price, valid only for index options. Set to 0 for all other options.
    /// </summary>
    [JsonProperty("52")]
    public decimal IndicativeAskPrice { get; }

    /// <summary>
    /// The indicative bid price, valid only for index options. Set to 0 for all other options.
    /// </summary>
    [JsonProperty("53")]
    public decimal IndicativeBidPrice { get; }

    /// <summary>
    /// The time of the last update to the indicative bid/ask prices, expressed in milliseconds since the Unix epoch. Valid only for index options. Set to 0 for all other options.
    /// </summary>
    [JsonProperty("54"), JsonConverter(typeof(CharlesSchwabUnixMillisecondsConverter))]
    public DateTime IndicativeQuoteTime { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LevelOneOptionContent"/> class.
    /// </summary>
    /// <param name="symbol">The symbol representing the option instrument.</param>
    /// <param name="delayed">Indicates whether the data is from the SIP or NFL.</param>
    /// <param name="assetMainType">The type of asset, such as options or stocks.</param>
    /// <param name="bidPrice">The current bid price.</param>
    /// <param name="askPrice">The current ask price.</param>
    /// <param name="lastPrice">The last traded price.</param>
    /// <param name="openInterest">The total number of outstanding contracts.</param>
    /// <param name="bidSize">The size of the bid in contracts.</param>
    /// <param name="askSize">The size of the ask in contracts.</param>
    /// <param name="lastSize">The size of the last trade in contracts.</param>
    /// <param name="tradeTime">The timestamp of the last trade.</param>
    /// <param name="indicativeAskPrice">The indicative ask price for index options.</param>
    /// <param name="indicativeBidPrice">The indicative bid price for index options.</param>
    /// <param name="indicativeQuoteTime">The timestamp of the indicative price update.</param>
    [JsonConstructor]
    public LevelOneOptionContent(string symbol, bool delayed, AssetType assetMainType, decimal bidPrice, decimal askPrice,
        decimal lastPrice, decimal openInterest, decimal bidSize, decimal askSize, decimal lastSize, DateTime tradeTime, decimal indicativeAskPrice, decimal indicativeBidPrice, DateTime indicativeQuoteTime) : base(symbol, delayed, assetMainType, bidPrice, askPrice, lastPrice, bidSize, askSize, lastSize, tradeTime)
    {
        OpenInterest = openInterest;
        IndicativeAskPrice = indicativeAskPrice;
        IndicativeBidPrice = indicativeBidPrice;
        IndicativeQuoteTime = indicativeQuoteTime;
    }
}