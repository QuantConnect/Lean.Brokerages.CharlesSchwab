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
using NUnit.Framework;
using QuantConnect.Tests;
using System.Collections.Generic;

namespace QuantConnect.Brokerages.CharlesSchwab.Tests;

[TestFixture]
public class CharlesSchwabBrokerageSymbolMapperTests
{
    /// <summary>
    /// Provides the mapping between Lean symbols and brokerage specific symbols.
    /// </summary>
    private CharlesSchwabBrokerageSymbolMapper _symbolMapper;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _symbolMapper = new CharlesSchwabBrokerageSymbolMapper();
    }

    [Test]
    public void ReturnsCorrectLeanSymbol()
    {

    }

    private static IEnumerable<TestCaseData> LeanSymbolTestCases
    {
        get
        {
            TestGlobals.Initialize();
            // TSLA - Equity
            var underlying = Symbol.Create("TSLA", SecurityType.Equity, Market.USA);
            yield return new TestCaseData(underlying, "TSLA");
            yield return new TestCaseData(Symbol.CreateOption(underlying, Market.USA, OptionStyle.American, OptionRight.Call, 252.5m, new DateTime(2024, 11, 1)), "TSLA  241101C00252500");
            yield return new TestCaseData(Symbol.CreateOption(underlying, Market.USA, OptionStyle.American, OptionRight.Call, 260m, new DateTime(2024, 12, 06)), "TSLA  241206C00260000");
            yield return new TestCaseData(Symbol.CreateOption(underlying, Market.USA, OptionStyle.American, OptionRight.Put, 252.5m, new DateTime(2024, 11, 01)), "TSLA  241101P00252500");
            // SPX - Index
            var SPX = Symbol.Create("SPX", SecurityType.Index, Market.USA);
            yield return new TestCaseData(SPX, "$SPX");
            yield return new TestCaseData(Symbol.CreateOption(SPX, Market.USA, SecurityType.IndexOption.DefaultOptionStyle(), OptionRight.Call,
                5805m, new DateTime(2024, 11, 15)), "SPX   241115C05805000");
            // SPXW - Index
            var SPXW = Symbol.Create("SPXW", SecurityType.Index, Market.USA);
            yield return new TestCaseData(SPXW, "$SPXW");
            yield return new TestCaseData(Symbol.CreateOption(SPXW, Market.USA, SecurityType.IndexOption.DefaultOptionStyle(), OptionRight.Call,
                5925, new DateTime(2025, 09, 30)), "SPXW  250930C05925000");
            // F - Equity
            var F = Symbol.Create("F", SecurityType.Equity, Market.USA);
            yield return new TestCaseData(F, "F");
            yield return new TestCaseData(Symbol.CreateOption(F, Market.USA, SecurityType.Option.DefaultOptionStyle(), OptionRight.Put, 6.5m, new DateTime(2024, 11, 01)), "F     241101P00006500");
            // DJT - Equity
            var DJT = Symbol.Create("DJT", SecurityType.Equity, Market.USA);
            yield return new TestCaseData(DJT, "DJT");
            yield return new TestCaseData(Symbol.CreateOption(DJT, Market.USA, SecurityType.Option.DefaultOptionStyle(), OptionRight.Call, 1m, new DateTime(2024, 11, 01)), "DJT   241101C00001000");
            // IRBT - Equity
            var IRBT = Symbol.Create("IRBT", SecurityType.Equity, Market.USA);
            yield return new TestCaseData(IRBT, "IRBT");
            yield return new TestCaseData(Symbol.CreateOption(IRBT, Market.USA, SecurityType.Option.DefaultOptionStyle(), OptionRight.Call, 1m, new DateTime(2024, 11, 01)), "IRBT  241101C00001000");
            // GOOGL - Equity
            var GOOGL = Symbol.Create("GOOGL", SecurityType.Equity, Market.USA);
            yield return new TestCaseData(GOOGL, "GOOGL");
            yield return new TestCaseData(Symbol.CreateOption(GOOGL, Market.USA, SecurityType.Option.DefaultOptionStyle(), OptionRight.Put, 180m, new DateTime(2024, 12, 06)), "GOOGL 241206P00180000");
            // DJI - Index
            yield return new TestCaseData(Symbol.Create("DJI", SecurityType.Index, Market.USA), "$DJI");
            // ES - Future
            yield return new TestCaseData(Symbol.CreateFuture("ES", Market.USA, new DateTime(2024, 12, 10)), "/ESZ24");
            yield return new TestCaseData(Symbol.CreateFuture("ES", Market.USA, new DateTime(2024, 5, 10)), "/ESK24");
        }
    }

    [Test, TestCaseSource(nameof(LeanSymbolTestCases))]
    public void ReturnsCorrectBrokerageSymbol(Symbol symbol, string expectedBrokerageSymbol)
    {
        var brokerageSymbol = _symbolMapper.GetBrokerageSymbol(symbol);

        Assert.IsNotNull(brokerageSymbol);
        Assert.IsNotEmpty(brokerageSymbol);
        Assert.That(brokerageSymbol, Is.EqualTo(expectedBrokerageSymbol));
    }
}