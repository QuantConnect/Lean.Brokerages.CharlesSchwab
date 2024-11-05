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
using QuantConnect.Interfaces;
using QuantConnect.Securities;
using QuantConnect.Configuration;
using System.Collections.Generic;
using QuantConnect.Tests.Brokerages;

namespace QuantConnect.Brokerages.CharlesSchwab.Tests;

[TestFixture]
public partial class CharlesSchwabBrokerageTests : BrokerageTests
{
    protected override Symbol Symbol { get; } = Symbol.Create("F", SecurityType.Equity, Market.USA);
    protected override SecurityType SecurityType { get; }

    protected override IBrokerage CreateBrokerage(IOrderProvider orderProvider, ISecurityProvider securityProvider)
    {
        var baseUrl = Config.Get("charles-schwab-api-url");
        var appKey = Config.Get("charles-schwab-app-key");
        var secret = Config.Get("charles-schwab-secret");
        var accountNumber = Config.Get("charles-schwab-account-number");

        var refreshToken = Config.Get("charles-schwab-refresh-token");
        if (string.IsNullOrEmpty(refreshToken))
        {
            var redirectUrl = Config.Get("charles-schwab-redirect-url");
            var authorizationCode = Config.Get("charles-schwab-authorization-code-from-url");

            if (new string[] { redirectUrl, authorizationCode }.Any(string.IsNullOrEmpty))
            {
                throw new ArgumentException("RedirectUrl or AuthorizationCode cannot be empty or null. Please ensure these values are correctly set in the configuration file.");
            }

            return new CharlesSchwabBrokerage(baseUrl, appKey, secret, accountNumber, redirectUrl, authorizationCode, string.Empty, orderProvider);
        }
        return new CharlesSchwabBrokerage(baseUrl, appKey, secret, accountNumber, string.Empty, string.Empty, refreshToken, orderProvider);
    }
    protected override bool IsAsync()
    {
        return false;
    }

    protected override decimal GetAskPrice(Symbol symbol)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Provides the data required to test each order type in various cases
    /// </summary>
    private static IEnumerable<TestCaseData> EquityOrderParameters
    {
        get
        {
            var symbol = Symbol.Create("F", SecurityType.Equity, Market.USA);
            yield return new TestCaseData(new MarketOrderTestParameters(symbol)).SetCategory("Equity");
            yield return new TestCaseData(new LimitOrderTestParameters(symbol, 11m, 10m)).SetCategory("Equity");
            yield return new TestCaseData(new StopMarketOrderTestParameters(symbol, 11m, 10m)).SetCategory("Equity");
        }
    }

    private static IEnumerable<TestCaseData> OptionOrderParameters
    {
        get
        {
            var symbol = Symbol.Create("F", SecurityType.Equity, Market.USA);
            var option = Symbol.CreateOption(symbol, symbol.ID.Market, SecurityType.Option.DefaultOptionStyle(), OptionRight.Call, 10m, new DateTime(2024, 11, 08));
            yield return new TestCaseData(new MarketOrderTestParameters(option)).SetCategory("Option");
            yield return new TestCaseData(new LimitOrderTestParameters(option, 0.5m, 0.1m)).SetCategory("Option");
            yield return new TestCaseData(new StopMarketOrderTestParameters(option, 0.5m, 0.1m)).SetCategory("Option");
        }
    }

    #region Equtiy

    [TestCaseSource(nameof(EquityOrderParameters))]
    public void CancelEquityOrders(OrderTestParameters parameters)
    {
        CancelOrders(parameters);
    }

    [Test, TestCaseSource(nameof(EquityOrderParameters))]
    public void LongFromZeroEquityOrders(OrderTestParameters parameters)
    {
        LongFromZero(parameters);
    }

    [Test, TestCaseSource(nameof(EquityOrderParameters))]
    public void CloseFromLongEquityOrders(OrderTestParameters parameters)
    {
        CloseFromLong(parameters);
    }

    [Test, TestCaseSource(nameof(EquityOrderParameters))]
    public void ShortFromZeroEquityOrders(OrderTestParameters parameters)
    {
        ShortFromZero(parameters);
    }

    [Test, TestCaseSource(nameof(EquityOrderParameters))]
    public void CloseFromShortEquityOrders(OrderTestParameters parameters)
    {
        CloseFromShort(parameters);
    }

    [Test, TestCaseSource(nameof(EquityOrderParameters))]
    public void ShortFromLongEquityOrders(OrderTestParameters parameters)
    {
        ShortFromLong(parameters);
    }

    [Test, TestCaseSource(nameof(EquityOrderParameters))]
    public void LongFromShortEquityOrders(OrderTestParameters parameters)
    {
        LongFromShort(parameters);
    }

    #endregion

    #region Option

    [TestCaseSource(nameof(OptionOrderParameters))]
    public void CancelOptionOrders(OrderTestParameters parameters)
    {
        CancelOrders(parameters);
    }

    #endregion
}