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
using System.Linq;
using NUnit.Framework;
using System.Threading;
using QuantConnect.Data;
using QuantConnect.Tests;
using QuantConnect.Logging;
using System.Threading.Tasks;
using QuantConnect.Data.Market;
using System.Collections.Generic;
using System.Collections.Concurrent;
using QuantConnect.Lean.Engine.DataFeeds.Enumerators;

namespace QuantConnect.Brokerages.CharlesSchwab.Tests;

[TestFixture]
public partial class CharlesSchwabBrokerageTests
{
    private CharlesSchwabBrokerage _brokerage;

    [SetUp]
    public void SetUp()
    {
        _brokerage = (CharlesSchwabBrokerage)Brokerage;
    }

    private static IEnumerable<TestCaseData> TestParameters
    {
        get
        {
            var AAPL = Symbols.AAPL;
            yield return new TestCaseData(new[] { AAPL }, Resolution.Tick);
            yield return new TestCaseData(new[] { AAPL }, Resolution.Second);
            yield return new TestCaseData(new[] { AAPL }, Resolution.Minute);

            var DJI_Index = Symbol.Create("DJI", SecurityType.Index, Market.USA);
            yield return new TestCaseData(new[] { DJI_Index }, Resolution.Tick);

            var NVDA = Symbol.Create("NVDA", SecurityType.Equity, Market.USA);
            var DJT = Symbol.Create("DJT", SecurityType.Equity, Market.USA);
            var TSLA = Symbol.Create("TSLA", SecurityType.Equity, Market.USA);
            yield return new TestCaseData(new[] { AAPL, DJI_Index, NVDA, DJT, TSLA }, Resolution.Tick);
            yield return new TestCaseData(new[] { AAPL, DJI_Index, NVDA, DJT, TSLA }, Resolution.Second);
            yield return new TestCaseData(new[] { AAPL, DJI_Index, NVDA, DJT, TSLA }, Resolution.Minute);

            var AAPL_Option = Symbol.CreateOption(AAPL, AAPL.ID.Market, SecurityType.Option.DefaultOptionStyle(), OptionRight.Call, 230m, new DateTime(2024, 11, 29));
            yield return new TestCaseData(new[] { AAPL_Option }, Resolution.Tick);

            yield return new TestCaseData(new[] { AAPL, AAPL_Option }, Resolution.Tick);

            var SPX = Symbol.Create("SPX", SecurityType.Index, Market.USA);
            var SPX_IndexOption = Symbol.CreateOption(SPX, SPX.ID.Market, SecurityType.IndexOption.DefaultOptionStyle(), OptionRight.Put, 5000m, new DateTime(2024, 11, 29));
            yield return new TestCaseData(new[] { SPX, SPX_IndexOption }, Resolution.Tick);
        }
    }

