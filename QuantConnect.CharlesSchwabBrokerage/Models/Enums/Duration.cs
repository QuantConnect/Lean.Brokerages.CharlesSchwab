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
/// Represents the duration of an order in the Charles Schwab trading system.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum Duration
{
    /// <summary>
    /// Order is valid for the day.
    /// </summary>
    [EnumMember(Value = "DAY")]
    Day = 0,

    /// <summary>
    /// Order remains valid until it is explicitly canceled by the user.
    /// </summary>
    [EnumMember(Value = "GOOD_TILL_CANCEL")]
    GoodTillCancel = 1,

    /// <summary>
    /// Order must be filled immediately in its entirety or it is canceled.
    /// </summary>
    [EnumMember(Value = "FILL_OR_KILL")]
    FillOrKill = 2,

    /// <summary>
    /// Order must be filled immediately, but partial fills are allowed.
    /// The remaining portion is canceled.
    /// </summary>
    [EnumMember(Value = "IMMEDIATE_OR_CANCEL")]
    ImmediateOrCancel = 3,

    /// <summary>
    /// Order is valid until the end of the current week.
    /// </summary>
    [EnumMember(Value = "END_OF_WEEK")]
    EndOfWeek = 4,

    /// <summary>
    /// Order is valid until the end of the current month.
    /// </summary>
    [EnumMember(Value = "END_OF_MONTH")]
    EndOfMonth = 5,

    /// <summary>
    /// Order is valid until the end of the next month.
    /// </summary>
    [EnumMember(Value = "NEXT_END_OF_MONTH")]
    NextEndOfMonth = 6,

    /// <summary>
    /// The duration of the order is unknown or unspecified.
    /// </summary>
    [EnumMember(Value = "UNKNOWN")]
    Unknown = 7
}
