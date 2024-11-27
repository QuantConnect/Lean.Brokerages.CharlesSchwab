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

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Stream;

/// <summary>
/// Represents the <b>response to a client request</b>, containing a collection of response data.
/// </summary>
/// <param name="Responses">A collection of <see cref="Response"/> objects representing individual service responses.</param>
public record StreamResponse(
    [property: JsonProperty("response")] IReadOnlyCollection<Response> Responses) : IStreamBaseResponse;

/// <summary>
/// Represents a single response within a <see cref="StreamResponse"/>.
/// </summary>
/// <param name="Service">The service providing this response.</param>
/// <param name="Command">The command issued by the service (e.g., update or status type).</param>
/// <param name="SchwabClientCorrelId">A client correlation identifier, allowing clients to match this response to a request.</param>
/// <param name="Timestamp">The Unix timestamp representing when the response was generated.</param>
/// <param name="Content">The <see cref="Content"/> associated with this response, containing status and message details.</param>
public record Response(
    [property: JsonProperty("service")] Service Service,
    [property: JsonProperty("command")] Command Command,
    [property: JsonProperty("SchwabClientCorrelId")] string SchwabClientCorrelId,
    [property: JsonProperty("timestamp")] long Timestamp,
    [property: JsonProperty("content")] Content Content
    );

/// <summary>
/// Represents the content of a response message, including status code and message details.
/// </summary>
/// <param name="Code">The status code associated with the response, indicating success, failure, or other status.</param>
/// <param name="Message">A message providing details about the response, such as error information or descriptive text.</param>
public record Content(
    [property: JsonProperty("code")] int Code,
    [property: JsonProperty("msg")] string Message);

