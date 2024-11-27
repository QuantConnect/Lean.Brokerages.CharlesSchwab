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
/// Represents a request to stream level one options data, including bid and ask prices, trade sizes, and other option-related details.
/// </summary>
public class LevelOneOptionsStreamRequest : StreamRequest
{
    /// <summary>
    /// The specific service type for this request, which is <see cref="Service.LevelOneOptions"/>.
    /// </summary>
    public override Service Service => Service.LevelOneOptions;

    /// <summary>
    /// Specifies the fields to retrieve in the data stream, represented by comma-separated field IDs:
    /// <list type="bullet">
    ///     <item><description>0 - Symbol (Ticker symbol in upper case)</description></item>
    ///     <item><description>2 - Bid Price (Current Bid Price)</description></item>
    ///     <item><description>3 - Ask Price (Current Ask Price)</description></item>
    ///     <item><description>4 - Last Price (Price at which the last trade was matched)</description></item>
    ///     <item><description>9 - Open Interest</description></item>
    ///     <item><description>16 - Bid Size (Number of contracts for bid)</description></item>
    ///     <item><description>17 - Ask Size (Number of contracts for ask)</description></item>
    ///     <item><description>18 - Last Size (Number of contracts traded with last trade)</description></item>
    ///     <item><description>39 - Trade Time in Long (Last trade time in milliseconds since Epoch)</description></item>
    ///     <item><description>52 - Indicative Ask Price (Only valid for index options, 0 for all other options)</description></item>
    ///     <item><description>53 - Indicative Bid Price (Only valid for index options, 0 for all other options)</description></item>
    ///     <item><description>54 - Indicative Quote Time (The latest time the indicative bid/ask prices were updated, in milliseconds since Epoch, only valid for index options)</description></item>
    /// </list>
    /// </summary>
    public const string Fields = "0,2,3,4,9,16,17,18,39,52,53,54";

    /// <summary>
    /// Initializes a new instance of the <see cref="LevelOneOptionsStreamRequest"/> class with the specified parameters.
    /// </summary>
    /// <param name="requestId">The unique request identifier for this stream request.</param>
    /// <param name="command">The command to be executed for this request (e.g., Add or Unsubscribe).</param>
    /// <param name="schwabClientCustomerId">The Schwab client customer ID associated with the request.</param>
    /// <param name="schwabClientCorrelId">The Schwab client correlation ID associated with the request.</param>
    /// <param name="symbols">An array of option symbols (ticker symbols) to subscribe to or unsubscribe from.</param>
    public LevelOneOptionsStreamRequest(
        int requestId,
        Command command,
        string schwabClientCustomerId,
        string schwabClientCorrelId,
        string[] symbols) : base(requestId, command, schwabClientCustomerId, schwabClientCorrelId, new Parameter(string.Join(',', symbols), Fields))
    {

    }
}
