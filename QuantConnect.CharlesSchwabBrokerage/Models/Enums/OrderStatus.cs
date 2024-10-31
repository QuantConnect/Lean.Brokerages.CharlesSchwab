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

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Enums;

/// <summary>
/// Represents the status of an order in the Charles Schwab trading system.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum OrderStatus
{
    /// <summary>
    /// The order is waiting for its parent order to be executed or completed.
    /// </summary>
    [EnumMember(Value = "AWAITING_PARENT_ORDER")]
    AwaitingParentOrder = 0,

    /// <summary>
    /// The order is waiting for specific conditions, such as a limit or trigger, to be met.
    /// </summary>
    [EnumMember(Value = "AWAITING_CONDITION")]
    AwaitingCondition = 1,

    /// <summary>
    /// The order is waiting for a stop condition, typically related to a stop-loss order.
    /// </summary>
    [EnumMember(Value = "AWAITING_STOP_CONDITION")]
    AwaitingStopCondition = 2,

    /// <summary>
    /// The order is awaiting manual review by a human or a system.
    /// </summary>
    [EnumMember(Value = "AWAITING_MANUAL_REVIEW")]
    AwaitingManualReview = 3,

    /// <summary>
    /// The order has been accepted by the trading system but is not yet executed.
    /// </summary>
    [EnumMember(Value = "ACCEPTED")]
    Accepted = 4,

    /// <summary>
    /// The order is waiting for an under review (UR) outcome.
    /// </summary>
    [EnumMember(Value = "AWAITING_UR_OUT")]
    AwaitingUrOut = 5,

    /// <summary>
    /// The order is pending activation, waiting for certain conditions or a specific time to activate.
    /// </summary>
    [EnumMember(Value = "PENDING_ACTIVATION")]
    PendingActivation = 6,

    /// <summary>
    /// The order has been queued in the trading system and is waiting to be processed.
    /// </summary>
    [EnumMember(Value = "QUEUED")]
    Queued = 7,

    /// <summary>
    /// The order is currently being worked on or partially filled.
    /// </summary>
    [EnumMember(Value = "WORKING")]
    Working = 8,

    /// <summary>
    /// The order has been rejected by the system and will not be executed.
    /// </summary>
    [EnumMember(Value = "REJECTED")]
    Rejected = 9,

    /// <summary>
    /// The cancellation of the order is pending, but not yet confirmed.
    /// </summary>
    [EnumMember(Value = "PENDING_CANCEL")]
    PendingCancel = 10,

    /// <summary>
    /// The order has been canceled and will not be executed.
    /// </summary>
    [EnumMember(Value = "CANCELED")]
    Canceled = 11,

    /// <summary>
    /// The order is pending replacement, often after a modification request.
    /// </summary>
    [EnumMember(Value = "PENDING_REPLACE")]
    PendingReplace = 12,

    /// <summary>
    /// The order has been replaced with a modified version.
    /// </summary>
    [EnumMember(Value = "REPLACED")]
    Replaced = 13,

    /// <summary>
    /// The order has been fully executed (filled).
    /// </summary>
    [EnumMember(Value = "FILLED")]
    Filled = 14,

    /// <summary>
    /// The order has expired and will not be executed.
    /// </summary>
    [EnumMember(Value = "EXPIRED")]
    Expired = 15,

    /// <summary>
    /// A new order that has been entered into the system but not yet processed.
    /// </summary>
    [EnumMember(Value = "NEW")]
    New = 16,

    /// <summary>
    /// The order is waiting for a release time before it can be processed.
    /// </summary>
    [EnumMember(Value = "AWAITING_RELEASE_TIME")]
    AwaitingReleaseTime = 17,

    /// <summary>
    /// The order is pending acknowledgement from the system.
    /// </summary>
    [EnumMember(Value = "PENDING_ACKNOWLEDGEMENT")]
    PendingAcknowledgement = 18,

    /// <summary>
    /// The order is pending recall, typically in situations of administrative or regulatory concern.
    /// </summary>
    [EnumMember(Value = "PENDING_RECALL")]
    PendingRecall = 19,

    /// <summary>
    /// The status of the order is unknown.
    /// </summary>
    [EnumMember(Value = "UNKNOWN")]
    Unknown = 20
}
