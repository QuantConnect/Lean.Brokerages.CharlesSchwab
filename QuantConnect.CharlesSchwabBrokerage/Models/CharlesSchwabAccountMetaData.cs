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
/// Represents a account metadata with various details about balances and positions.
/// </summary>
/// <param name="AccountNumber">The account number.</param>
/// <param name="RoundTrips">The number of round trips (day trades) made within a specific period.</param>
/// <param name="IsDayTrader">Indicates if the account holder is classified as a day trader.</param>
/// <param name="IsClosingOnlyRestricted">Indicates if the account is in closing-only status.</param>
/// <param name="pfcbFlag">Indicates if the account is flagged for PFCB (Pattern Day Trading Flag).</param>
/// <param name="Positions">A list of positions held by account number.</param>
/// <param name="InitialBalances">Initial balances for the account.</param>
/// <param name="CurrentBalances">Current balances for the account.</param>
/// <param name="ProjectedBalances">Projected balances for the account.</param>
public record CharlesSchwabAccountMetaData(
    [property: JsonProperty("accountNumber")] string AccountNumber,
    [property: JsonProperty("roundTrips")] int RoundTrips,
    [property: JsonProperty("isDayTrader")] bool IsDayTrader,
    [property: JsonProperty("isClosingOnlyRestricted")] bool IsClosingOnlyRestricted,
    [property: JsonProperty("pfcbFlag")] bool pfcbFlag,
    [property: JsonProperty("positions")] IReadOnlyCollection<Position> Positions,
    [property: JsonProperty("initialBalances")] InitialBalance InitialBalances,
    [property: JsonProperty("currentBalances")] TradingPowerSummary CurrentBalances,
    [property: JsonProperty("projectedBalances")] TradingPowerSummary ProjectedBalances
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

/// <summary>
/// Represents the balances of a account at different stages.
/// </summary>
public class Balance
{
    /// <summary>
    /// Available funds for non-marginable trades.
    /// </summary>
    [JsonProperty("availableFundsNonMarginableTrade")]
    public decimal AvailableFundsNonMarginableTrade { get; }

    /// <summary>
    /// The buying power available in the account.
    /// </summary>
    [JsonProperty("buyingPower")]
    public decimal BuyingPower { get; }

    /// <summary>
    /// Day trading buying power available.
    /// </summary>
    [JsonProperty("dayTradingBuyingPower")]
    public decimal DayTradingBuyingPower { get; }

    /// <summary>
    /// The call amount for day trading buying power.
    /// </summary>

    [JsonProperty("dayTradingBuyingPowerCall")]
    public decimal DayTradingBuyingPowerCall { get; }

    /// <summary>
    /// The total equity in the account.
    /// </summary>
    [JsonProperty("equity")]
    public decimal Equity { get; }

    /// <summary>
    /// The percentage of equity in the account.
    /// </summary>
    [JsonProperty("equityPercentage")]
    public decimal EquityPercentage { get; }

    /// <summary>
    /// The long margin value in the account.
    /// </summary>
    [JsonProperty("longMarginValue")]
    public decimal LongMarginValue { get; }

    /// <summary>
    /// The maintenance call amount in the account.
    /// </summary>
    [JsonProperty("maintenanceCall")]
    public decimal MaintenanceCall { get; }

    /// <summary>
    /// The maintenance requirement in the account.
    /// </summary>
    [JsonProperty("maintenanceRequirement")]
    public decimal MaintenanceRequirement { get; }

    /// <summary>
    /// The call amount for Regulation T.
    /// </summary>
    [JsonProperty("regTCall")]
    public decimal RegTCall { get; set; }

    /// <summary>
    /// The short margin value in the account.
    /// </summary>
    [JsonProperty("shortMarginValue")]
    public decimal ShortMarginValue { get; }

    /// <summary>
    /// Indicates if the account is in a call status.
    /// </summary>
    public bool IsInCall { get; set; }

    /// <summary>
    /// The margin balance in the account.
    /// </summary>
    [JsonProperty("marginBalance")]
    public decimal MarginBalance { get; }

    /// <summary>
    /// The short balance in the account.
    /// </summary>
    [JsonProperty("shortBalance")]
    public decimal ShortBalance { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Balance"/> class.
    /// </summary>
    /// <param name="availableFundsNonMarginableTrade">The available funds for non-marginable trades.</param>
    /// <param name="buyingPower">The buying power available in the account.</param>
    /// <param name="dayTradingBuyingPower">The day trading buying power available.</param>
    /// <param name="dayTradingBuyingPowerCall">The call amount for day trading buying power.</param>
    /// <param name="equity">The total equity in the account.</param>
    /// <param name="equityPercentage">The percentage of equity in the account.</param>
    /// <param name="longMarginValue">The long margin value in the account.</param>
    /// <param name="maintenanceCall">The maintenance call amount in the account.</param>
    /// <param name="maintenanceRequirement">The maintenance requirement in the account.</param>
    /// <param name="regTCall">The call amount for Regulation T.</param>
    /// <param name="shortMarginValue">The short margin value in the account.</param>
    /// <param name="isInCall">Indicates if the account is in call status.</param>
    /// <param name="marginBalance">The margin balance in the account.</param>
    /// <param name="shortBalance">The short balance in the account.</param>
    public Balance(decimal availableFundsNonMarginableTrade, decimal buyingPower, decimal dayTradingBuyingPower, decimal dayTradingBuyingPowerCall, decimal equity, decimal equityPercentage,
        decimal longMarginValue, decimal maintenanceCall, decimal maintenanceRequirement, decimal regTCall, decimal shortMarginValue, bool isInCall, decimal marginBalance, decimal shortBalance)
    {
        AvailableFundsNonMarginableTrade = availableFundsNonMarginableTrade;
        BuyingPower = buyingPower;
        DayTradingBuyingPower = dayTradingBuyingPower;
        DayTradingBuyingPowerCall = dayTradingBuyingPowerCall;
        Equity = equity;
        EquityPercentage = equityPercentage;
        LongMarginValue = longMarginValue;
        MaintenanceCall = maintenanceCall;
        MaintenanceRequirement = maintenanceRequirement;
        RegTCall = regTCall;
        ShortMarginValue = shortMarginValue;
        IsInCall = isInCall;
        MarginBalance = marginBalance;
        ShortBalance = shortBalance;
    }
}

/// <summary>
/// Represents the initial balances of an account, including additional properties like accrued interest and bond value.
/// </summary>
public class InitialBalance : Balance
{
    /// <summary>
    /// Accrued interest in the account.
    /// </summary>
    [JsonProperty("accruedInterest")]
    public decimal AccruedInterest { get; }

    /// <summary>
    /// The bond value held in the account.
    /// </summary>
    [JsonProperty("bondValue")]
    public decimal BondValue { get; }

    /// <summary>
    /// The cash balance available in the account.
    /// </summary>
    [JsonProperty("cashBalance")]
    public decimal CashBalance { get; }

    /// <summary>
    /// The cash available for trading in the account.
    /// </summary>
    [JsonProperty("cashAvailableForTrading")]
    public decimal CashAvailableForTrading { get; }

    /// <summary>
    /// The total cash receipts in the account.
    /// </summary>
    [JsonProperty("cashReceipts")]
    public decimal CashReceipts { get; }

    /// <summary>
    /// The equity call for day trading.
    /// </summary>
    [JsonProperty("dayTradingEquityCall")]
    public decimal DayTradingEquityCall { get; }

    /// <summary>
    /// The liquidation value of the account.
    /// </summary>
    [JsonProperty("liquidationValue")]
    public decimal LiquidationValue { get; }

    /// <summary>
    /// The long option market value in the account.
    /// </summary>
    [JsonProperty("longOptionMarketValue")]
    public decimal LongOptionMarketValue { get; }

    /// <summary>
    /// The long stock value in the account.
    /// </summary>
    [JsonProperty("longStockValue")]
    public decimal LongStockValue { get; }

    /// <summary>
    /// The margin available in the account.
    /// </summary>
    [JsonProperty("margin")]
    public decimal Margin { get; }

    /// <summary>
    /// The margin equity available in the account.
    /// </summary>
    [JsonProperty("marginEquity")]
    public decimal MarginEquity { get; }

    /// <summary>
    /// The value of money market funds in the account.
    /// </summary>
    [JsonProperty("moneyMarketFund")]
    public decimal MoneyMarketFund { get; }

    /// <summary>
    /// The value of mutual funds held in the account.
    /// </summary>
    [JsonProperty("mutualFundValue")]
    public decimal MutualFundValue { get; }

    /// <summary>
    /// The short option market value in the account.
    /// </summary>
    [JsonProperty("shortOptionMarketValue")]
    public decimal ShortOptionMarketValue { get; }

    /// <summary>
    /// The short stock value in the account.
    /// </summary>
    [JsonProperty("shortStockValue")]
    public decimal ShortStockValue { get; }

    /// <summary>
    /// The total cash available in the account.
    /// </summary>
    [JsonProperty("totalCash")]
    public decimal TotalCash { get; }

    /// <summary>
    /// The unsettled cash in the account.
    /// </summary>
    [JsonProperty("unsettledCash")]
    public decimal UnsettledCash { get; }

    /// <summary>
    /// The amount of pending deposits in the account.
    /// </summary>
    [JsonProperty("pendingDeposits")]
    public decimal PendingDeposits { get; }

    /// <summary>
    /// The total account value.
    /// </summary>
    [JsonProperty("accountValue")]
    public decimal AccountValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InitialBalance"/> class.
    /// </summary>
    /// <param name="accruedInterest">The accrued interest in the account.</param>
    /// <param name="bondValue">The bond value held in the account.</param>
    /// <param name="cashBalance">The cash balance available in the account.</param>
    /// <param name="cashAvailableForTrading">The cash available for trading in the account.</param>
    /// <param name="cashReceipts">The total cash receipts in the account.</param>
    /// <param name="dayTradingEquityCall">The equity call for day trading.</param>
    /// <param name="liquidationValue">The liquidation value of the account.</param>
    /// <param name="longOptionMarketValue">The long option market value in the account.</param>
    /// <param name="longStockValue">The long stock value in the account.</param>
    /// <param name="margin">The margin available in the account.</param>
    /// <param name="marginEquity">The margin equity available in the account.</param>
    /// <param name="moneyMarketFund">The value of money market funds in the account.</param>
    /// <param name="mutualFundValue">The value of mutual funds held in the account.</param>
    /// <param name="shortOptionMarketValue">The short option market value in the account.</param>
    /// <param name="shortStockValue">The short stock value in the account.</param>
    /// <param name="totalCash">The total cash available in the account.</param>
    /// <param name="unsettledCash">The unsettled cash in the account.</param>
    /// <param name="pendingDeposits">The amount of pending deposits in the account.</param>
    /// <param name="accountValue">The total account value.</param>
    /// <param name="availableFundsNonMarginableTrade">The available funds for non-marginable trades.</param>
    /// <param name="buyingPower">The buying power available in the account.</param>
    /// <param name="dayTradingBuyingPower">The day trading buying power available.</param>
    /// <param name="dayTradingBuyingPowerCall">The call amount for day trading buying power.</param>
    /// <param name="equity">The total equity in the account.</param>
    /// <param name="equityPercentage">The percentage of equity in the account.</param>
    /// <param name="longMarginValue">The long margin value in the account.</param>
    /// <param name="maintenanceCall">The maintenance call amount in the account.</param>
    /// <param name="maintenanceRequirement">The maintenance requirement in the account.</param>
    /// <param name="regTCall">The call amount for Regulation T.</param>
    /// <param name="shortMarginValue">The short margin value in the account.</param>
    /// <param name="isInCall">Indicates if the account is in call status.</param>
    /// <param name="marginBalance">The margin balance in the account.</param>
    /// <param name="shortBalance">The short balance in the account.</param>
    public InitialBalance(decimal accruedInterest, decimal bondValue, decimal cashBalance, decimal cashAvailableForTrading, decimal cashReceipts, decimal dayTradingEquityCall, 
        decimal liquidationValue, decimal longOptionMarketValue, decimal longStockValue, decimal margin, decimal marginEquity, decimal moneyMarketFund, decimal mutualFundValue,
        decimal shortOptionMarketValue, decimal shortStockValue, decimal totalCash, decimal unsettledCash, decimal pendingDeposits, decimal accountValue,
        decimal availableFundsNonMarginableTrade, decimal buyingPower, decimal dayTradingBuyingPower, decimal dayTradingBuyingPowerCall, decimal equity, decimal equityPercentage,
        decimal longMarginValue, decimal maintenanceCall, decimal maintenanceRequirement, decimal regTCall, decimal shortMarginValue, bool isInCall, decimal marginBalance, decimal shortBalance)
        : base(availableFundsNonMarginableTrade, buyingPower, dayTradingBuyingPower, dayTradingBuyingPowerCall, equity, equityPercentage, longMarginValue, maintenanceCall, maintenanceRequirement,
            regTCall, shortMarginValue, isInCall, marginBalance, shortBalance)
    {
        AccruedInterest = accruedInterest;
        BondValue = bondValue;
        CashBalance = cashBalance;
        CashAvailableForTrading = cashAvailableForTrading;
        CashReceipts = cashReceipts;
        DayTradingEquityCall = dayTradingEquityCall;
        LiquidationValue = liquidationValue;
        LongOptionMarketValue = longOptionMarketValue;
        LongStockValue = longStockValue;
        Margin = margin;
        MarginEquity = marginEquity;
        MoneyMarketFund = moneyMarketFund;
        MutualFundValue = mutualFundValue;
        ShortOptionMarketValue = shortOptionMarketValue;
        ShortStockValue = shortStockValue;
        TotalCash = totalCash;
        UnsettledCash = unsettledCash;
        PendingDeposits = pendingDeposits;
        AccountValue = accountValue;
    }
}

/// <summary>
/// Represents the summary of trading power, including available funds and buying power details.
/// Inherits from the <see cref="Balance"/> class.
/// </summary>
public class TradingPowerSummary : Balance
{
    /// <summary>
    /// Gets the available funds in the account.
    /// </summary>
    [JsonProperty("availableFunds")]
    public decimal AvailableFunds { get; }

    /// <summary>
    /// Gets the buying power for non-marginable trades.
    /// </summary>
    [JsonProperty("buyingPowerNonMarginableTrade")]
    public decimal BuyingPowerNonMarginableTrade { get; }

    /// <summary>
    /// Gets the Special Memorandum Account (SMA) value.
    /// </summary>
    [JsonProperty("sma")]
    public decimal Sma { get; }

    /// <summary>
    /// Gets the stock buying power available in the account.
    /// </summary>
    [JsonProperty("stockBuyingPower")]
    public decimal StockBuyingPower { get; }

    /// <summary>
    /// Gets the option buying power available in the account.
    /// </summary>
    [JsonProperty("OptionBuyingPower")]
    public decimal OptionBuyingPower { get; }


    /// <summary>
    /// Initializes a new instance of the <see cref="TradingPowerSummary"/> class.
    /// </summary>
    /// <param name="availableFunds">The available funds in the account.</param>
    /// <param name="buyingPowerNonMarginableTrade">The buying power for non-marginable trades.</param>
    /// <param name="sma">The Special Memorandum Account (SMA) value.</param>
    /// <param name="stockBuyingPower">The stock buying power available.</param>
    /// <param name="optionBuyingPower">The option buying power available.</param>
    /// <param name="availableFundsNonMarginableTrade">The non-marginable available funds.</param>
    /// <param name="buyingPower">The total buying power available.</param>
    /// <param name="dayTradingBuyingPower">The day trading buying power available.</param>
    /// <param name="dayTradingBuyingPowerCall">The day trading buying power call.</param>
    /// <param name="equity">The total equity of the account.</param>
    /// <param name="equityPercentage">The percentage of equity.</param>
    /// <param name="longMarginValue">The long margin value.</param>
    /// <param name="maintenanceCall">The maintenance call amount.</param>
    /// <param name="maintenanceRequirement">The maintenance requirement.</param>
    /// <param name="regTCall">The Regulation T call amount.</param>
    /// <param name="shortMarginValue">The short margin value.</param>
    /// <param name="isInCall">Indicates whether the account is in call status.</param>
    /// <param name="marginBalance">The margin balance available in the account.</param>
    /// <param name="shortBalance">The short balance in the account.</param>
    public TradingPowerSummary(decimal availableFunds, decimal buyingPowerNonMarginableTrade, decimal sma, decimal stockBuyingPower, decimal optionBuyingPower,
        decimal availableFundsNonMarginableTrade, decimal buyingPower, decimal dayTradingBuyingPower, decimal dayTradingBuyingPowerCall, decimal equity, decimal equityPercentage,
        decimal longMarginValue, decimal maintenanceCall, decimal maintenanceRequirement, decimal regTCall, decimal shortMarginValue, bool isInCall, decimal marginBalance, decimal shortBalance)
        : base(availableFundsNonMarginableTrade, buyingPower, dayTradingBuyingPower, dayTradingBuyingPowerCall, equity, equityPercentage, longMarginValue, maintenanceCall, maintenanceRequirement,
            regTCall, shortMarginValue, isInCall, marginBalance, shortBalance)
    {
        AvailableFunds = availableFunds;
        BuyingPowerNonMarginableTrade = buyingPowerNonMarginableTrade;
        Sma = sma;
        StockBuyingPower = stockBuyingPower;
        OptionBuyingPower = optionBuyingPower;
    }
}