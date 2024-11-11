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

using System;
using NodaTime;
using System.Linq;
using NUnit.Framework;
using QuantConnect.Util;
using QuantConnect.Data;
using Microsoft.CodeAnalysis;
using QuantConnect.Securities;
using QuantConnect.Data.Market;
using System.Collections.Generic;
using QuantConnect.Configuration;
using QuantConnect.Lean.Engine.HistoricalData;

namespace QuantConnect.Brokerages.CharlesSchwab.Tests;

[TestFixture]
public class CharlesSchwabBrokerageHistoryProviderTests
{
    private BrokerageHistoryProvider _historyProvider;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var brokerage = TestSetup.CreateBrokerage(null, null);

        _historyProvider = new BrokerageHistoryProvider();
        _historyProvider.SetBrokerage(brokerage);
        _historyProvider.Initialize(new HistoryProviderInitializeParameters(null, null, null, null, null, null, null, false, null, null, new AlgorithmSettings() { DailyPreciseEndTime = true }));
    }

    private static IEnumerable<TestCaseData> ValidHistoryParameters
    {
        get
        {
            var AAPL = CreateSymbol("AAPL", SecurityType.Equity);
            yield return new TestCaseData(AAPL, Resolution.Minute, new DateTime(2024, 10, 1), new DateTime(2024, 11, 8));
            var endDate = DateTime.UtcNow.Date;
            yield return new TestCaseData(AAPL, Resolution.Minute, endDate.AddDays(-45), endDate);
            yield return new TestCaseData(AAPL, Resolution.Hour, new DateTime(2024, 06, 18), new DateTime(2024, 07, 18));
            yield return new TestCaseData(AAPL, Resolution.Daily, new DateTime(2024, 06, 18), new DateTime(2024, 07, 18));

            var DJI_Index = Symbol.Create("DJI", SecurityType.Index, Market.USA);
            yield return new TestCaseData(DJI_Index, Resolution.Minute, new DateTime(2024, 10, 1), new DateTime(2024, 11, 8));
            yield return new TestCaseData(DJI_Index, Resolution.Minute, endDate.AddDays(-45), endDate);
            yield return new TestCaseData(DJI_Index, Resolution.Hour, new DateTime(2024, 06, 18), new DateTime(2024, 07, 18));
            yield return new TestCaseData(DJI_Index, Resolution.Daily, new DateTime(2024, 06, 18), new DateTime(2024, 07, 18));

            var AAPLOption = CreateSymbol("AAPL", SecurityType.Option, OptionRight.Call, 230m, new DateTime(2024, 11, 15));
            yield return new TestCaseData(AAPLOption, Resolution.Minute, new DateTime(2024, 10, 1), new DateTime(2024, 11, 8));
            yield return new TestCaseData(AAPLOption, Resolution.Hour, new DateTime(2024, 9, 1), new DateTime(2024, 11, 8));
            yield return new TestCaseData(AAPLOption, Resolution.Daily, new DateTime(2024, 10, 18), new DateTime(2024, 11, 8));
        }
    }

    [TestCaseSource(nameof(ValidHistoryParameters))]
    public void GetsHistory(Symbol symbol, Resolution resolution, DateTime startDateTime, DateTime endDateTime)
    {
        var historyRequest = CreateHistoryRequest(symbol, resolution, TickType.Trade, startDateTime, endDateTime);

        var history = _historyProvider.GetHistory(new[] { historyRequest }, TimeZones.NewYork);

        Assert.IsNotNull(history);
        Assert.IsNotEmpty(history);

        AssertTradeBars(history.SelectMany(t => t.Bars.Values), symbol, resolution.ToTimeSpan());

        if (_historyProvider.DataPointCount > 0)
        {
            // Ordered by time
            Assert.That(history, Is.Ordered.By("Time"));

            // No repeating bars
            var timesArray = history.Select(x => x.Time).ToArray();
            Assert.AreEqual(timesArray.Length, timesArray.Distinct().Count());
        }
    }

    private static void AssertTradeBars(IEnumerable<TradeBar> tradeBars, Symbol symbol, TimeSpan period)
    {
        foreach (var tradeBar in tradeBars)
        {
            Assert.That(tradeBar.Symbol, Is.EqualTo(symbol));
            Assert.That(tradeBar.Period, Is.EqualTo(period));
            Assert.That(tradeBar.Open, Is.GreaterThan(0));
            Assert.That(tradeBar.High, Is.GreaterThan(0));
            Assert.That(tradeBar.Low, Is.GreaterThan(0));
            Assert.That(tradeBar.Close, Is.GreaterThan(0));
            Assert.That(tradeBar.Price, Is.GreaterThan(0));
            Assert.That(tradeBar.Volume, Is.GreaterThanOrEqualTo(0));
            Assert.That(tradeBar.Time, Is.GreaterThan(default(DateTime)));
            Assert.That(tradeBar.EndTime, Is.GreaterThan(default(DateTime)));
        }
    }

    private static HistoryRequest CreateHistoryRequest(Symbol symbol, Resolution resolution, TickType tickType, DateTime startDateTime, DateTime endDateTime,
                SecurityExchangeHours exchangeHours = null, DateTimeZone dataTimeZone = null)
    {
        if (exchangeHours == null)
        {
            exchangeHours = SecurityExchangeHours.AlwaysOpen(TimeZones.NewYork);
        }

        if (dataTimeZone == null)
        {
            dataTimeZone = TimeZones.NewYork;
        }

        var dataType = LeanData.GetDataType(resolution, tickType);
        return new HistoryRequest(
            startDateTime,
            endDateTime,
            dataType,
            symbol,
            resolution,
            exchangeHours,
            dataTimeZone,
            null,
            true,
            false,
            DataNormalizationMode.Adjusted,
            tickType
            );
    }

    public static Symbol CreateSymbol(string ticker, SecurityType securityType, OptionRight? optionRight = null, decimal? strikePrice = null, DateTime? expirationDate = null, string market = Market.USA)
    {
        switch (securityType)
        {
            case SecurityType.Equity:
                return Symbol.Create(ticker, securityType, market);
            case SecurityType.Option:
                var underlyingEquitySymbol = Symbol.Create(ticker, SecurityType.Equity, market);
                return Symbol.CreateOption(underlyingEquitySymbol, market, SecurityType.Option.DefaultOptionStyle(), optionRight.Value, strikePrice.Value, expirationDate.Value);
            default:
                throw new NotSupportedException($"The security type '{securityType}' is not supported.");
        }
    }
}
