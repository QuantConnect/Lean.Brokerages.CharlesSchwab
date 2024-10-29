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
/// Represents the root object containing Charles Schwab securities account information.
/// </summary>
/// <param name="SecuritiesAccount">
/// Metadata of the securities account, including details such as account type, positions, and trading-related attributes.
/// </param>
/// <param name="AggregatedBalance">
/// The aggregated balance of the account, which includes liquidation values and other high-level account balance information.
/// </param>
public record CharlesSchwabSecuritiesAccount(
    [property: JsonProperty("securitiesAccount")] SecuritiesAccount SecuritiesAccount,
    [property: JsonProperty("aggregatedBalance")] AggregatedBalance AggregatedBalance
    );

/// <summary>
/// Represents aggregated balance information for a Charles Schwab securities account.
/// </summary>
/// <param name="CurrentLiquidationValue">The current liquidation value of the account, representing the immediate value if all holdings were sold.</param>
/// <param name="LiquidationValue">The overall liquidation value of the account, which may include adjustments or future expectations.</param>
public record AggregatedBalance(
    [property: JsonProperty("currentLiquidationValue")] decimal CurrentLiquidationValue,
    [property: JsonProperty("liquidationValue")] decimal LiquidationValue
    );

/// <summary>
/// Represents a securities account, including account metadata and balance details.
/// </summary>
/// <param name="Type">The type of the account, such as "CASH" or "MARGIN".</param>
/// <param name="AccountNumber">The unique identifier for the account.</param>
/// <param name="RoundTrips">The total number of round trips (buy and sell transactions completed within the same day).</param>
/// <param name="IsDayTrader">Indicates whether the account holder is classified as a day trader.</param>
/// <param name="IsClosingOnlyRestricted">Indicates whether the account is restricted to closing only (no new positions can be opened).</param>
/// <param name="PfcbFlag">Indicates if the Pattern Day Trader (PDT) flag is set on the account.</param>
/// <param name="Positions">A collection of financial positions (such as stocks, bonds, etc.) held in the account.</param>
/// <param name="InitialBalances">The initial balance details of the account, including available funds and margin values.</param>
/// <param name="CurrentBalances">The current balance details, such as available funds, buying power, and equity.</param>
/// <param name="ProjectedBalances">The projected balance details, including future buying power and maintenance calls.</param>
public record SecuritiesAccount(
    [JsonProperty("type")] string Type,
    [JsonProperty("accountNumber")] string AccountNumber,
    [JsonProperty("roundTrips")] int RoundTrips,
    [JsonProperty("isDayTrader")] bool IsDayTrader,
    [JsonProperty("isClosingOnlyRestricted")] bool IsClosingOnlyRestricted,
    [JsonProperty("pfcbFlag")] bool PfcbFlag,
    [JsonProperty("positions")] IReadOnlyCollection<Position> Positions,
    [JsonProperty("initialBalances")] InitialBalance InitialBalances,
    [JsonProperty("currentBalances")] CurrentBalance CurrentBalances,
    [JsonProperty("projectedBalances")] ProjectedBalance ProjectedBalances
    );

