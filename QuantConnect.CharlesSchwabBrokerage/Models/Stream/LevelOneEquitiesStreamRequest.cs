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

using QuantConnect.Brokerages.CharlesSchwab.Models.Enums.Stream;

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Stream;

/// <summary>
/// Represents a request to stream level-one equity data, providing details such as bid/ask prices, sizes, volumes, and timestamps.
/// </summary>
public class LevelOneEquitiesStreamRequest : StreamRequest
{
    /// <summary>
    /// The specific service type for this request, which is <see cref="Service.LevelOneEquities"/>.
    /// </summary>
    public override Service Service => Service.LevelOneEquities;

    /// <summary>
    /// Specifies the fields to retrieve in the data stream, represented by comma-separated field IDs:
    /// 0 - Symbol (Ticker symbol in upper case)
    /// 1 - Bid Price (Current Bid Price)
    /// 2 - Ask Price (Current Ask Price)
    /// 3 - Last Price (Price at which the last trade was matched)
    /// 4 - Bid Size (Number of shares for bid; units are in lots, typically 100 shares per lot)
    /// 5 - Ask Size (Number of shares for ask)
    /// 9 - Last Size (Number of shares traded in the last trade; units are in shares)
    /// 35 - Trade Time in milliseconds since Epoch (last trade time)
    /// </summary>
    public const string Fields = "0,1,2,3,4,5,9,35";

    /// <summary>
    /// Initializes a new instance of the <see cref="LevelOneEquitiesStreamRequest"/> class with the specified parameters.
    /// </summary>
    /// <param name="requestId">The unique identifier for this request, used to track individual responses.</param>
    /// <param name="command">The command indicating the action to perform, such as subscribe or unsubscribe.</param>
    /// <param name="schwabClientCustomerId">The unique customer ID of the client, retrieved from user preferences.</param>
    /// <param name="schwabClientCorrelId">A unique correlation ID attached to requests and messages for tracking a transaction or event chain.</param>
    /// <param name="symbols">One or more ticker symbols representing equities to include in the stream request.</param>
    public LevelOneEquitiesStreamRequest(
        int requestId,
        Command command,
        string schwabClientCustomerId,
        string schwabClientCorrelId,
        string[] symbols
        ) : base(requestId, command, schwabClientCustomerId, schwabClientCorrelId, new Parameter(string.Join(',', symbols), Fields))
    { }
}
