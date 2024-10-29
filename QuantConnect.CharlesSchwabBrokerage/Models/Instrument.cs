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
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums;

namespace QuantConnect.Brokerages.CharlesSchwab.Models;

/// <summary>
/// Represents a financial instrument held in a position.
/// </summary>
/// <param name="AssetType">The type of asset (e.g., stock, bond) associated with the instrument.</param>
/// <param name="Cusip">The CUSIP (Committee on Uniform Securities Identification Procedures) identifier for the instrument, which uniquely identifies a financial security.</param>
/// <param name="Symbol">The ticker symbol of the instrument, representing the trading identifier used on exchanges.</param>
/// <param name="Description">A brief description of the instrument, providing additional context or details.</param>
/// <param name="InstrumentId">A unique identifier for the instrument, used for tracking and management purposes.</param>
/// <param name="NetChange">The net change in value of the instrument since the previous trading session, indicating the price movement.</param>
/// <param name="Type">The specific type of financial instrument (e.g., SWEEP_VEHICLE), indicating its functionality or category.</param>
public record Instrument(
    [JsonProperty("assetType")] AssetType AssetType,
    [JsonProperty("cusip")] string Cusip,
    [JsonProperty("symbol")] string Symbol,
    [JsonProperty("description")] string Description,
    [JsonProperty("instrumentId")] int InstrumentId,
    [JsonProperty("netChange")] decimal NetChange,
    [JsonProperty("type")] string Type
    );
