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
/// Represents the basis for a stop price link.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum StopPriceLinkBasis
{
    /// <summary>
    /// A manual basis for the stop price.
    /// </summary>
    [EnumMember(Value = "MANUAL")]
    Manual = 0,

    /// <summary>
    /// The stop price is based on a base price.
    /// </summary>
    [EnumMember(Value = "BASE")]
    Base = 1,

    /// <summary>
    /// The stop price is based on a trigger price.
    /// </summary>
    [EnumMember(Value = "TRIGGER")]
    Trigger = 2,

    /// <summary>
    /// The stop price is based on the last price.
    /// </summary>
    [EnumMember(Value = "LAST")]
    Last = 3,

    /// <summary>
    /// The stop price is based on the bid price.
    /// </summary>
    [EnumMember(Value = "BID")]
    Bid = 4,

    /// <summary>
    /// The stop price is based on the ask price.
    /// </summary>
    [EnumMember(Value = "ASK")]
    Ask = 5,

    /// <summary>
    /// The stop price is based on both the ask and bid prices.
    /// </summary>
    [EnumMember(Value = "ASK_BID")]
    AskBid = 6,

    /// <summary>
    /// The stop price is based on the mark price.
    /// </summary>
    [EnumMember(Value = "MARK")]
    Mark = 7,

    /// <summary>
    /// The stop price is based on the average price.
    /// </summary>
    [EnumMember(Value = "AVERAGE")]
    Average = 8,
}
