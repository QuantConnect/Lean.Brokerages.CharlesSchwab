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
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums;

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Requests;

/// <summary>
/// Base class for order requests. 
/// </summary>
public abstract class OrderBaseRequest
{
    /// <summary>
    /// The type of the order.
    /// </summary>
    [JsonProperty("orderType")]
    public abstract OrderType OrderType { get; }

    /// <summary>
    /// The session type for the order.
    /// </summary>
    [JsonProperty("session")]
    public SessionType Session { get; }

    /// <summary>
    /// The duration of the order.
    /// </summary>
    [JsonProperty("duration")]
    public Duration Duration { get; }

    /// <summary>
    /// The order strategy type. Defaults to <see cref="OrderStrategyType.Single"/>.
    /// </summary>
    [JsonProperty("orderStrategyType", Required = Required.Always)]
    public OrderStrategyType OrderStrategyType { get; } = OrderStrategyType.Single;

    /// <summary>
    /// The cancel time for the order. 
    /// </summary>
    [JsonProperty("cancelTime", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime CancelTime { get; set; }

    /// <summary>
    /// The collection of order legs for the order.
    /// </summary>
    [JsonProperty("orderLegCollection")]
    public List<OrderLegRequest> OrderLegCollection { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderBaseRequest"/> class.
    /// </summary>
    /// <param name="session">The session type for the order.</param>
    /// <param name="duration">The duration of the order.</param>
    /// <param name="instruction">The instruction for the order leg.</param>
    /// <param name="quantity">The quantity for the order leg.</param>
    /// <param name="symbol">The symbol for the order leg.</param>
    /// <param name="assetType">The asset type for the order leg.</param>
    protected internal OrderBaseRequest(
        SessionType session,
        Duration duration,
        Instruction instruction,
        decimal quantity,
        string symbol,
        AssetType assetType)
    {
        Session = session;
        Duration = duration;
        OrderLegCollection = new List<OrderLegRequest> { new OrderLegRequest(instruction, quantity, new InstrumentRequest(symbol, assetType)) };
    }
}

/// <summary>
/// Represents a single leg of an order.
/// </summary>
public class OrderLegRequest
{
    /// <summary>
    /// Gets the instruction for the order leg (e.g., buy, sell, etc.).
    /// </summary>
    [JsonProperty("instruction")]
    public Instruction Instruction { get; }

    /// <summary>
    /// Gets the quantity for the order leg.
    /// </summary>
    [JsonProperty("quantity")]
    public decimal Quantity { get; }

    /// <summary>
    /// Gets the instrument associated with the order leg.
    /// </summary>
    [JsonProperty("instrument")]
    public InstrumentRequest Instrument { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderLegRequest"/> class.
    /// </summary>
    /// <param name="instruction">The instruction for the order leg.</param>
    /// <param name="quantity">The quantity for the order leg.</param>
    /// <param name="instrument">The instrument associated with the order leg.</param>
    [JsonConstructor]
    public OrderLegRequest(Instruction instruction, decimal quantity, InstrumentRequest instrument)
        => (Instruction, Quantity, Instrument) = (instruction, quantity, instrument);
}

/// <summary>
/// Represents an instrument request for an order leg.
/// </summary>
public readonly struct InstrumentRequest
{
    /// <summary>
    /// Gets the symbol of the instrument.
    /// </summary>
    [JsonProperty("symbol")]
    public string Symbol { get; }

    /// <summary>
    /// Gets the asset type of the instrument.
    /// </summary>
    [JsonProperty("assetType")]
    public AssetType AssetType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InstrumentRequest"/> struct.
    /// </summary>
    /// <param name="symbol">The symbol of the instrument.</param>
    /// <param name="assetType">The asset type of the instrument.</param>
    [JsonConstructor]
    public InstrumentRequest(string symbol, AssetType assetType) => (Symbol, AssetType) = (symbol, assetType);
}
