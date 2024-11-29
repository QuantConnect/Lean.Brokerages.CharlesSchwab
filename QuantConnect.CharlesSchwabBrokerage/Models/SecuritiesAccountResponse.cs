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
public class SecuritiesAccountResponse
{
    /// <summary>
    /// Metadata of the securities account, including details such as account type, positions, and trading-related attributes.
    /// </summary>
    public SecuritiesAccount SecuritiesAccount { get; }

    /// <summary>
    /// The aggregated balance of the account, which includes liquidation values and other high-level account balance information.
    /// </summary>
    public AggregatedBalance AggregatedBalance { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SecuritiesAccountResponse"/> class with the specified securities account and aggregated balance.
    /// </summary>
    /// <param name="securitiesAccount">
    /// Metadata of the securities account, including details such as account type, positions, and trading-related attributes.
    /// </param>
    /// <param name="aggregatedBalance">
    /// The aggregated balance of the account, which includes liquidation values and other high-level account balance information.
    /// </param>
    [JsonConstructor]
    public SecuritiesAccountResponse(SecuritiesAccount securitiesAccount, AggregatedBalance aggregatedBalance)
        => (SecuritiesAccount, AggregatedBalance) = (securitiesAccount, aggregatedBalance);
}


/// <summary>
/// Represents aggregated balance information for a Charles Schwab securities account.
/// </summary>
public class AggregatedBalance
{
    /// <summary>
    /// The current liquidation value of the account, representing the immediate value if all holdings were sold.
    /// </summary>
    public decimal CurrentLiquidationValue { get; }

    /// <summary>
    /// The overall liquidation value of the account, which may include adjustments or future expectations.
    /// </summary>
    public decimal LiquidationValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregatedBalance"/> class with the specified current liquidation value and overall liquidation value.
    /// </summary>
    /// <param name="currentLiquidationValue">
    /// The current liquidation value of the account, representing the immediate value if all holdings were sold.
    /// </param>
    /// <param name="liquidationValue">
    /// The overall liquidation value of the account, which may include adjustments or future expectations.
    /// </param>
    [JsonConstructor]
    public AggregatedBalance(decimal currentLiquidationValue, decimal liquidationValue)
        => (CurrentLiquidationValue, LiquidationValue) = (currentLiquidationValue, liquidationValue);
}

/// <summary>
/// Represents a securities account, including account metadata and balance details.
/// </summary>
public class SecuritiesAccount
{
    /// <summary>
    /// The type of the account, such as "CASH" or "MARGIN".
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// The unique identifier for the account.
    /// </summary>
    public string AccountNumber { get; }

    /// <summary>
    /// The total number of round trips (buy and sell transactions completed within the same day).
    /// </summary>
    public int RoundTrips { get; }

    /// <summary>
    /// Indicates whether the account holder is classified as a day trader.
    /// </summary>
    public bool IsDayTrader { get; }

    /// <summary>
    /// Indicates whether the account is restricted to closing only (no new positions can be opened).
    /// </summary>
    public bool IsClosingOnlyRestricted { get; }

    /// <summary>
    /// Indicates if the Pattern Day Trader (PDT) flag is set on the account.
    /// </summary>
    public bool PfcbFlag { get; }

    /// <summary>
    /// A collection of financial positions (such as stocks, bonds, etc.) held in the account.
    /// </summary>
    public IReadOnlyCollection<Position> Positions { get; }

    /// <summary>
    /// The initial balance details of the account, including available funds and margin values.
    /// </summary>
    public InitialBalance InitialBalances { get; }

    /// <summary>
    /// The current balance details, such as available funds, buying power, and equity.
    /// </summary>
    public CurrentBalance CurrentBalances { get; }

