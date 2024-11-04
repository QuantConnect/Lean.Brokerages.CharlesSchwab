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
using QuantConnect.Orders;
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
        return true;
    }

    protected override decimal GetAskPrice(Symbol symbol)
    {
        throw new System.NotImplementedException();
    }


    /// <summary>
    /// Provides the data required to test each order type in various cases
    /// </summary>
    private static IEnumerable<TestCaseData> OrderParameters
    {
        get
        {
            var symbol = Symbol.Create("F", SecurityType.Equity, Market.USA);
            yield return new TestCaseData(new MarketOrderTestParameters(symbol));
            yield return new TestCaseData(new LimitOrderTestParameters(symbol, 11m, 10m));
            yield return new TestCaseData(new StopLimitOrderTestParameters(symbol, 11m, 10m));
            yield return new TestCaseData(new StopMarketOrderTestParameters(symbol, 11m, 10m));
        }
    }

    [Test, TestCaseSource(nameof(OrderParameters))]
    public override void CancelOrders(OrderTestParameters parameters)
    {
        base.CancelOrders(parameters);
    }

    [Test, TestCaseSource(nameof(OrderParameters))]
    public override void LongFromZero(OrderTestParameters parameters)
    {
        base.LongFromZero(parameters);
    }

    [Test, TestCaseSource(nameof(OrderParameters))]
    public override void CloseFromLong(OrderTestParameters parameters)
    {
        base.CloseFromLong(parameters);
    }

    [Test, TestCaseSource(nameof(OrderParameters))]
    public override void ShortFromZero(OrderTestParameters parameters)
    {
        base.ShortFromZero(parameters);
    }

    [Test, TestCaseSource(nameof(OrderParameters))]
    public override void CloseFromShort(OrderTestParameters parameters)
    {
        base.CloseFromShort(parameters);
    }

    [Test, TestCaseSource(nameof(OrderParameters))]
    public override void ShortFromLong(OrderTestParameters parameters)
    {
        base.ShortFromLong(parameters);
    }

    [Test, TestCaseSource(nameof(OrderParameters))]
    public override void LongFromShort(OrderTestParameters parameters)
    {
        base.LongFromShort(parameters);
    }

    [TestCase(0.0001, false)]
    [TestCase(0.01, true)]
    public void TrailingStopOrder(decimal trailingAmount, bool trailingAsPercentage)
    {
        var trailingStopOrder = new TrailingStopOrder(Symbol, GetDefaultQuantity(), trailingAmount, trailingAsPercentage, DateTime.UtcNow);

        Assert.IsTrue(Brokerage.PlaceOrder(trailingStopOrder));
    }
}