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
public enum Service
{
    /// <summary>
    /// Represents administrative functions and operations.
    /// use for <see cref="Command.Login"/> or <seealso cref="Command.Logout"/>
    /// </summary>
    [EnumMember(Value = "ADMIN")]
    Admin = 0,

    /// <summary>
    /// Provides access to account activity details, including order fills and other account transactions.
    /// </summary>
    [EnumMember(Value = "ACCT_ACTIVITY")]
    Account = 1,

    /// <summary>
    /// Service for streaming Level 1 market data for equities.
    /// </summary>
    [EnumMember(Value = "LEVELONE_EQUITIES")]
    LevelOneEquities = 2,

    /// <summary>
    /// Service for streaming Level 1 market data for options.
    /// </summary>
    [EnumMember(Value = "LEVELONE_OPTIONS")]
    LevelOneOptions = 3,
}
