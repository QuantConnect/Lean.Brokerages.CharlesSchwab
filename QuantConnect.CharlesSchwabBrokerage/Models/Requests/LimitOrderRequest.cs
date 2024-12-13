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

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Requests;

/// <summary>
/// Represents a limit order request.
/// </summary>
public class LimitOrderRequest : OrderBaseRequest
{
    /// <summary>
    /// The type of the order, which is <see cref="OrderType.Limit"/>.
    /// </summary>
    public override OrderType OrderType => OrderType.Limit;

    /// <summary>
    /// The price for the limit order.
    /// </summary>
    [JsonProperty("price")]
    public decimal LimitPrice { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LimitOrderRequest"/> class
    /// for an order with multiple legs.
    /// </summary>
    /// <param name="session">The session type for the order.</param>
    /// <param name="duration">The duration of the order.</param>
    /// <param name="orderLegCollections">A list of order legs for the multi-leg order.</param>
    /// <param name="limitPrice">The price for the limit order.</param>
    public LimitOrderRequest(
        SessionType session,
        Duration duration,
        List<OrderLegRequest> orderLegCollections,
        decimal limitPrice) : base(session, duration, orderLegCollections)
    {
        LimitPrice = limitPrice;
    }
}