/// <summary>
/// Represents the balances of an account at different stages, including available funds, buying power, equity, and margin-related values.
/// </summary>
/// <param name="AccruedInterest">The amount of interest that has been accrued but not yet paid.</param>
/// <param name="AvailableFundsNonMarginableTrade">Available funds that can be used for non-marginable trades.</param>
/// <param name="BondValue">The total value of bonds held in the account.</param>
/// <param name="BuyingPower">The amount of buying power available in the account.</param>
/// <param name="CashBalance">The total cash balance in the account.</param>
/// <param name="CashAvailableForTrading">The amount of cash available for trading activities.</param>
/// <param name="CashReceipts">The total cash receipts in the account.</param>
/// <param name="DayTradingBuyingPower">The buying power available for day trading purposes.</param>
/// <param name="DayTradingBuyingPowerCall">The call amount related to day trading buying power.</param>
/// <param name="DayTradingEquityCall">The call amount related to day trading equity.</param>
/// <param name="Equity">The total equity in the account.</param>
/// <param name="EquityPercentage">The percentage of the account that is equity.</param>
/// <param name="LiquidationValue">The value of the account if all positions were liquidated.</param>
/// <param name="LongMarginValue">The margin value of long positions.</param>
/// <param name="LongOptionMarketValue">The market value of long options in the account.</param>
/// <param name="LongStockValue">The market value of long stock positions in the account.</param>
/// <param name="MaintenanceCall">The amount of maintenance call in the account.</param>
/// <param name="MaintenanceRequirement">The minimum maintenance requirement for the account.</param>
/// <param name="Margin">The total margin balance in the account.</param>
/// <param name="MarginEquity">The equity portion of the margin balance.</param>
/// <param name="MoneyMarketFund">The total value of the money market fund in the account.</param>
/// <param name="MutualFundValue">The total value of mutual fund holdings in the account.</param>
/// <param name="RegTCall">The Regulation T call amount for the account.</param>
/// <param name="ShortMarginValue">The margin value of short positions.</param>
/// <param name="ShortOptionMarketValue">The market value of short options in the account.</param>
/// <param name="ShortStockValue">The market value of short stock positions in the account.</param>
/// <param name="TotalCash">The total cash value in the account.</param>
/// <param name="IsInCall">Indicates whether the account is in a call status (typically due to margin or other obligations).</param>
/// <param name="PendingDeposits">The amount of pending deposits that have not yet cleared in the account.</param>
/// <param name="MarginBalance">The total margin balance in the account.</param>
/// <param name="ShortBalance">The total short balance in the account.</param>
/// <param name="AccountValue">The overall value of the account, including cash and positions.</param>
public record InitialBalance(
    [JsonProperty("accruedInterest")] decimal AccruedInterest,
    [JsonProperty("availableFundsNonMarginableTrade")] decimal AvailableFundsNonMarginableTrade,
    [JsonProperty("bondValue")] decimal BondValue,
    [JsonProperty("buyingPower")] decimal BuyingPower,
    [JsonProperty("cashBalance")] decimal CashBalance,
    [JsonProperty("cashAvailableForTrading")] decimal CashAvailableForTrading,
    [JsonProperty("cashReceipts")] decimal CashReceipts,
    [JsonProperty("dayTradingBuyingPower")] decimal DayTradingBuyingPower,
    [JsonProperty("dayTradingBuyingPowerCall")] decimal DayTradingBuyingPowerCall,
    [JsonProperty("dayTradingEquityCall")] decimal DayTradingEquityCall,
    [JsonProperty("equity")] decimal Equity,
    [JsonProperty("equityPercentage")] decimal EquityPercentage,
    [JsonProperty("liquidationValue")] decimal LiquidationValue,
    [JsonProperty("longMarginValue")] decimal LongMarginValue,
    [JsonProperty("longOptionMarketValue")] decimal LongOptionMarketValue,
    [JsonProperty("longStockValue")] decimal LongStockValue,
    [JsonProperty("maintenanceCall")] decimal MaintenanceCall,
    [JsonProperty("maintenanceRequirement")] decimal MaintenanceRequirement,
    [JsonProperty("margin")] decimal Margin,
    [JsonProperty("marginEquity")] decimal MarginEquity,
    [JsonProperty("moneyMarketFund")] decimal MoneyMarketFund,
    [JsonProperty("mutualFundValue")] decimal MutualFundValue,
    [JsonProperty("regTCall")] decimal RegTCall,
    [JsonProperty("shortMarginValue")] decimal ShortMarginValue,
    [JsonProperty("shortOptionMarketValue")] decimal ShortOptionMarketValue,
    [JsonProperty("shortStockValue")] decimal ShortStockValue,
    [JsonProperty("totalCash")] decimal TotalCash,
    [JsonProperty("isInCall")] bool IsInCall,
    [JsonProperty("pendingDeposits")] decimal PendingDeposits,
    [JsonProperty("marginBalance")] decimal MarginBalance,
    [JsonProperty("shortBalance")] decimal ShortBalance,
    [JsonProperty("accountValue")] decimal AccountValue
    );

