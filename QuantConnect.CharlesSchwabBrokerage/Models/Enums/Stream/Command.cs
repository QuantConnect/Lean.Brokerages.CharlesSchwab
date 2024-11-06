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

[JsonConverter(typeof(StringEnumConverter))]
public enum Command
{
    /// <summary>
    /// Initial request when opening a new connection. This must be successful before sending other commands.
    /// </summary>
    [EnumMember(Value = "LOGIN")]
    Login = 0,

    /// <summary>
    /// Subscribes to a set of symbols or keys for a particular service.
    /// This overwrites all previously subscribed symbols for that service.
    /// This is a convenient way to wipe out old subscription list and start fresh, but it's not the most efficient.
    /// If you only want to add one symbol to 300 already subscribed, use an ADD instead.
    /// </summary>
    /// <example>
    /// - SUBS A,B,C (fresh sub for LEVELONE_EQUITIES)
    /// - SUBS A (fresh sub for LEVELONE_EQUITIES, previous SUBS of B,C are unsub'ed, only A is sub'ed)
    /// </example>
    [EnumMember(Value = "SUBS")]
    Subscription = 1,

    /// <summary>
    /// Adds a new symbol for a particular service.
    /// This does NOT wipe out previous symbols that were already subscribed.
    /// It is OK to use ADD for first subscription command instead of SUBS.
    /// </summary>
    /// <example>
    /// - ADD A,B (fresh sub for LEVELONE_EQUITIES)
    /// - ADD C (additional symbol C added to A, B. All 3 symbols will stream)
    /// </example>
    [EnumMember(Value = "ADD")]
    Add = 2,

    /// <summary>
    /// Logs out of the streamer connection. Streamer will close the connection.
    /// </summary>
    [EnumMember(Value = "LOGOUT")]
    Logout = 3,
}