    /// <summary>
    /// The projected balance details, including future buying power and maintenance calls.
    /// </summary>
    public ProjectedBalance ProjectedBalances { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SecuritiesAccount"/> class with the specified details.
    /// </summary>
    /// <param name="type">The type of the account, such as "CASH" or "MARGIN".</param>
    /// <param name="accountNumber">The unique identifier for the account.</param>
    /// <param name="roundTrips">The total number of round trips (buy and sell transactions completed within the same day).</param>
    /// <param name="isDayTrader">Indicates whether the account holder is classified as a day trader.</param>
    /// <param name="isClosingOnlyRestricted">Indicates whether the account is restricted to closing only (no new positions can be opened).</param>
    /// <param name="pfcbFlag">Indicates if the Pattern Day Trader (PDT) flag is set on the account.</param>
    /// <param name="positions">A collection of financial positions (such as stocks, bonds, etc.) held in the account.</param>
    /// <param name="initialBalances">The initial balance details of the account, including available funds and margin values.</param>
    /// <param name="currentBalances">The current balance details, such as available funds, buying power, and equity.</param>
    /// <param name="projectedBalances">The projected balance details, including future buying power and maintenance calls.</param>
    [JsonConstructor]
    public SecuritiesAccount(string type, string accountNumber, int roundTrips, bool isDayTrader, bool isClosingOnlyRestricted, bool pfcbFlag, IReadOnlyCollection<Position> positions,
        InitialBalance initialBalances, CurrentBalance currentBalances, ProjectedBalance projectedBalances)
    {
        Type = type;
        AccountNumber = accountNumber;
        RoundTrips = roundTrips;
        IsDayTrader = isDayTrader;
        IsClosingOnlyRestricted = isClosingOnlyRestricted;
        PfcbFlag = pfcbFlag;
        Positions = positions;
        InitialBalances = initialBalances;
        CurrentBalances = currentBalances;
        ProjectedBalances = projectedBalances;
    }
}

/// <summary>
/// Represents the balances of an account at different stages, including available funds, buying power, equity, and margin-related values.
/// </summary>
public class InitialBalance
{
    /// <summary>
    /// The amount of interest that has been accrued but not yet paid.
    /// </summary>
    public decimal AccruedInterest { get; }

    /// <summary>
    /// Available funds that can be used for non-marginable trades.
    /// </summary>
    public decimal AvailableFundsNonMarginableTrade { get; }

    /// <summary>
    /// The total value of bonds held in the account.
    /// </summary>
    public decimal BondValue { get; }

    /// <summary>
    /// The amount of buying power available in the account.
    /// </summary>
    public decimal BuyingPower { get; }

    /// <summary>
    /// The total cash balance in the account.
    /// </summary>
    public decimal CashBalance { get; }

    /// <summary>
    /// The amount of cash available for trading activities.
    /// </summary>
    public decimal CashAvailableForTrading { get; }

    /// <summary>
    /// The total cash receipts in the account.
    /// </summary>
    public decimal CashReceipts { get; }

    /// <summary>
    /// The buying power available for day trading purposes.
    /// </summary>
    public decimal DayTradingBuyingPower { get; }

    /// <summary>
    /// The call amount related to day trading buying power.
    /// </summary>
    public decimal DayTradingBuyingPowerCall { get; }

    /// <summary>
    /// The call amount related to day trading equity.
    /// </summary>
    public decimal DayTradingEquityCall { get; }

    /// <summary>
    /// The total equity in the account.
    /// </summary>
    public decimal Equity { get; }

    /// <summary>
    /// The percentage of the account that is equity.
    /// </summary>
    public decimal EquityPercentage { get; }

    /// <summary>
    /// The value of the account if all positions were liquidated.
    /// </summary>
    public decimal LiquidationValue { get; }

    /// <summary>
    /// The margin value of long positions.
    /// </summary>
    public decimal LongMarginValue { get; }

    /// <summary>
    /// The market value of long options in the account.
    /// </summary>
    public decimal LongOptionMarketValue { get; }

    /// <summary>
    /// The market value of long stock positions in the account.
    /// </summary>
    public decimal LongStockValue { get; }

    /// <summary>
    /// The amount of maintenance call in the account.
    /// </summary>
    public decimal MaintenanceCall { get; }