/// <summary>
/// Represents the current balance details of an account, including various market values, available funds, and margin information.
/// </summary>
/// <param name="AccruedInterest">The amount of interest accrued but not yet paid.</param>
/// <param name="CashBalance">The total cash balance available in the account.</param>
/// <param name="CashReceipts">The total cash receipts in the account.</param>
/// <param name="LongOptionMarketValue">The market value of long options held in the account.</param>
/// <param name="LiquidationValue">The value of the account if all positions were liquidated.</param>
/// <param name="LongMarketValue">The total market value of long positions in the account.</param>
/// <param name="MoneyMarketFund">The total value of money market funds in the account.</param>
/// <param name="Savings">The amount saved in savings or similar accounts.</param>
/// <param name="ShortMarketValue">The total market value of short positions in the account.</param>
/// <param name="PendingDeposits">The total amount of deposits that are pending or not yet cleared.</param>
/// <param name="MutualFundValue">The total market value of mutual fund holdings in the account.</param>
/// <param name="BondValue">The total market value of bond holdings in the account.</param>
/// <param name="ShortOptionMarketValue">The market value of short options held in the account.</param>
/// <param name="AvailableFunds">The total funds currently available in the account for trading or withdrawal.</param>
/// <param name="AvailableFundsNonMarginableTrade">The amount of available funds for non-marginable trades.</param>
/// <param name="BuyingPower">The total buying power available in the account.</param>
/// <param name="BuyingPowerNonMarginableTrade">The buying power available for non-marginable trades.</param>
/// <param name="DayTradingBuyingPower">The buying power specifically available for day trading.</param>
/// <param name="Equity">The total equity in the account.</param>
/// <param name="EquityPercentage">The percentage of the account value that is equity.</param>
/// <param name="LongMarginValue">The margin value of long positions held in the account.</param>
/// <param name="MaintenanceCall">The total maintenance call amount in the account.</param>
/// <param name="MaintenanceRequirement">The minimum maintenance requirement for the account.</param>
/// <param name="MarginBalance">The total margin balance in the account.</param>
/// <param name="RegTCall">The Regulation T call amount in the account.</param>
/// <param name="ShortBalance">The short balance in the account.</param>
/// <param name="ShortMarginValue">The margin value of short positions held in the account.</param>
/// <param name="Sma">The Special Memorandum Account (SMA) value.</param>
public record CurrentBalance(
    [JsonProperty("accruedInterest")] decimal AccruedInterest,
    [JsonProperty("cashBalance")] decimal CashBalance,
    [JsonProperty("cashReceipts")] decimal CashReceipts,
    [JsonProperty("longOptionMarketValue")] decimal LongOptionMarketValue,
    [JsonProperty("liquidationValue")] decimal LiquidationValue,
    [JsonProperty("longMarketValue")] decimal LongMarketValue,
    [JsonProperty("moneyMarketFund")] decimal MoneyMarketFund,
    [JsonProperty("savings")] decimal Savings,
    [JsonProperty("shortMarketValue")] decimal ShortMarketValue,
    [JsonProperty("pendingDeposits")] decimal PendingDeposits,
    [JsonProperty("mutualFundValue")] decimal MutualFundValue,
    [JsonProperty("bondValue")] decimal BondValue,
    [JsonProperty("shortOptionMarketValue")] decimal ShortOptionMarketValue,
    [JsonProperty("availableFunds")] decimal AvailableFunds,
    [JsonProperty("availableFundsNonMarginableTrade")] decimal AvailableFundsNonMarginableTrade,
    [JsonProperty("buyingPower")] decimal BuyingPower,
    [JsonProperty("buyingPowerNonMarginableTrade")] decimal BuyingPowerNonMarginableTrade,
    [JsonProperty("dayTradingBuyingPower")] decimal DayTradingBuyingPower,
    [JsonProperty("equity")] decimal Equity,
    [JsonProperty("equityPercentage")] decimal EquityPercentage,
    [JsonProperty("longMarginValue")] decimal LongMarginValue,
    [JsonProperty("maintenanceCall")] decimal MaintenanceCall,
    [JsonProperty("maintenanceRequirement")] decimal MaintenanceRequirement,
    [JsonProperty("marginBalance")] decimal MarginBalance,
    [JsonProperty("regTCall")] decimal RegTCall,
    [JsonProperty("shortBalance")] decimal ShortBalance,
    [JsonProperty("shortMarginValue")] decimal ShortMarginValue,
    [JsonProperty("sma")] decimal Sma
    );

/// <summary>
/// Represents the projected balance of an account, including future buying power, available funds, and call amounts.
/// </summary>
/// <param name="AvailableFunds">The projected amount of funds that will be available for trading or withdrawal.</param>
/// <param name="AvailableFundsNonMarginableTrade">The projected amount of funds available for non-marginable trades.</param>
/// <param name="BuyingPower">The projected buying power available in the account.</param>
/// <param name="DayTradingBuyingPower">The projected buying power specifically available for day trading.</param>
/// <param name="DayTradingBuyingPowerCall">The projected call amount for day trading buying power.</param>
/// <param name="MaintenanceCall">The projected maintenance call amount in the account.</param>
/// <param name="RegTCall">The projected Regulation T call amount in the account.</param>
/// <param name="IsInCall">Indicates if the account is projected to be in a call status.</param>
/// <param name="StockBuyingPower">The projected buying power for stock purchases.</param>
public record ProjectedBalance(
    [JsonProperty("availableFunds")] decimal AvailableFunds,
    [JsonProperty("availableFundsNonMarginableTrade")] decimal AvailableFundsNonMarginableTrade,
    [JsonProperty("buyingPower")] decimal BuyingPower,
    [JsonProperty("dayTradingBuyingPower")] decimal DayTradingBuyingPower,
    [JsonProperty("dayTradingBuyingPowerCall")] decimal DayTradingBuyingPowerCall,
    [JsonProperty("maintenanceCall")] decimal MaintenanceCall,
    [JsonProperty("regTCall")] decimal RegTCall,
    [JsonProperty("isInCall")] bool IsInCall,
    [JsonProperty("stockBuyingPower")] decimal StockBuyingPower
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
    [property: JsonProperty("instrument")] Instrument Instrument,
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