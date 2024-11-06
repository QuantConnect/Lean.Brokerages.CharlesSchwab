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

            return new CharlesSchwabBrokerage(baseUrl, appKey, secret, accountNumber, redirectUrl, authorizationCode, string.Empty, orderProvider, securityProvider);
        }
        return new CharlesSchwabBrokerage(baseUrl, appKey, secret, accountNumber, string.Empty, string.Empty, refreshToken, orderProvider, securityProvider);
    }
    protected override bool IsAsync()
    {
        return false;
    }

    protected override decimal GetAskPrice(Symbol symbol)
    {
        throw new NotImplementedException();
    }

    private static IEnumerable<OrderTestMetaData> OrderTestParameters
    {
        get
        {
            var symbol = Symbol.Create("F", SecurityType.Equity, Market.USA);
            yield return new OrderTestMetaData(OrderType.Market, symbol);
            yield return new OrderTestMetaData(OrderType.Limit, symbol, 11m, 10m);
            yield return new OrderTestMetaData(OrderType.StopMarket, symbol, 11m, 10m);

            var option = Symbol.CreateOption(symbol, symbol.ID.Market, SecurityType.Option.DefaultOptionStyle(), OptionRight.Call, 10m, new DateTime(2024, 11, 08));
            yield return new OrderTestMetaData(OrderType.Market, option);
            yield return new OrderTestMetaData(OrderType.Limit, option, 0.5m, 0.1m);
            yield return new OrderTestMetaData(OrderType.StopMarket, option, 0.5m, 0.1m);
        }
    }

    [TestCaseSource(nameof(OrderTestParameters))]
    public void CancelOrders(OrderTestMetaData orderTestMetaData)
    {
        var parameters = GetOrderTestParameters(orderTestMetaData.OrderType, orderTestMetaData.Symbol, orderTestMetaData.HighLimit, orderTestMetaData.LowLimit);
        CancelOrders(parameters);
    }

    [Test, TestCaseSource(nameof(OrderTestParameters))]
    public void LongFromZero(OrderTestMetaData orderTestMetaData)
    {
        var parameters = GetOrderTestParameters(orderTestMetaData.OrderType, orderTestMetaData.Symbol, orderTestMetaData.HighLimit, orderTestMetaData.LowLimit);
        LongFromZero(parameters);
    }

    [Test, TestCaseSource(nameof(OrderTestParameters))]
    public void CloseFromLong(OrderTestMetaData orderTestMetaData)
    {
        var parameters = GetOrderTestParameters(orderTestMetaData.OrderType, orderTestMetaData.Symbol, orderTestMetaData.HighLimit, orderTestMetaData.LowLimit);
        CloseFromLong(parameters);
    }

    [Test, TestCaseSource(nameof(OrderTestParameters))]
    public void ShortFromZero(OrderTestMetaData orderTestMetaData)
    {
        var parameters = GetOrderTestParameters(orderTestMetaData.OrderType, orderTestMetaData.Symbol, orderTestMetaData.HighLimit, orderTestMetaData.LowLimit);
        ShortFromZero(parameters);
    }

    [Test, TestCaseSource(nameof(OrderTestParameters))]
    public void CloseFromShort(OrderTestMetaData orderTestMetaData)
    {
        var parameters = GetOrderTestParameters(orderTestMetaData.OrderType, orderTestMetaData.Symbol, orderTestMetaData.HighLimit, orderTestMetaData.LowLimit);
        CloseFromShort(parameters);
    }

    [Test, TestCaseSource(nameof(OrderTestParameters))]
    public void ShortFromLong(OrderTestMetaData orderTestMetaData)
    {
        var parameters = GetOrderTestParameters(orderTestMetaData.OrderType, orderTestMetaData.Symbol, orderTestMetaData.HighLimit, orderTestMetaData.LowLimit);
        ShortFromLong(parameters);
    }

    [Test, TestCaseSource(nameof(OrderTestParameters))]
    public void LongFromShort(OrderTestMetaData orderTestMetaData)
    {
        var parameters = GetOrderTestParameters(orderTestMetaData.OrderType, orderTestMetaData.Symbol, orderTestMetaData.HighLimit, orderTestMetaData.LowLimit);
        LongFromShort(parameters);
    }

    /// <summary>
    /// Represents the parameters required for testing an order, including order type, symbol, and price limits.
    /// </summary>
    /// <param name="OrderType">The type of order being tested (e.g., Market, Limit, Stop).</param>
    /// <param name="Symbol">The financial symbol for the order, such as a stock or option ticker.</param>
    /// <param name="HighLimit">The high limit price for the order (if applicable).</param>
    /// <param name="LowLimit">The low limit price for the order (if applicable).</param>
    public record OrderTestMetaData(OrderType OrderType, Symbol Symbol, decimal HighLimit = 0, decimal LowLimit = 0);

    private static OrderTestParameters GetOrderTestParameters(OrderType orderType, Symbol symbol, decimal highLimit, decimal lowLimit)
    {
        return orderType switch
        {
            OrderType.Market => new MarketOrderTestParameters(symbol),
            OrderType.Limit => new LimitOrderTestParameters(symbol, highLimit, lowLimit),
            OrderType.StopMarket => new StopMarketOrderTestParameters(symbol, highLimit, lowLimit),
            _ => throw new NotImplementedException()
        };
    }
}