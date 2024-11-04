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
/// Represents a response containing <b>streaming market data</b>.
/// </summary>
/// <param name="Data">A collection of <see cref="Data"/> objects that contain market data details.</param>
public record DataResponse([property: JsonProperty("data")] IReadOnlyCollection<Data> Data) : IStreamBaseResponse;

/// <summary>
/// Represents market data with details about the service, timestamp, command, and account content.
/// </summary>
/// <param name="Service">The type of service providing the market data.</param>
/// <param name="Timestamp">The Unix timestamp representing when the data was generated.</param>
/// <param name="Command">The command associated with the data, such as an update or response type.</param>
/// <param name="Content">A collection of <see cref="AccountContent"/> objects containing account-specific details.</param>
public record Data(
    [property: JsonProperty("service")] Service Service,
    [property: JsonProperty("timestamp")] long Timestamp,
    [property: JsonProperty("command")] Command Command,
    [property: JsonProperty("content")] IReadOnlyCollection<AccountContent> Content);

/// <summary>
/// Represents the content of an account-related message.
/// </summary>
/// <param name="Sequence">This field identifies the message number. If client reconnects and receives the same seq number again, it can choose to ignore the duplicate.</param>
/// <param name="Key">Passed back to the client from the request to identify a subscription this response belongs to.</param>
/// <param name="Account">Account Number that the activity occurred on.</param>
/// <param name="MessageType">Message Type that dictates the format of the Message Data field.</param>
/// <param name="MessageData">The core data for the message. Either JSON-formatted data describing the update, NULL in some cases, or plain text in case of ERROR.</param>
public record AccountContent(
    [property: JsonProperty("seq")] int Sequence,
    [property: JsonProperty("key")] string Key,
    [property: JsonProperty("1")] string Account,
    [property: JsonProperty("2")] string MessageType,
    [property: JsonProperty("3")] string MessageData);