    /// <summary>
    /// The minimum maintenance requirement for the account.
    /// </summary>
    public decimal MaintenanceRequirement { get; }

    /// <summary>
    /// The total margin balance in the account.
    /// </summary>
    public decimal Margin { get; }

    /// <summary>
    /// The equity portion of the margin balance.
    /// </summary>
    public decimal MarginEquity { get; }

    /// <summary>
    /// The total value of the money market fund in the account.
    /// </summary>
    public decimal MoneyMarketFund { get; }

    /// <summary>
    /// The total value of mutual fund holdings in the account.
    /// </summary>
    public decimal MutualFundValue { get; }

    /// <summary>
    /// The Regulation T call amount for the account.
    /// </summary>
    public decimal RegTCall { get; }

    /// <summary>
    /// The margin value of short positions.
    /// </summary>
    public decimal ShortMarginValue { get; }

    /// <summary>
    /// The market value of short options in the account.
    /// </summary>
    public decimal ShortOptionMarketValue { get; }

    /// <summary>
    /// The market value of short stock positions in the account.
    /// </summary>
    public decimal ShortStockValue { get; }

    /// <summary>
    /// The total cash value in the account.
    /// </summary>
    public decimal TotalCash { get; }

    /// <summary>
    /// Indicates whether the account is in a call status (typically due to margin or other obligations).
    /// </summary>
    public bool IsInCall { get; }

    /// <summary>
    /// The amount of pending deposits that have not yet cleared in the account.
    /// </summary>
    public decimal PendingDeposits { get; }

    /// <summary>
    /// The total margin balance in the account.
    /// </summary>
    public decimal MarginBalance { get; }

    /// <summary>
    /// The total short balance in the account.
    /// </summary>
    public decimal ShortBalance { get; }

