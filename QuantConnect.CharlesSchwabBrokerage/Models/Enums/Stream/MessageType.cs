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
using System.Runtime.Serialization;

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Enums.Stream;

/// <summary>
/// Specifies the type of message received in response to various order and execution events.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum MessageType
{
    /// <summary>
    /// Indicates that the subscription to a data feed or event stream was successful.
    /// </summary>
    [EnumMember(Value = "SUBSCRIBED")]
    Subscribed = 0,

    /// <summary>
    /// Indicates that a new order has been created and submitted for processing.
    /// </summary>
    OrderCreated = 1,

    /// <summary>
    /// Indicates that the order has been accepted by the trading system and is now active.
    /// </summary>
    OrderAccepted = 2,

    /// <summary>
    /// Indicates a request to execute an order has been made.
    /// </summary>
    ExecutionRequested = 3,

    /// <summary>
    /// Indicates that an execution request has been successfully created in the system.
    /// </summary>
    ExecutionRequestCreated = 4,

    /// <summary>
    /// Indicates that an execution request has been completed successfully.
    /// </summary>
    ExecutionRequestCompleted = 5,

    /// <summary>
    /// Indicates that an execution (i.e., trade) has been successfully created for the order.
    /// </summary>
    ExecutionCreated = 6,

    /// <summary>
    /// Indicates that a cancellation request for the order has been accepted by the system.
    /// </summary>
    CancelAccepted = 7,

    /// <summary>
    /// Indicates that an order update (UROut) process has been completed.
    /// </summary>
    OrderUROutCompleted = 8,

    /// <summary>
    /// Indicates that an order has been filled, completing the order fill process.
    /// </summary>
    OrderFillCompleted = 9,

    /// <summary>
    /// Indicates that a change request for an order has been created in the system.
    /// </summary>
    ChangeCreated = 10,

    /// <summary>
    /// Indicates that a change request for an order has been accepted by the system.
    /// </summary>
    ChangeAccepted = 11,
}