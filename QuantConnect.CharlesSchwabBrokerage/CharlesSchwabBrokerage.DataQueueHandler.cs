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
using Newtonsoft.Json;
using QuantConnect.Data;
using QuantConnect.Orders;
using QuantConnect.Packets;
using QuantConnect.Logging;
using QuantConnect.Interfaces;
using QuantConnect.Orders.Fees;
using System.Collections.Generic;
using QuantConnect.Brokerages.CharlesSchwab.Models.Stream;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums.Stream;

namespace QuantConnect.Brokerages.CharlesSchwab;

public partial class CharlesSchwabBrokerage : IDataQueueHandler
{
    /// <summary>
    /// Count number of subscribers for each channel (Symbol, Socket) pair
    /// </summary>
    private EventBasedDataQueueHandlerSubscriptionManager _subscriptionManager;

    /// <summary>
    /// Aggregates ticks and bars based on given subscriptions.
    /// </summary>
    private IDataAggregator _aggregator;

    /// <summary>
    /// Sets the job we're subscribing for
    /// </summary>
    /// <param name="job">Job we're subscribing for</param>
    public void SetJob(LiveNodePacket job)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Subscribe to the specified configuration
    /// </summary>
    /// <param name="dataConfig">defines the parameters to subscribe to a data feed</param>
    /// <param name="newDataAvailableHandler">handler to be fired on new data available</param>
    /// <returns>The new enumerator for this subscription request</returns>
    public IEnumerator<BaseData> Subscribe(SubscriptionDataConfig dataConfig, EventHandler newDataAvailableHandler)
    {
        if (!CanSubscribe(dataConfig.Symbol))
        {
            return null;
        }

        var enumerator = _aggregator.Add(dataConfig, newDataAvailableHandler);
        _subscriptionManager.Subscribe(dataConfig);

        return enumerator;
    }

    protected override bool Subscribe(IEnumerable<Symbol> symbols)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Removes the specified configuration
    /// </summary>
    /// <param name="dataConfig">Subscription config to be removed</param>
    public void Unsubscribe(SubscriptionDataConfig dataConfig)
    {
        _subscriptionManager.Unsubscribe(dataConfig);
        _aggregator.Remove(dataConfig);
    }

    /// <summary>
    /// Removes the specified symbols to the subscription
    /// </summary>
    /// <param name="symbols">The symbols to be removed keyed by SecurityType</param>
    private bool Unsubscribe(IEnumerable<Symbol> symbols)
    {
        throw new NotImplementedException();
    }

    private void onOrderUpdate(object _, AccountContent accountContent)
    {
        _messageHandler.HandleNewMessage(accountContent);
    }

    private void OnUserMessage(AccountContent accountContent)
    {
        switch (accountContent.MessageType)
        {
            case MessageType.OrderUROutCompleted:
                var orderUROut = JsonConvert.DeserializeObject<OrderUROutCompleted>(accountContent.MessageData);

                if (!TryGetLeanOrderByBrokerageId(orderUROut.SchwabOrderID, out var leanOrder))
                {
                    break;
                }

                var leanOrderStatus = default(OrderStatus);
                var message = default(string);
                switch (orderUROut.BaseEvent.OrderUROutCompletedEvent.OutCancelType)
                {
                    case OrderOutCancelType.SystemReject:
                        leanOrderStatus = OrderStatus.Invalid;
                        message = string.Join('\n', orderUROut.BaseEvent.OrderUROutCompletedEvent.ValidationDetail.Select(x => new { Name = x.NgOMSRuleName, Description = x.NgOMSRuleDescription }));
                        break;
                    case OrderOutCancelType.ClientCancel:
                        leanOrderStatus = OrderStatus.Canceled;
                        break;
                }

                OnOrderEvent(
                    new OrderEvent(leanOrder, orderUROut.BaseEvent.OrderUROutCompletedEvent.ExecutionTimeStamp?.DateTime ?? DateTime.UtcNow, OrderFee.Zero, message)
                    { Status = leanOrderStatus });
                break;
            case MessageType.OrderFillCompleted:
                var orderFill = JsonConvert.DeserializeObject<OrderFillCompletedEvent>(accountContent.MessageData);

                if (!TryGetLeanOrderByBrokerageId(orderFill.SchwabOrderID, out leanOrder))
                {
                    break;
                }

                OnOrderEvent(
                    new OrderEvent(leanOrder, orderFill.BaseEvent.OrderFillCompletedEventOrderLegQuantityInfo.ExecutionInfo.ExecutionTimeStamp?.DateTime ?? DateTime.UtcNow, OrderFee.Zero)
                    { Status = OrderStatus.Filled });
                break;
        }
    }

    /// <summary>
    /// Attempts to retrieve a Lean order by its brokerage order ID.
    /// </summary>
    /// <param name="brokerageOrderId">The brokerage order ID to search for.</param>
    /// <param name="leanOrder">
    /// When this method returns, contains the Lean order associated with the specified brokerage order ID, 
    /// if found; otherwise, <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if an order with the specified brokerage ID is found; otherwise, <c>false</c>.
    /// </returns>
    private bool TryGetLeanOrderByBrokerageId(string brokerageOrderId, out Order leanOrder)
    {
        leanOrder = _orderProvider.GetOrdersByBrokerageId(brokerageOrderId).FirstOrDefault();

        if (leanOrder == null)
        {
            Log.Error($"{nameof(CharlesSchwabBrokerage)}.{nameof(TryGetLeanOrderByBrokerageId)}: Order not found: {brokerageOrderId}");
            return false;
        }

        return true;
    }

    protected override void OnMessage(object sender, WebSocketMessage e)
    {
        throw new NotImplementedException();
    }
}