    /// <summary>
    /// The overall value of the account, including cash and positions.
    /// </summary>
    public decimal AccountValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InitialBalance"/> class with the specified balance details.
    /// </summary>
    /// <param name="accruedInterest">The amount of interest that has been accrued but not yet paid.</param>
    /// <param name="availableFundsNonMarginableTrade">Available funds that can be used for non-marginable trades.</param>
    /// <param name="bondValue">The total value of bonds held in the account.</param>
    /// <param name="buyingPower">The amount of buying power available in the account.</param>
    /// <param name="cashBalance">The total cash balance in the account.</param>
    /// <param name="cashAvailableForTrading">The amount of cash available for trading activities.</param>
    /// <param name="cashReceipts">The total cash receipts in the account.</param>
    /// <param name="dayTradingBuyingPower">The buying power available for day trading purposes.</param>
    /// <param name="dayTradingBuyingPowerCall">The call amount related to day trading buying power.</param>
    /// <param name="dayTradingEquityCall">The call amount related to day trading equity.</param>
    /// <param name="equity">The total equity in the account.</param>
    /// <param name="equityPercentage">The percentage of the account that is equity.</param>
    /// <param name="liquidationValue">The value of the account if all positions were liquidated.</param>
    /// <param name="longMarginValue">The margin value of long positions.</param>
    /// <param name="longOptionMarketValue">The market value of long options in the account.</param>
    /// <param name="longStockValue">The market value of long stock positions in the account.</param>
    /// <param name="maintenanceCall">The amount of maintenance call in the account.</param>
    /// <param name="maintenanceRequirement">The minimum maintenance requirement for the account.</param>
    /// <param name="margin">The total margin balance in the account.</param>
    /// <param name="marginEquity">The equity portion of the margin balance.</param>
    /// <param name="moneyMarketFund">The total value of the money market fund in the account.</param>
    /// <param name="mutualFundValue">The total value of mutual fund holdings in the account.</param>
    /// <param name="regTCall">The Regulation T call amount for the account.</param>
    /// <param name="shortMarginValue">The margin value of short positions.</param>
    /// <param name="shortOptionMarketValue">The market value of short options in the account.</param>
    /// <param name="shortStockValue">The market value of short stock positions in the account.</param>
    /// <param name="totalCash">The total cash value in the account.</param>
    /// <param name="isInCall">Indicates whether the account is in a call status (typically due to margin or other obligations).</param>
    /// <param name="pendingDeposits">The amount of pending deposits that have not yet cleared in the account.</param>
    /// <param name="marginBalance">The total margin balance in the account.</param>
    /// <param name="shortBalance">The total short balance in the account.</param>
    /// <param name="accountValue">The overall value of the account, including cash and positions.</param>
    [JsonConstructor]
    public InitialBalance(decimal accruedInterest, decimal availableFundsNonMarginableTrade, decimal bondValue, decimal buyingPower, decimal cashBalance, decimal cashAvailableForTrading, decimal cashReceipts, decimal dayTradingBuyingPower, decimal dayTradingBuyingPowerCall, decimal dayTradingEquityCall, decimal equity, decimal equityPercentage, decimal liquidationValue,
decimal longMarginValue, decimal longOptionMarketValue, decimal longStockValue, decimal maintenanceCall, decimal maintenanceRequirement, decimal margin, decimal marginEquity, decimal moneyMarketFund, decimal mutualFundValue, decimal regTCall, decimal shortMarginValue, decimal shortOptionMarketValue, decimal shortStockValue, decimal totalCash, bool isInCall, decimal pendingDeposits, decimal marginBalance, decimal shortBalance, decimal accountValue)
    {
        AccruedInterest = accruedInterest;
        AvailableFundsNonMarginableTrade = availableFundsNonMarginableTrade;
        BondValue = bondValue;
        BuyingPower = buyingPower;
        CashBalance = cashBalance;
        CashAvailableForTrading = cashAvailableForTrading;
        CashReceipts = cashReceipts;
        DayTradingBuyingPower = dayTradingBuyingPower;
        DayTradingBuyingPowerCall = dayTradingBuyingPowerCall;
        DayTradingEquityCall = dayTradingEquityCall;
        Equity = equity;
        EquityPercentage = equityPercentage;
        LiquidationValue = liquidationValue;
        LongMarginValue = longMarginValue;
        LongOptionMarketValue = longOptionMarketValue;
        LongStockValue = longStockValue;
        MaintenanceCall = maintenanceCall;
        MaintenanceRequirement = maintenanceRequirement;
        Margin = margin;
        MarginEquity = marginEquity;
        MoneyMarketFund = moneyMarketFund;
        MutualFundValue = mutualFundValue;
        RegTCall = regTCall;
        ShortMarginValue = shortMarginValue;
        ShortOptionMarketValue = shortOptionMarketValue;
        ShortStockValue = shortStockValue;
        TotalCash = totalCash;
        IsInCall = isInCall;
        PendingDeposits = pendingDeposits;
        MarginBalance = marginBalance;
        ShortBalance = shortBalance;
        AccountValue = accountValue;
    }
}

/// <summary>
/// Represents the current balance details of an account, including various market values, available funds, and margin information.
/// </summary>
public class CurrentBalance
{
    /// <summary>
    /// The amount of interest accrued but not yet paid.
    /// </summary>
    public decimal AccruedInterest { get; }

    /// <summary>
    /// The total cash balance available in the account.
    /// </summary>
    public decimal CashBalance { get; }

    /// <summary>
    /// The total cash receipts in the account.
    /// </summary>
    public decimal CashReceipts { get; }

    /// <summary>
    /// The market value of long options held in the account.
    /// </summary>
    public decimal LongOptionMarketValue { get; }

    /// <summary>
    /// The value of the account if all positions were liquidated.
    /// </summary>
    public decimal LiquidationValue { get; }

    /// <summary>
    /// The total market value of long positions in the account.
    /// </summary>
    public decimal LongMarketValue { get; }

    /// <summary>
    /// The total value of money market funds in the account.
    /// </summary>
    public decimal MoneyMarketFund { get; }

    /// <summary>
    /// The amount saved in savings or similar accounts.
    /// </summary>
    public decimal Savings { get; }