    [Test, TestCaseSource(nameof(TestParameters))]
    public void StreamsData(Symbol[] symbol, Resolution resolution)
    {
        var obj = new object();
        var cancellationTokenSource = new CancellationTokenSource();
        var resetEvent = new AutoResetEvent(false);

        var incomingSymbolDataByTickType = new ConcurrentDictionary<(Symbol, TickType), List<BaseData>>();
        var configs = symbol.SelectMany(s => GetSubscriptionDataConfigs(s, resolution)).ToList();

        Action<BaseData> callback = (dataPoint) =>
        {
            if (dataPoint == null)
            {
                return;
            }

            switch (dataPoint)
            {
                case Tick tick:
                    AddOrUpdateDataPoint(incomingSymbolDataByTickType, tick.Symbol, tick.TickType, tick);
                    break;
                case TradeBar tradeBar:
                    AddOrUpdateDataPoint(incomingSymbolDataByTickType, tradeBar.Symbol, TickType.Trade, tradeBar);
                    break;
                case QuoteBar quoteBar:
                    AddOrUpdateDataPoint(incomingSymbolDataByTickType, quoteBar.Symbol, TickType.Quote, quoteBar);
                    break;
            }

            lock (obj)
            {
                if (incomingSymbolDataByTickType.Count == configs.Count && incomingSymbolDataByTickType.All(d => d.Value.Count > 2))
                {
                    resetEvent.Set();
                }
            }
        };

        foreach (var config in configs)
        {
            ProcessFeed(_brokerage.Subscribe(config, (sender, args) =>
            {
                var dataPoint = ((NewDataAvailableEventArgs)args).DataPoint;
                Log.Trace($"{dataPoint}. Time span: {dataPoint.Time} - {dataPoint.EndTime}");
            }),
            cancellationTokenSource.Token,
            300,
            callback: callback,
            throwExceptionCallback: () => cancellationTokenSource.Cancel());
        }

        resetEvent.WaitOne(TimeSpan.FromMinutes(5), cancellationTokenSource.Token);

        foreach (var config in configs)
        {
            _brokerage.Unsubscribe(config);
        }

        resetEvent.WaitOne(TimeSpan.FromSeconds(5), cancellationTokenSource.Token);

        var symbolVolatilities = incomingSymbolDataByTickType.Where(kv => kv.Value.Count > 0).ToList();

        Assert.IsNotEmpty(symbolVolatilities);
        Assert.That(symbolVolatilities.Count, Is.GreaterThan(1));

        cancellationTokenSource.Cancel();
    }

    private void AddOrUpdateDataPoint(
    ConcurrentDictionary<(Symbol, TickType), List<BaseData>> dictionary,
    Symbol symbol,
    TickType tickType,
    BaseData dataPoint)
    {
        dictionary.AddOrUpdate(
            (symbol, tickType),
            new List<BaseData> { dataPoint }, // Add scenario: create a new list with the dataPoint
            (key, existingList) =>
            {
                existingList.Add(dataPoint); // Add dataPoint to the existing list
                return existingList; // Return the updated list
            }
        );
    }


    private static IEnumerable<SubscriptionDataConfig> GetSubscriptionDataConfigs(Symbol symbol, Resolution resolution)
    {
        if (resolution == Resolution.Tick)
        {
            return GetSubscriptionTickDataConfigs(symbol);
        }

        return new[]
        {
            GetSubscriptionDataConfig<TradeBar>(symbol, resolution),
            GetSubscriptionDataConfig<QuoteBar>(symbol, resolution)
        };
    }

    private static IEnumerable<SubscriptionDataConfig> GetSubscriptionTickDataConfigs(Symbol symbol)
    {
        yield return new SubscriptionDataConfig(GetSubscriptionDataConfig<Tick>(symbol, Resolution.Tick), tickType: TickType.Trade);
        yield return new SubscriptionDataConfig(GetSubscriptionDataConfig<Tick>(symbol, Resolution.Tick), tickType: TickType.Quote);
    }

    private Task ProcessFeed(
        IEnumerator<BaseData> enumerator,
        CancellationToken cancellationToken,
        int cancellationTokenDelayMilliseconds = 100,
        Action<BaseData> callback = null,
        Action throwExceptionCallback = null)
    {
        return Task.Factory.StartNew(() =>
        {
            try
            {
                while (enumerator.MoveNext() && !cancellationToken.IsCancellationRequested)
                {
                    BaseData tick = enumerator.Current;

                    if (tick != null)
                    {
                        callback?.Invoke(tick);
                    }

                    cancellationToken.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(cancellationTokenDelayMilliseconds));
                }
            }
            catch (Exception ex)
            {
                Log.Debug($"{nameof(CharlesSchwabBrokerageTests)}.{nameof(ProcessFeed)}.Exception: {ex.Message}");
                throw;
            }
        }, cancellationToken).ContinueWith(task =>
        {
            if (throwExceptionCallback != null)
            {
                throwExceptionCallback();
            }
            Log.Debug("The throwExceptionCallback is null.");
        }, TaskContinuationOptions.OnlyOnFaulted);
    }
}
