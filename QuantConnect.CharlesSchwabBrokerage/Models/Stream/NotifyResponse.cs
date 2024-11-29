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

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Stream;

/// <summary>
/// Represents a response containing <b>notifications of heartbeats</b>.
/// </summary>
public class NotifyResponse : IStreamBaseResponse
{
    /// <summary>
    /// Gets the collection of heartbeat notifications.
    /// </summary>
    [property: JsonProperty("notify")]
    public IReadOnlyCollection<HeartbeatResponse> Notify { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyResponse"/> class.
    /// </summary>
    /// <param name="notify">A collection of <see cref="HeartbeatResponse"/> objects representing heartbeat notifications.</param>
    public NotifyResponse(IReadOnlyCollection<HeartbeatResponse> notify) => Notify = notify;
}

/// <summary>
/// Represents a single heartbeat notification.
/// </summary>
public readonly struct HeartbeatResponse
{
    /// <summary>
    /// Gets the timestamp or identifier of the heartbeat notification.
    /// </summary>
    [property: JsonProperty("heartbeat")]
    public string Heartbeat { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HeartbeatResponse"/> struct.
    /// </summary>
    /// <param name="heartbeat">The timestamp or identifier of the heartbeat notification.</param>

    public HeartbeatResponse(string heartbeat) => Heartbeat = heartbeat;
}