    /// <summary>
    /// The total market value of short positions in the account.
    /// </summary>
    public decimal ShortMarketValue { get; }

    /// <summary>
    /// The total amount of deposits that are pending or not yet cleared.
    /// </summary>
    public decimal PendingDeposits { get; }

    /// <summary>
    /// The total market value of mutual fund holdings in the account.
    /// </summary>
    public decimal MutualFundValue { get; }

    /// <summary>
    /// The total market value of bond holdings in the account.
    /// </summary>
    public decimal BondValue { get; }

    /// <summary>
    /// The market value of short options held in the account.
    /// </summary>
    public decimal ShortOptionMarketValue { get; }

    /// <summary>
    /// The total funds currently available in the account for trading or withdrawal.
    /// </summary>
    public decimal AvailableFunds { get; }

    /// <summary>
    /// The amount of available funds for non-marginable trades.
    /// </summary>
    public decimal AvailableFundsNonMarginableTrade { get; }

    /// <summary>
    /// The total buying power available in the account.
    /// </summary>
    public decimal BuyingPower { get; }

    /// <summary>
    /// The buying power available for non-marginable trades.
    /// </summary>
    public decimal BuyingPowerNonMarginableTrade { get; }

    /// <summary>
    /// The buying power specifically available for day trading.
    /// </summary>
    public decimal DayTradingBuyingPower { get; }

    /// <summary>
    /// The total equity in the account.
    /// </summary>
    public decimal Equity { get; }

    /// <summary>
    /// The percentage of the account value that is equity.
    /// </summary>
    public decimal EquityPercentage { get; }

    /// <summary>
    /// The margin value of long positions held in the account.
    /// </summary>
    public decimal LongMarginValue { get; }

    /// <summary>
    /// The total maintenance call amount in the account.
    /// </summary>
    public decimal MaintenanceCall { get; }

    /// <summary>
    /// The minimum maintenance requirement for the account.
    /// </summary>
    public decimal MaintenanceRequirement { get; }

    /// <summary>
    /// The total margin balance in the account.
    /// </summary>
    public decimal MarginBalance { get; }

    /// <summary>
    /// The Regulation T call amount in the account.
    /// </summary>
    public decimal RegTCall { get; }

    /// <summary>
    /// The short balance in the account.
    /// </summary>
    public decimal ShortBalance { get; }

    /// <summary>
    /// The margin value of short positions held in the account.
    /// </summary>
    public decimal ShortMarginValue { get; }

