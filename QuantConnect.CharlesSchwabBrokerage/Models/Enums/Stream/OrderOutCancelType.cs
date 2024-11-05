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
using Newtonsoft.Json.Converters;

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Enums.Stream;

// <summary>
/// Specifies the reason for an order cancellation in stream responses.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum OrderOutCancelType
{
    /// <summary>
    /// Represents a system-initiated rejection of the order.
    /// This occurs when the order fails validation or other system checks.
    /// </summary>
    SystemReject = 0,

    /// <summary>
    /// Represents a client-initiated cancellation of the order.
    /// This occurs when the client manually cancels the order.
    /// </summary>
    ClientCancel = 1,
}