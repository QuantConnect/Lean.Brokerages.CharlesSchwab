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
/// Represents a request for subscribing to account activity stream updates.
/// </summary>
public class AccountStreamRequest : StreamRequest
{
    /// <summary>
    /// The key specifying the type of account activity to track.
    /// </summary>
    public const string Keys = "Account Activity";

    /// <summary>
    /// Specifies the fields included in the account activity response. Each field represents distinct information:
    /// <list type="bullet">
    /// <item>
    /// <term>"1"</term><description>Account: The account number where the activity occurred.</description>
    /// </item>
    /// <item>
    /// <term>"2"</term><description>Message Type: A string that specifies the format of the Message Data field.</description>
    /// </item>
    /// <item>
    /// <term>"3"</term><description>Message Data: The main content of the message, which may be JSON-formatted data, plain text, or NULL in case of an error.</description>
    /// </item>
    /// </list>
    /// </summary>
    public const string Fields = "0,1,2,3";

    /// <summary>
    /// The specific service type for this request, which is <see cref="Service.Account"/>.
    /// </summary>
    public override Service Service => Service.Account;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountStreamRequest"/> class with details for subscribing to account activity updates.
    /// </summary>
    /// <param name="requestId">The unique identifier for this request, used to track individual responses.</param>
    /// <param name="schwabClientCustomerId">The unique customer ID of the client as retrieved from user preferences.</param>
    /// <param name="schwabClientCorrelId">A unique correlation ID attached to requests and messages, allowing tracking of a transaction or event chain.</param>
    public AccountStreamRequest(
        int requestId,
        string schwabClientCustomerId,
        string schwabClientCorrelId) : base(requestId, Command.Subscription, schwabClientCustomerId, schwabClientCorrelId, new Parameter(Keys, Fields))
    {
    }
}
