using NUnit.Framework;
using QuantConnect.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace QuantConnect.Brokerages.CharlesSchwab.Tests;

public partial class CharlesSchwabBrokerageTests
{
    public record ComboLimitPriceByOptionContracts(decimal ComboLimitPrice, IReadOnlyCollection<OptionContractByQuantity> OptionContracts);
    public record OptionContractByQuantity(Symbol Symbol, decimal Quantity);

    private static IEnumerable<ComboLimitPriceByOptionContracts> ComboOrderTestParameters
    {
        get
        {
            var F_Equity = Symbol.Create("F", SecurityType.Equity, Market.USA);
            var options = new List<OptionContractByQuantity>
            {
                new (Symbol.CreateOption(F_Equity, Market.USA, SecurityType.Option.DefaultOptionStyle(), OptionRight.Call, 9m, new DateTime(2024, 12, 13)), 1),
                new (Symbol.CreateOption(F_Equity, Market.USA, SecurityType.Option.DefaultOptionStyle(), OptionRight.Put, 13.5m, new DateTime(2024, 12, 13)), 1),
            };
            yield return new(0.02m, options);

            var VIX_Index = Symbol.Create("VIX", SecurityType.Index, Market.USA);
            var indexOptions = new List<OptionContractByQuantity>
                {
                    new (Symbol.CreateOption(VIX_Index, Market.USA, SecurityType.IndexOption.DefaultOptionStyle(), OptionRight.Call, 12m, new DateTime(2024, 12, 18)), 1),
                    new (Symbol.CreateOption(VIX_Index, Market.USA, SecurityType.IndexOption.DefaultOptionStyle(), OptionRight.Put, 15m, new DateTime(2024, 12, 18)), 1)
                };

            yield return new(0.01m, indexOptions);

        }
    }

    [TestCaseSource(nameof(ComboOrderTestParameters))]
    public void PlaceComboLimitOrder(ComboLimitPriceByOptionContracts comboLimitPriceByOptionContracts)
    {
        var (limitPrice, optionContracts) = comboLimitPriceByOptionContracts;
        var groupOrderManager = new GroupOrderManager(1, legCount: optionContracts.Count, quantity: 2, limitPrice);

        var comboOrders = PlaceComboOrder(
            optionContracts,
            limitPrice,
            (optionContract, quantity, price, groupOrderManager) =>
                new ComboLimitOrder(optionContract, quantity.GetOrderLegGroupQuantity(groupOrderManager), price.Value, DateTime.UtcNow, groupOrderManager),
            groupOrderManager);

        AssertComboOrderPlacedSuccessfully(comboOrders);
        CancelComboOpenOrders(comboOrders);
    }

    private void AssertComboOrderPlacedSuccessfully<T>(IReadOnlyCollection<T> comboOrders) where T : ComboOrder
    {
        Assert.IsTrue(comboOrders.All(o => o.Status.IsClosed() || o.Status == OrderStatus.Submitted));
    }

    private IReadOnlyCollection<T> PlaceComboOrder<T>(
    IReadOnlyCollection<OptionContractByQuantity> legs,
    decimal? orderLimitPrice,
    Func<Symbol, decimal, decimal?, GroupOrderManager, T> orderType, GroupOrderManager groupOrderManager) where T : ComboOrder
    {
        var comboOrders = legs
            .Select(optionContract => orderType(optionContract.Symbol, optionContract.Quantity, orderLimitPrice, groupOrderManager))
            .ToList().AsReadOnly();

        var manualResetEvent = new ManualResetEvent(false);
        var orderStatusCallback = HandleComboOrderStatusChange(comboOrders, manualResetEvent, OrderStatus.Submitted);

        Brokerage.OrdersStatusChanged += orderStatusCallback;

        foreach (var comboOrder in comboOrders)
        {
            OrderProvider.Add(comboOrder);
            groupOrderManager.OrderIds.Add(comboOrder.Id);
            Assert.IsTrue(Brokerage.PlaceOrder(comboOrder));
        }

        Assert.IsTrue(manualResetEvent.WaitOne(TimeSpan.FromSeconds(60)));

        Brokerage.OrdersStatusChanged -= orderStatusCallback;

        return comboOrders;
    }

    private void CancelComboOpenOrders(IReadOnlyCollection<ComboLimitOrder> comboLimitOrders)
    {
        using var manualResetEvent = new ManualResetEvent(false);

        var orderStatusCallback = HandleComboOrderStatusChange(comboLimitOrders, manualResetEvent, OrderStatus.Canceled);

        Brokerage.OrdersStatusChanged += orderStatusCallback;

        var openOrders = OrderProvider.GetOpenOrders(order => order.Type == OrderType.ComboLimit);
        foreach (var openOrder in openOrders)
        {
            Assert.IsTrue(Brokerage.CancelOrder(openOrder));
        }

        if (openOrders.Count > 0)
        {
            Assert.IsTrue(manualResetEvent.WaitOne(TimeSpan.FromSeconds(60)));
        }

        Brokerage.OrdersStatusChanged -= orderStatusCallback;
    }

    private static EventHandler<List<OrderEvent>> HandleComboOrderStatusChange<T>(
    IReadOnlyCollection<T> comboOrders,
    ManualResetEvent manualResetEvent,
    OrderStatus expectedOrderStatus) where T : ComboOrder
    {
        return (_, orderEvents) =>
        {

            foreach (var order in comboOrders)
            {
                foreach (var orderEvent in orderEvents)
                {
                    if (orderEvent.OrderId == order.Id)
                    {
                        order.Status = orderEvent.Status;
                    }
                }

                if (comboOrders.All(o => o.Status.IsClosed()) || comboOrders.All(o => o.Status == expectedOrderStatus))
                {
                    manualResetEvent.Set();
                }
            }
        };
    }
}
