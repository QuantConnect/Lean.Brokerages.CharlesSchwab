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
using QuantConnect.Brokerages.CharlesSchwab.Extensions;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums;

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

    [TestCase("F", AssetType.Equity, "F", null, null, null)]
    [TestCase("AAPL", AssetType.Equity, "AAPL", null, null, null)]
    [TestCase("F     241101C00010500", AssetType.Option, "F", 10.5, "2024/11/01", OptionRight.Call, Description = "FORD MTR CO DEL 11/01/2024 $10.5 Call")]
    [TestCase("GOOGL 250815C00180000", AssetType.Option, "GOOGL", 180, "2025/08/15", OptionRight.Call, Description = "GOOGL 08/15/2025 180.00 C")]
    public void ReturnsCorrectLeanSymbol(string brokerageSymbol, AssetType assetType, string expectedSymbol, decimal? expectedStrike, DateTime? expectedExpiryDateTime, OptionRight? expectedOptionRight)
    {
        var leanSymbol = _symbolMapper.GetLeanSymbol(brokerageSymbol, assetType.ConvertAssetTypeToSecurityType(), Market.USA);

        Assert.IsNotNull(leanSymbol);

        switch (leanSymbol.SecurityType)
        {
            case SecurityType.Equity:
                Assert.AreEqual(expectedSymbol, leanSymbol.Value);
                break;
            case SecurityType.Option:
                Assert.AreEqual(expectedSymbol, leanSymbol.Underlying.Value);
                Assert.AreEqual(expectedStrike, leanSymbol.ID.StrikePrice);
                Assert.AreEqual(expectedExpiryDateTime, leanSymbol.ID.Date);
                Assert.AreEqual(expectedOptionRight, leanSymbol.ID.OptionRight);
                break;
            default:
                throw new NotImplementedException($"{nameof(CharlesSchwabBrokerageSymbolMapperTests)}.{nameof(ReturnsCorrectLeanSymbol)}: SecurityType '{leanSymbol.SecurityType}' is not implemented.");
        }
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