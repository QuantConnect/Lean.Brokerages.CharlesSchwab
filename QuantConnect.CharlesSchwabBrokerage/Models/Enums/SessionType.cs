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
/// Defines the various session types for Charles Schwab trading.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum SessionType
{
    /// <summary>
    /// The normal session type.
    /// 9:30 a.m. ET – 4 p.m. ET
    /// </summary>
    [EnumMember(Value = "NORMAL")]
    Normal = 0,

    /// <summary>
    /// The AM session type.
    /// Extended A.M.7:00 a.m. ET - 9:25 a.m. ET
    /// </summary>
    [EnumMember(Value = "AM")]
    Am = 1,

    /// <summary>
    /// The PM session type.
    /// Extended P.M.: 4:05 p.m. ET - 8:00 p.m. ET
    /// </summary>
    [EnumMember(Value = "PM")]
    Pm = 2,

    /// <summary>
    /// The seamless session type, used for continuous trading across sessions.
    /// 7 a.m. ET – 8 p.m. ET 
    /// </summary>
    [EnumMember(Value = "SEAMLESS")]
    Seamless = 3
}
