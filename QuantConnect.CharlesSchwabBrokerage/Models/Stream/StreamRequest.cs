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
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums.Stream;

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Stream;


/// <summary>
/// Represents a base request for streaming data over WebSocket.
/// </summary>
public abstract class StreamRequest
{
    /// <summary>
    /// The unique identifier for the request.
    /// </summary>
    [JsonProperty("requestid")]
    public string RequestId { get; }

    /// <summary>
    /// The service associated with the request.
    /// </summary>
    [JsonProperty("service")]
    public abstract Service Service { get; }

    /// <summary>
    /// The command to be executed, indicating the specific action required.
    /// </summary>
    [JsonProperty("command")]
    public Command Command { get; }

    /// <summary>
    /// The client customer ID associated with the request.
    /// </summary>
    [JsonProperty("SchwabClientCustomerId")]
    public string SchwabClientCustomerId { get; }

    /// <summary>
    /// The client correlation ID, allowing clients to track individual requests.
    /// </summary>
    [JsonProperty("SchwabClientCorrelId")]
    public string SchwabClientCorrelId { get; }

    /// <summary>
    /// Any additional parameters required for the request, such as authorization or data fields.
    /// </summary>
    [JsonProperty("parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Parameter Parameters { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamRequest"/> class.
    /// </summary>
    /// <param name="requestId">The unique identifier for the request.</param>
    /// <param name="command">The command indicating the action required.</param>
    /// <param name="schwabClientCustomerId">The client customer ID associated with the request.</param>
    /// <param name="schwabClientCorrelId">The correlation ID for tracking the request.</param>
    /// <param name="parameters">Optional additional parameters for the request.</param>
    protected internal StreamRequest(int requestId, Command command, string schwabClientCustomerId, string schwabClientCorrelId, Parameter parameters = null)
    {
        RequestId = requestId.ToStringInvariant();
        Command = command;
        SchwabClientCustomerId = schwabClientCustomerId;
        SchwabClientCorrelId = schwabClientCorrelId;
        Parameters = parameters;
    }
}

/// <summary>
/// Represents optional parameters for a streaming request.
/// </summary>
public class Parameter
{
    /// <summary>
    /// The access token required for authorization, if applicable.
    /// </summary>
    [JsonProperty("Authorization", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string AccessToken { get; }

    /// <summary>
    /// The client channel through which the request is sent, if applicable.
    /// </summary>
    [JsonProperty("SchwabClientChannel", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string SchwabClientChannel { get; }

    /// <summary>
    /// The client function identifier, used to specify the function of the request.
    /// </summary>
    [JsonProperty("SchwabClientFunctionId", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string SchwabClientFunctionId { get; }

    /// <summary>
    /// The keys that identify specific data or subscriptions, if applicable.
    /// </summary>
    [JsonProperty("keys", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Keys { get; }

    /// <summary>
    /// The fields specifying particular data elements to include in the response, if applicable.
    /// </summary>
    [JsonProperty("fields", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Fields { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Parameter"/> class with authorization and client channel details.
    /// </summary>
    /// <param name="accessToken">The access token required for authorization.</param>
    /// <param name="schwabClientChannel">The client channel identifier.</param>
    /// <param name="schwabClientFunctionId">The client function identifier.</param>
    public Parameter(string accessToken, string schwabClientChannel, string schwabClientFunctionId)
    {
        AccessToken = accessToken;
        SchwabClientChannel = schwabClientChannel;
        SchwabClientFunctionId = schwabClientFunctionId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Parameter"/> class with keys and fields.
    /// </summary>
    /// <param name="keys">The keys used to identify specific data or subscriptions.</param>
    /// <param name="fields">The fields specifying data elements to include in the response.</param>
    public Parameter(string keys, string fields)
    {
        Keys = keys;
        Fields = fields;
    }
}