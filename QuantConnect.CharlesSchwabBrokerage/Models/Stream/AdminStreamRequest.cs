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
/// Represents a request for administrative services sent over WebSocket.
/// </summary>
public class AdminStreamRequest : StreamRequest
{
    /// <summary>
    /// The specific service type for this request, which is <see cref="Service.Admin"/>.
    /// </summary>
    public override Service Service => Service.Admin;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminStreamRequest"/> class with required administrative parameters.
    /// </summary>
    /// <param name="requestId">The unique identifier for the request.</param>
    /// <param name="command">The command indicating the specific action for the request.</param>
    /// <param name="schwabClientCustomerId">The customer ID for the client associated with the request.</param>
    /// <param name="schwabClientCorrelId">The correlation ID allowing clients to track individual requests.</param>
    /// <param name="accessToken">The access token required for authorization, if applicable.</param>
    /// <param name="schwabClientChannel">The client channel used to send the request.</param>
    /// <param name="schwabClientFunctionId">The client function identifier for specifying the function of the request.</param>
    public AdminStreamRequest(
        int requestId,
        Command command,
        string schwabClientCustomerId,
        string schwabClientCorrelId,
        string accessToken,
        string schwabClientChannel,
        string schwabClientFunctionId) : base(requestId, command, schwabClientCustomerId, schwabClientCorrelId, new Parameter(accessToken, schwabClientChannel, schwabClientFunctionId))
    {
    }
}