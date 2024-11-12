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
using QuantConnect.Orders;
using QuantConnect.Logging;
using QuantConnect.Interfaces;
using QuantConnect.Securities;
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
        return TestSetup.CreateBrokerage(orderProvider, securityProvider, forceCreateBrokerageInstance: true);
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
            yield return new OrderTestMetaData(OrderType.Limit, symbol, 12m, 10m);
            yield return new OrderTestMetaData(OrderType.StopMarket, symbol, 12m, 10m);

            var option = Symbol.CreateOption(symbol, symbol.ID.Market, SecurityType.Option.DefaultOptionStyle(), OptionRight.Call, 11m, new DateTime(2024, 11, 15));
            yield return new OrderTestMetaData(OrderType.Market, option);
            yield return new OrderTestMetaData(OrderType.Limit, option, 0.18m, 0.1m);
            yield return new OrderTestMetaData(OrderType.StopMarket, option, 0.5m, 0.1m);
        }
    }

    private static IEnumerable<OrderTestMetaData> LimitOrderTestParameters => OrderTestParameters.Where(o => o.OrderType == OrderType.Limit);

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

    [TestCaseSource(nameof(LimitOrderTestParameters))]
    public void LongFromZeroUpdateAndCancel(OrderTestMetaData orderTestMetaData)
    {
        Log.Trace("");
        Log.Trace("LONG FROM ZERO THEN UPDATE AND CANCEL");
        Log.Trace("");

        var parameters = GetOrderTestParameters(orderTestMetaData.OrderType, orderTestMetaData.Symbol, orderTestMetaData.HighLimit, orderTestMetaData.LowLimit);

        var order = PlaceOrderWaitForStatus(parameters.CreateLongOrder(GetDefaultQuantity()), parameters.ExpectedStatus) as LimitOrder;

        using var updatedOrderStatusEvent = new AutoResetEvent(false);
        Brokerage.OrdersStatusChanged += (_, orderEvents) =>
        {
            var eventOrderStatus = orderEvents[0].Status;

            order.Status = eventOrderStatus;

            switch (eventOrderStatus)
            {
                case OrderStatus.UpdateSubmitted:
                    updatedOrderStatusEvent.Set();
                    break;
            }
        };

        Brokerage.OrderIdChanged += (_, args) =>
        {
            Log.Trace($"ORDER ID CHANGED EVENT: Id = {args.OrderId}, BrokerageId = [{string.Join(',', args.BrokerId)}]");
        };

        var updateOrderRequest = new UpdateOrderRequest(DateTime.UtcNow, order.Id, new()
        {
            LimitPrice = order.LimitPrice + 0.01m,
            Quantity = order.Quantity + 1m
        });

        order.ApplyUpdateOrderRequest(updateOrderRequest);

        if (!Brokerage.UpdateOrder(order) || !updatedOrderStatusEvent.WaitOne(TimeSpan.FromSeconds(5)))
        {
            Assert.Fail("Order is not updated well.");
        }
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