    /// <summary>
    /// The Special Memorandum Account (SMA) value.
    /// </summary>
    public decimal Sma { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentBalance"/> class with the specified balance details.
    /// </summary>
    /// <param name="accruedInterest">The amount of interest accrued but not yet paid.</param>
    /// <param name="cashBalance">The total cash balance available in the account.</param>
    /// <param name="cashReceipts">The total cash receipts in the account.</param>
    /// <param name="longOptionMarketValue">The market value of long options held in the account.</param>
    /// <param name="liquidationValue">The value of the account if all positions were liquidated.</param>
    /// <param name="longMarketValue">The total market value of long positions in the account.</param>
    /// <param name="moneyMarketFund">The total value of money market funds in the account.</param>
    /// <param name="savings">The amount saved in savings or similar accounts.</param>
    /// <param name="shortMarketValue">The total market value of short positions in the account.</param>
    /// <param name="pendingDeposits">The total amount of deposits that are pending or not yet cleared.</param>
    /// <param name="mutualFundValue">The total market value of mutual fund holdings in the account.</param>
    /// <param name="bondValue">The total market value of bond holdings in the account.</param>
    /// <param name="shortOptionMarketValue">The market value of short options held in the account.</param>
    /// <param name="availableFunds">The total funds currently available in the account for trading or withdrawal.</param>
    /// <param name="availableFundsNonMarginableTrade">The amount of available funds for non-marginable trades.</param>
    /// <param name="buyingPower">The total buying power available in the account.</param>
    /// <param name="buyingPowerNonMarginableTrade">The buying power available for non-marginable trades.</param>
    /// <param name="dayTradingBuyingPower">The buying power specifically available for day trading.</param>
    /// <param name="equity">The total equity in the account.</param>
    /// <param name="equityPercentage">The percentage of the account value that is equity.</param>
    /// <param name="longMarginValue">The margin value of long positions held in the account.</param>
    /// <param name="maintenanceCall">The total maintenance call amount in the account.</param>
    /// <param name="maintenanceRequirement">The minimum maintenance requirement for the account.</param>
    /// <param name="marginBalance">The total margin balance in the account.</param>
    /// <param name="regTCall">The Regulation T call amount in the account.</param>
    /// <param name="shortBalance">The short balance in the account.</param>
    /// <param name="shortMarginValue">The margin value of short positions held in the account.</param>
    /// <param name="sma">The Special Memorandum Account (SMA) value.</param>
    [JsonConstructor]
    public CurrentBalance(decimal accruedInterest, decimal cashBalance, decimal cashReceipts, decimal longOptionMarketValue,
        decimal liquidationValue, decimal longMarketValue, decimal moneyMarketFund, decimal savings, decimal shortMarketValue,
        decimal pendingDeposits, decimal mutualFundValue, decimal bondValue, decimal shortOptionMarketValue, decimal availableFunds,
        decimal availableFundsNonMarginableTrade, decimal buyingPower, decimal buyingPowerNonMarginableTrade, decimal dayTradingBuyingPower, decimal equity,
        decimal equityPercentage, decimal longMarginValue, decimal maintenanceCall, decimal maintenanceRequirement, decimal marginBalance,
        decimal regTCall, decimal shortBalance, decimal shortMarginValue, decimal sma)
    {
        AccruedInterest = accruedInterest;
        CashBalance = cashBalance;
        CashReceipts = cashReceipts;
        LongOptionMarketValue = longOptionMarketValue;
        LiquidationValue = liquidationValue;
        LongMarketValue = longMarketValue;
        MoneyMarketFund = moneyMarketFund;
        Savings = savings;
        ShortMarketValue = shortMarketValue;
        PendingDeposits = pendingDeposits;
        MutualFundValue = mutualFundValue;
        BondValue = bondValue;
        ShortOptionMarketValue = shortOptionMarketValue;
        AvailableFunds = availableFunds;
        AvailableFundsNonMarginableTrade = availableFundsNonMarginableTrade;
        BuyingPower = buyingPower;
        BuyingPowerNonMarginableTrade = buyingPowerNonMarginableTrade;
        DayTradingBuyingPower = dayTradingBuyingPower;
        Equity = equity;
        EquityPercentage = equityPercentage;
        LongMarginValue = longMarginValue;
        MaintenanceCall = maintenanceCall;
        MaintenanceRequirement = maintenanceRequirement;
        MarginBalance = marginBalance;
        RegTCall = regTCall;
        ShortBalance = shortBalance;
        ShortMarginValue = shortMarginValue;
        Sma = sma;
    }
}

/// <summary>
/// Represents the projected balance of an account, including future buying power, available funds, and call amounts.
/// </summary>
public class ProjectedBalance
{
    /// <summary>
    /// The projected amount of funds that will be available for trading or withdrawal.
    /// </summary>
    public decimal AvailableFunds { get; }

    /// <summary>
    /// The projected amount of funds available for non-marginable trades.
    /// </summary>
    public decimal AvailableFundsNonMarginableTrade { get; }

    /// <summary>
    /// The projected buying power available in the account.
    /// </summary>
    public decimal BuyingPower { get; }

    /// <summary>
    /// The projected buying power specifically available for day trading.
    /// </summary>
    public decimal DayTradingBuyingPower { get; }

    /// <summary>
    /// The projected call amount for day trading buying power.
    /// </summary>
    public decimal DayTradingBuyingPowerCall { get; }

    /// <summary>
    /// The projected maintenance call amount in the account.
    /// </summary>
    public decimal MaintenanceCall { get; }

