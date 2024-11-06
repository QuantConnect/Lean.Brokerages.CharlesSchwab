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
            yield return new TestCaseData(Symbols.AAPL, Resolution.Tick);
            yield return new TestCaseData(Symbols.AAPL, Resolution.Second);
            yield return new TestCaseData(Symbols.AAPL, Resolution.Minute);

        }
    }

    [Test, TestCaseSource(nameof(TestParameters))]
    public void StreamsData(Symbol symbol, Resolution resolution)
    {
        var obj = new object();
        var cancellationTokenSource = new CancellationTokenSource();
        var resetEvent = new AutoResetEvent(false);

        var incomingSymbolDataByTickType = new ConcurrentDictionary<(Symbol, TickType), int>();

        Action<BaseData> callback = (dataPoint) =>
        {
            if (dataPoint == null)
            {
                return;
            }

            switch (dataPoint)
            {
                case Tick tick:
                    switch (tick.TickType)
                    {
                        case TickType.Trade:
                            incomingSymbolDataByTickType[(tick.Symbol, tick.TickType)] += 1;
                            break;
                        case TickType.Quote:
                            incomingSymbolDataByTickType[(tick.Symbol, tick.TickType)] += 1;
                            break;
                    };
                    break;
                case TradeBar tradeBar:
                    incomingSymbolDataByTickType[(tradeBar.Symbol, TickType.Trade)] += 1;
                    break;
                case QuoteBar quoteBar:
                    incomingSymbolDataByTickType[(quoteBar.Symbol, TickType.Quote)] += 1;
                    break;
            }
        };

        var configs = GetSubscriptionDataConfigs(symbol, resolution).ToList();

        foreach (var config in configs)
        {
            incomingSymbolDataByTickType.TryAdd((config.Symbol, config.TickType), 0);
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

        resetEvent.WaitOne(TimeSpan.FromMinutes(2), cancellationTokenSource.Token);

        foreach (var config in configs)
        {
            _brokerage.Unsubscribe(config);
        }

        resetEvent.WaitOne(TimeSpan.FromSeconds(20), cancellationTokenSource.Token);

        var symbolVolatilities = incomingSymbolDataByTickType.Where(kv => kv.Value > 0).ToList();

        Assert.IsNotEmpty(symbolVolatilities);
        Assert.That(symbolVolatilities.Count, Is.GreaterThan(1));

        cancellationTokenSource.Cancel();
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
