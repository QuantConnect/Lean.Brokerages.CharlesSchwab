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

namespace QuantConnect.Brokerages.CharlesSchwab.Models;

/// <summary>
/// Represents the root object for Charles Schwab securities account information.
/// </summary>
/// <param name="SecuritiesAccount">Metadata of the securities account.</param>
public record CharlesSchwabSecuritiesAccount(
    [property: JsonProperty("securitiesAccount")] SecuritiesAccountMetaData SecuritiesAccount
    );

/// <summary>
/// Represents a securities account.
/// </summary>
/// <param name="Type">The type of the account (e.g., "CASH").</param>
/// <param name="AccountNumber">The account number.</param>
/// <param name="RoundTrips">The number of round trips.</param>
/// <param name="IsDayTrader">A value indicating whether the account is marked as a day trader.</param>
/// <param name="IsClosingOnlyRestricted">A value indicating whether the account is restricted to closing only.</param>
/// <param name="PfcbFlag">A value indicating whether the PFCB (Pattern Day Trader) flag is set.</param>
/// <param name="Positions">The positions associated with the securities account.</param>
public record SecuritiesAccountMetaData(
    [property: JsonProperty("type")] string Type,
    [property: JsonProperty("accountNumber")] string AccountNumber,
    [property: JsonProperty("roundTrips")] int RoundTrips,
    [property: JsonProperty("isDayTrader")] bool IsDayTrader,
    [property: JsonProperty("isClosingOnlyRestricted")] bool IsClosingOnlyRestricted,
    [property: JsonProperty("pfcbFlag")] bool PfcbFlag,
    [property: JsonProperty("positions")] IReadOnlyCollection<Position> Positions
    );

/// <summary>
/// Represents a position held in the securities account.
/// </summary>
/// <param name="ShortQuantity">The short quantity held in the position.</param>
/// <param name="AveragePrice">The average price of the position.</param>
/// <param name="CurrentDayProfitLoss">The profit or loss for the current day.</param>
/// <param name="CurrentDayProfitLossPercentage">The percentage profit or loss for the current day.</param>
/// <param name="LongQuantity">The long quantity held in the position.</param>
/// <param name="SettledLongQuantity">The settled long quantity in the position.</param>
/// <param name="SettledShortQuantity">The settled short quantity in the position.</param>
/// <param name="AgedQuantity">The aged quantity of the position.</param>
/// <param name="Instrument">The financial instrument associated with this position.</param>
/// <param name="MarketValue">The market value of the position.</param>
/// <param name="MaintenanceRequirement">The maintenance requirement for the position.</param>
/// <param name="AverageLongPrice">The average price for the long position.</param>
/// <param name="AverageShortPrice">The average price for the short position.</param>
/// <param name="TaxLotAverageLongPrice">The tax lot average price for the long position.</param>
/// <param name="TaxLotAverageShortPrice">The tax lot average price for the short position.</param>
/// <param name="LongOpenProfitLoss">The open profit or loss for the long position.</param>
/// <param name="ShortOpenProfitLoss">The open profit or loss for the short position.</param>
/// <param name="PreviousSessionLongQuantity">The previous session's long quantity.</param>
/// <param name="PreviousSessionShortQuantity">The previous session's short quantity.</param>
/// <param name="CurrentDayCost">The cost incurred for the position on the current day.</param>
public record Position(
    [property: JsonProperty("shortQuantity")] int ShortQuantity,
    [property: JsonProperty("averagePrice")] decimal AveragePrice,
    [property: JsonProperty("currentDayProfitLoss")] decimal CurrentDayProfitLoss,
    [property: JsonProperty("currentDayProfitLossPercentage")] decimal CurrentDayProfitLossPercentage,
    [property: JsonProperty("longQuantity")] int LongQuantity,
    [property: JsonProperty("settledLongQuantity")] int SettledLongQuantity,
    [property: JsonProperty("settledShortQuantity")] int SettledShortQuantity,
    [property: JsonProperty("agedQuantity")] int AgedQuantity,
    [property: JsonProperty("instrument")] CharlesSchwabInstrument Instrument,
    [property: JsonProperty("marketValue")] decimal MarketValue,
    [property: JsonProperty("maintenanceRequirement")] decimal MaintenanceRequirement,
    [property: JsonProperty("averageLongPrice")] decimal AverageLongPrice,
    [property: JsonProperty("averageShortPrice")] decimal AverageShortPrice,
    [property: JsonProperty("taxLotAverageLongPrice")] decimal TaxLotAverageLongPrice,
    [property: JsonProperty("taxLotAverageShortPrice")] decimal TaxLotAverageShortPrice,
    [property: JsonProperty("longOpenProfitLoss")] decimal LongOpenProfitLoss,
    [property: JsonProperty("shortOpenProfitLoss")] decimal ShortOpenProfitLoss,
    [property: JsonProperty("previousSessionLongQuantity")] int PreviousSessionLongQuantity,
    [property: JsonProperty("previousSessionShortQuantity")] int PreviousSessionShortQuantity,
    [property: JsonProperty("currentDayCost")] decimal CurrentDayCost
    );