    /// <summary>
    /// The projected Regulation T call amount in the account.
    /// </summary>
    public decimal RegTCall { get; }

    /// <summary>
    /// Indicates if the account is projected to be in a call status.
    /// </summary>
    public bool IsInCall { get; }

    /// <summary>
    /// The projected buying power for stock purchases.
    /// </summary>
    public decimal StockBuyingPower { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectedBalance"/> class with the specified projected balance details.
    /// </summary>
    /// <param name="availableFunds">The projected amount of funds that will be available for trading or withdrawal.</param>
    /// <param name="availableFundsNonMarginableTrade">The projected amount of funds available for non-marginable trades.</param>
    /// <param name="buyingPower">The projected buying power available in the account.</param>
    /// <param name="dayTradingBuyingPower">The projected buying power specifically available for day trading.</param>
    /// <param name="dayTradingBuyingPowerCall">The projected call amount for day trading buying power.</param>
    /// <param name="maintenanceCall">The projected maintenance call amount in the account.</param>
    /// <param name="regTCall">The projected Regulation T call amount in the account.</param>
    /// <param name="isInCall">Indicates if the account is projected to be in a call status.</param>
    /// <param name="stockBuyingPower">The projected buying power for stock purchases.</param>
    [JsonConstructor]
    public ProjectedBalance(decimal availableFunds, decimal availableFundsNonMarginableTrade, decimal buyingPower, decimal dayTradingBuyingPower, decimal dayTradingBuyingPowerCall,
        decimal maintenanceCall, decimal regTCall, bool isInCall, decimal stockBuyingPower)
    {
        AvailableFunds = availableFunds;
        AvailableFundsNonMarginableTrade = availableFundsNonMarginableTrade;
        BuyingPower = buyingPower;
        DayTradingBuyingPower = dayTradingBuyingPower;
        DayTradingBuyingPowerCall = dayTradingBuyingPowerCall;
        MaintenanceCall = maintenanceCall;
        RegTCall = regTCall;
        IsInCall = isInCall;
        StockBuyingPower = stockBuyingPower;
    }
}

/// <summary>
/// Represents a position held in the securities account.
/// </summary>
public class Position
{
    /// <summary>
    /// The short quantity held in the position.
    /// </summary>
    public decimal ShortQuantity { get; }

    /// <summary>
    /// The average price of the position.
    /// </summary>
    public decimal AveragePrice { get; }

    /// <summary>
    /// The profit or loss for the current day.
    /// </summary>
    public decimal CurrentDayProfitLoss { get; }

    /// <summary>
    /// The percentage profit or loss for the current day.
    /// </summary>
    public decimal CurrentDayProfitLossPercentage { get; }

    /// <summary>
    /// The long quantity held in the position.
    /// </summary>
    public decimal LongQuantity { get; }

    /// <summary>
    /// The settled long quantity in the position.
    /// </summary>
    public decimal SettledLongQuantity { get; }

    /// <summary>
    /// The settled short quantity in the position.
    /// </summary>
    public decimal SettledShortQuantity { get; }

    /// <summary>
    /// The aged quantity of the position.
    /// </summary>
    public decimal AgedQuantity { get; }

    /// <summary>
    /// The financial instrument associated with this position.
    /// </summary>
    public Instrument Instrument { get; }

    /// <summary>
    /// The market value of the position.
    /// </summary>
    public decimal MarketValue { get; }

    /// <summary>
    /// The maintenance requirement for the position.
    /// </summary>
    public decimal MaintenanceRequirement { get; }

    /// <summary>
    /// The average price for the long position.
    /// </summary>
    public decimal AverageLongPrice { get; }

    /// <summary>
    /// The average price for the short position.
    /// </summary>
    public decimal AverageShortPrice { get; }

    /// <summary>
    /// The tax lot average price for the long position.
    /// </summary>
    public decimal TaxLotAverageLongPrice { get; }

    /// <summary>
    /// The tax lot average price for the short position.
    /// </summary>
    public decimal TaxLotAverageShortPrice { get; }

