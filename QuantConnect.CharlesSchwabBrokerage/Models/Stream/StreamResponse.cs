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
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums.Stream;
using System.Text;

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Stream;

/// <summary>
/// Represents the response to a client request, containing a collection of response data.
/// </summary>
public sealed class StreamResponse : IStreamBaseResponse
{
    /// <summary>
    /// Gets a collection of <see cref="Response"/> objects representing individual service responses.
    /// </summary>
    [JsonProperty("response")]
    public IReadOnlyCollection<Response> Responses { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamResponse"/> class.
    /// </summary>
    /// <param name="responses">A collection of <see cref="Response"/> objects.</param>
    [JsonConstructor]
    public StreamResponse(IReadOnlyCollection<Response> responses) => Responses = responses;

    /// <summary>
    /// Returns a string representation of the <see cref="StreamResponse"/> object.
    /// </summary>
    /// <returns>A string representation of the <see cref="StreamResponse"/>.</returns>
    public override string ToString()
    {
        var str = new StringBuilder();
        str.AppendLine($"StreamResponse: {Responses.Count} response(s)");
        foreach (var response in Responses)
        {
            str.AppendLine(response.ToString());
        }
        return str.ToString();
    }
}

/// <summary>
/// Represents a single response within a <see cref="StreamResponse"/>.
/// </summary>
public sealed class Response
{
    /// <summary>
    /// Gets the service providing this response.
    /// </summary>
    public Service Service { get; }

    /// <summary>
    /// Gets the command issued by the service (e.g., update or status type).
    /// </summary>
    public Command Command { get; }

    /// <summary>
    /// Gets the client correlation identifier, allowing clients to match this response to a request.
    /// </summary>
    public string SchwabClientCorrelId { get; }

    /// <summary>
    /// Gets the Unix timestamp representing when the response was generated.
    /// </summary>
    public long Timestamp { get; }

    /// <summary>
    /// Gets the <see cref="Content"/> associated with this response, containing status and message details.
    /// </summary>
    public Content Content { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Response"/> class.
    /// </summary>
    /// <param name="service">The service providing this response.</param>
    /// <param name="command">The command issued by the service.</param>
    /// <param name="schwabClientCorrelId">The client correlation identifier.</param>
    /// <param name="timestamp">The Unix timestamp of the response generation.</param>
    /// <param name="content">The <see cref="Content"/> of the response.</param>
    [JsonConstructor]
    public Response(Service service, Command command, string schwabClientCorrelId, long timestamp, Content content)
    {
        Service = service;
        Command = command;
        SchwabClientCorrelId = schwabClientCorrelId;
        Timestamp = timestamp;
        Content = content;
    }

    /// <summary>
    /// Returns a string representation of the <see cref="Response"/> object.
    /// </summary>
    /// <returns>A string representation of the <see cref="Response"/>.</returns>
    public override string ToString()
    {
        return $"Response: Service={Service}, Command={Command}, SchwabClientCorrelId={SchwabClientCorrelId}, Timestamp={Timestamp}, Content=({Content})";
    }
}

/// <summary>
/// Represents the content of a response message, including status code and message details.
/// </summary>
public readonly struct Content
{
    /// <summary>
    /// Gets the status code associated with the response, indicating success, failure, or other status.
    /// </summary>
    public int Code { get; }

    /// <summary>
    /// Gets a message providing details about the response, such as error information or descriptive text.
    /// </summary>
    [JsonProperty("msg")]
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Content"/> struct.
    /// </summary>
    /// <param name="code">The status code associated with the response.</param>
    /// <param name="message">The message providing details about the response.</param>
    [JsonConstructor]
    public Content(int code, string message) => (Code, Message) = (code, message);

    /// <summary>
    /// Returns a string representation of the <see cref="Content"/> struct.
    /// </summary>
    /// <returns>A string representation of the <see cref="Content"/>.</returns>
    public override string ToString()
    {
        return $"Content: Code={Code}, Message={Message}";
    }
}

