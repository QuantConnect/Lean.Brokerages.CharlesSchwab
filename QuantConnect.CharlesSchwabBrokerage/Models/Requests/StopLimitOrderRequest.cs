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

public class StopLimitOrderRequest : OrderBaseRequest
{
    public override OrderType OrderType => OrderType.StopLimit;

    /// <summary>
    /// The stop price for the stop market order.
    /// </summary>
    [JsonProperty("stopPrice")]
    public decimal StopPrice { get; }

    /// <summary>
    /// The price for the limit order.
    /// </summary>
    [JsonProperty("price")]
    public decimal LimitPrice { get; }

    public StopLimitOrderRequest(
        SessionType session,
        Duration duration,
        Instruction instruction,
        decimal quantity,
        string symbol,
        AssetType assetType,
        decimal stopPrice,
        decimal limitPrice) : base(session, duration, instruction, quantity, symbol, assetType)
    {
        StopPrice = stopPrice;
        LimitPrice = limitPrice;
    }
}
