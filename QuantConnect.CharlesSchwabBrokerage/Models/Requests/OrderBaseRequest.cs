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
    public abstract OrderType OrderType { get; }

    /// <summary>
    /// The session type for the order.
    /// </summary>
    public SessionType Session { get; }

    /// <summary>
    /// The duration of the order.
    /// </summary>
    public Duration Duration { get; }

    /// <summary>
    /// The order strategy type. Defaults to <see cref="OrderStrategyType.Single"/>.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public OrderStrategyType OrderStrategyType { get; } = OrderStrategyType.Single;

    /// <summary>
    /// The cancel time for the order. 
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime CancelTime { get; set; }

    /// <summary>
    /// The collection of order legs for the order.
    /// </summary>
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
/// <param name="Instruction">The instruction for the order leg.</param>
/// <param name="Quantity">The quantity for the order leg.</param>
/// <param name="Instrument">The instrument for the order leg.</param>
public record OrderLegRequest(
    Instruction Instruction,
    decimal Quantity,
    InstrumentRequest Instrument);

/// <summary>
/// Represents an instrument request for an order leg.
/// </summary>
/// <param name="Symbol">The symbol of the instrument.</param>
/// <param name="AssetType">The asset type of the instrument.</param>
public record InstrumentRequest(
    string Symbol,
    AssetType AssetType);
