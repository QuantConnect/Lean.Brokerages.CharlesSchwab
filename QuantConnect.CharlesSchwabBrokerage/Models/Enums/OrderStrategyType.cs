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
/// Represents the different types of order strategies.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum OrderStrategyType
{
    /// <summary>
    /// No chaining, only a single order is submitted
    /// </summary>
    [EnumMember(Value = "SINGLE")]
    Single = 0,

    /// <summary>
    /// Cancels an existing order.
    /// </summary>
    [EnumMember(Value = "CANCEL")]
    Cancel = 1,

    /// <summary>
    /// Recalls an order that was previously submitted.
    /// </summary>
    [EnumMember(Value = "RECALL")]
    Recall = 2,

    /// <summary>
    /// Places a paired order.
    /// </summary>
    [EnumMember(Value = "PAIR")]
    Pair = 3,

    /// <summary>
    /// Flattens existing positions.
    /// </summary>
    [EnumMember(Value = "FLATTEN")]
    Flatten = 4,

    /// <summary>
    /// Executes a swap over two days.
    /// </summary>
    [EnumMember(Value = "TWO_DAY_SWAP")]
    TwoDaySwap = 5,

    /// <summary>
    /// Executes all orders in the batch.
    /// </summary>
    [EnumMember(Value = "BLAST_ALL")]
    BlastAll = 6,

    /// <summary>
    /// Execution of one order cancels the other
    /// </summary>
    [EnumMember(Value = "OCO")]
    Oco = 7,

    /// <summary>
    /// Execution of one order triggers placement of the other
    /// </summary>
    [EnumMember(Value = "TRIGGER")]
    Trigger = 8,
}
