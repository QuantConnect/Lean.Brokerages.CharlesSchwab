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

namespace QuantConnect.Brokerages.CharlesSchwab.Models;

/// <summary>
/// Represents a financial instrument held in a position.
/// </summary>
/// <param name="Cusip">The CUSIP identifier for the instrument.</param>
/// <param name="Symbol">The symbol of the instrument.</param>
/// <param name="Description">The description of the instrument.</param>
/// <param name="InstrumentId">The unique instrument identifier.</param>
/// <param name="NetChange">The net change in value of the instrument.</param>
/// <param name="Type">The type of the instrument (e.g., SWEEP_VEHICLE).</param>
public record Instrument(
    [property: JsonProperty("cusip")] string Cusip,
    [property: JsonProperty("symbol")] string Symbol,
    [property: JsonProperty("description")] string Description,
    [property: JsonProperty("instrumentId")] int InstrumentId,
    [property: JsonProperty("netChange")] decimal NetChange,
    [property: JsonProperty("type")] string Type
    );
