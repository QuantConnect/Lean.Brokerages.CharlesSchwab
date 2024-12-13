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

using QuantConnect.Brokerages.CharlesSchwab.Models.Enums;
using System.Collections.Generic;

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Requests;

/// <summary>
/// Represents a market order request.
/// </summary>
public sealed class MarketOrderRequest : OrderBaseRequest
{
    /// <summary>
    /// The type of the order, which is <see cref="OrderType.Market"/>.
    /// </summary>
    public override OrderType OrderType => OrderType.Market;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarketOrderRequest"/> class for an order with multiple legs.
    /// </summary>
    /// <param name="orderLegCollections">A list of order legs for the multi-leg order.</param>
    public MarketOrderRequest(List<OrderLegRequest> orderLegCollections) : base(SessionType.Normal, Duration.Day, orderLegCollections)
    {
    }
}
