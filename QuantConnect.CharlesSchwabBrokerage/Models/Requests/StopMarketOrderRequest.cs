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
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums;

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Requests;

/// <summary>
/// Represents a stop market order request.
/// </summary>
public sealed class StopMarketOrderRequest : OrderBaseRequest
{
    /// <summary>
    /// The type of the order, which is <see cref="OrderType.Stop"/>.
    /// </summary>
    public override OrderType OrderType => OrderType.Stop;

    /// <summary>
    /// The stop price for the stop market order.
    /// </summary>
    [JsonProperty("stopPrice")]
    public decimal StopPrice { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StopMarketOrderRequest"/> class.
    /// </summary>
    /// <param name="session">The session type for the order.</param>
    /// <param name="duration">The duration of the order.</param>
    /// <param name="instruction">The instruction for the order leg.</param>
    /// <param name="quantity">The quantity for the order leg.</param>
    /// <param name="symbol">The symbol for the order leg.</param>
    /// <param name="assetType">The asset type for the order leg.</param>
    /// <param name="stopPrice">The stop price for the stop market order.</param>
    public StopMarketOrderRequest(SessionType session,
        Duration duration,
        Instruction instruction,
        decimal quantity,
        string symbol,
        AssetType assetType,
        decimal stopPrice) : base(session, duration, instruction, quantity, symbol, assetType)
    {
        StopPrice = stopPrice;
    }
}