    /// <summary>
    /// The open profit or loss for the long position.
    /// </summary>
    public decimal LongOpenProfitLoss { get; }

    /// <summary>
    /// The open profit or loss for the short position.
    /// </summary>
    public decimal ShortOpenProfitLoss { get; }

    /// <summary>
    /// The previous session's long quantity.
    /// </summary>
    public decimal PreviousSessionLongQuantity { get; }

    /// <summary>
    /// The previous session's short quantity.
    /// </summary>
    public decimal PreviousSessionShortQuantity { get; }

    /// <summary>
    /// The cost incurred for the position on the current day.
    /// </summary>
    public decimal CurrentDayCost { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Position"/> class with the specified position details.
    /// </summary>
    /// <param name="shortQuantity">The short quantity held in the position.</param>
    /// <param name="averagePrice">The average price of the position.</param>
    /// <param name="currentDayProfitLoss">The profit or loss for the current day.</param>
    /// <param name="currentDayProfitLossPercentage">The percentage profit or loss for the current day.</param>
    /// <param name="longQuantity">The long quantity held in the position.</param>
    /// <param name="settledLongQuantity">The settled long quantity in the position.</param>
    /// <param name="settledShortQuantity">The settled short quantity in the position.</param>
    /// <param name="agedQuantity">The aged quantity of the position.</param>
    /// <param name="instrument">The financial instrument associated with this position.</param>
    /// <param name="marketValue">The market value of the position.</param>
    /// <param name="maintenanceRequirement">The maintenance requirement for the position.</param>
    /// <param name="averageLongPrice">The average price for the long position.</param>
    /// <param name="averageShortPrice">The average price for the short position.</param>
    /// <param name="taxLotAverageLongPrice">The tax lot average price for the long position.</param>
    /// <param name="taxLotAverageShortPrice">The tax lot average price for the short position.</param>
    /// <param name="longOpenProfitLoss">The open profit or loss for the long position.</param>
    /// <param name="shortOpenProfitLoss">The open profit or loss for the short position.</param>
    /// <param name="previousSessionLongQuantity">The previous session's long quantity.</param>
    /// <param name="previousSessionShortQuantity">The previous session's short quantity.</param>
    /// <param name="currentDayCost">The cost incurred for the position on the current day.</param>
    [JsonConstructor]
    public Position(decimal shortQuantity, decimal averagePrice, decimal currentDayProfitLoss, decimal currentDayProfitLossPercentage, decimal longQuantity,
        decimal settledLongQuantity, decimal settledShortQuantity, decimal agedQuantity, Instrument instrument, decimal marketValue,
        decimal maintenanceRequirement, decimal averageLongPrice, decimal averageShortPrice, decimal taxLotAverageLongPrice, decimal taxLotAverageShortPrice,
        decimal longOpenProfitLoss, decimal shortOpenProfitLoss, decimal previousSessionLongQuantity, decimal previousSessionShortQuantity, decimal currentDayCost)
    {
        ShortQuantity = shortQuantity;
        AveragePrice = averagePrice;
        CurrentDayProfitLoss = currentDayProfitLoss;
        CurrentDayProfitLossPercentage = currentDayProfitLossPercentage;
        LongQuantity = longQuantity;
        SettledLongQuantity = settledLongQuantity;
        SettledShortQuantity = settledShortQuantity;
        AgedQuantity = agedQuantity;
        Instrument = instrument;
        MarketValue = marketValue;
        MaintenanceRequirement = maintenanceRequirement;
        AverageLongPrice = averageLongPrice;
        AverageShortPrice = averageShortPrice;
        TaxLotAverageLongPrice = taxLotAverageLongPrice;
        TaxLotAverageShortPrice = taxLotAverageShortPrice;
        LongOpenProfitLoss = longOpenProfitLoss;
        ShortOpenProfitLoss = shortOpenProfitLoss;
        PreviousSessionLongQuantity = previousSessionLongQuantity;
        PreviousSessionShortQuantity = previousSessionShortQuantity;
        CurrentDayCost = currentDayCost;
    }
}