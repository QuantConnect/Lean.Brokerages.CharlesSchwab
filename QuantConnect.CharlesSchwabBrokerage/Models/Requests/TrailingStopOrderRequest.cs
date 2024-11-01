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
/// Represents a tailing stop order request.
/// </summary>
public class TrailingStopOrderRequest : OrderBaseRequest
{
    /// <summary>
    /// The type of the order, which is <see cref="OrderType.TrailingStop"/>.
    /// </summary>
    public override OrderType OrderType => OrderType.TrailingStop;

    /// <summary>
    /// The stop price link basis for the trailing stop order.
    /// This indicates how the stop price is linked to the market price.
    /// </summary>
    [JsonProperty("stopPriceLinkBasis")]
    public StopPriceLinkBasis StopPriceLinkBasis => StopPriceLinkBasis.Mark;

    /// <summary>
    /// Еhe stop price link type for the trailing stop order.
    /// </summary>
    [JsonProperty("stopPriceLinkType")]
    public StopPriceLinkType StopPriceLinkType { get; }

    /// <summary>
    /// The stop price offset for the trailing stop order.
    /// </summary>
    [JsonProperty("stopPriceOffset")]
    public decimal StopPriceOffset { get; }

    public TrailingStopOrderRequest(
        Duration duration,
        Instruction instruction,
        decimal quantity,
        string symbol,
        AssetType assetType,
        StopPriceLinkType stopPriceLinkType,
        decimal stopPriceOffset) : base(SessionType.Normal, duration, instruction, quantity, symbol, assetType)
    {
        StopPriceLinkType = stopPriceLinkType;
        StopPriceOffset = stopPriceOffset;
    }
}
