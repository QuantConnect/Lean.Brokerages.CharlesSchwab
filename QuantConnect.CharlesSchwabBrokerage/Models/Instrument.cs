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
using System.Collections.Generic;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums;

namespace QuantConnect.Brokerages.CharlesSchwab.Models;

/// <summary>
/// Represents a financial instrument held in a position.
/// </summary>
public class Instrument
{
    /// <summary>
    /// The type of asset (e.g., stock, bond) associated with the instrument.
    /// </summary>
    public AssetType AssetType { get; }

    /// <summary>
    /// The CUSIP (Committee on Uniform Securities Identification Procedures) identifier for the instrument, which uniquely identifies a financial security.
    /// </summary>
    public string Cusip { get; }

    /// <summary>
    /// The ticker symbol of the instrument, representing the trading identifier used on exchanges.
    /// </summary>
    public string Symbol { get; }

    /// <summary>
    /// A brief description of the instrument, providing additional context or details.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// A unique identifier for the instrument, used for tracking and management purposes.
    /// </summary>
    public int InstrumentId { get; }

    /// <summary>
    /// The net change in value of the instrument since the previous trading session, indicating the price movement.
    /// </summary>
    public decimal NetChange { get; }

    /// <summary>
    /// The specific type of financial instrument (e.g., SWEEP_VEHICLE), indicating its functionality or category.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// The collection of option deliverables associated with this instrument.
    /// </summary>
    public IReadOnlyCollection<OptionDeliverables> OptionDeliverables { get; }

    /// <summary>
    /// Gets the symbol representing the underlying asset for this instrument.
    /// The underlying symbol typically refers to the main security or asset associated with an option or derivative.
    /// </summary>
    public string UnderlyingSymbol { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Instrument"/> class with the specified parameters.
    /// </summary>
    /// <param name="assetType">The type of asset (e.g., stock, bond) associated with the instrument.</param>
    /// <param name="cusip">The CUSIP identifier for the instrument, which uniquely identifies the financial security.</param>
    /// <param name="symbol">The ticker symbol of the instrument used in trading.</param>
    /// <param name="description">A brief description providing context for the instrument.</param>
    /// <param name="instrumentId">A unique identifier used for tracking and managing the instrument.</param>
    /// <param name="netChange">The net change in value of the instrument since the previous trading session.</param>
    /// <param name="type">The specific type of financial instrument (e.g., SWEEP_VEHICLE), indicating its category or functionality.</param>
    /// <param name="optionDeliverables">The collection of option deliverables associated with this instrument.</param>
    /// <param name="underlyingSymbol">The symbol representing the underlying asset for the instrument, typically used in options or derivative products.</param>
    [JsonConstructor]
    public Instrument(AssetType assetType, string cusip, string symbol, string description, int instrumentId, decimal netChange, string type,
        IReadOnlyCollection<OptionDeliverables> optionDeliverables, string underlyingSymbol)
        => (AssetType, Cusip, Symbol, Description, InstrumentId, NetChange, Type, OptionDeliverables, UnderlyingSymbol) = (assetType, cusip, symbol, description, instrumentId, netChange, type, optionDeliverables, underlyingSymbol);
}
