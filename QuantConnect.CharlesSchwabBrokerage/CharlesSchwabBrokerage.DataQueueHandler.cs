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
            case "OrderUROutCompleted":
                var orderUROut = JsonConvert.DeserializeObject<OrderUROutCompleted>(accountContent.MessageData);
                var leanOrder = _orderProvider.GetOrdersByBrokerageId(orderUROut.SchwabOrderID).FirstOrDefault();

                if (leanOrder == null)
                {
                    Log.Error($"{nameof(CharlesSchwabBrokerage)}.{nameof(OnUserMessage)}: Order od not found: {orderUROut.SchwabOrderID}");
                    break;
                }

                var orderEvent = new OrderEvent(leanOrder, orderUROut.BaseEvent.OrderUROutCompletedEvent.ExecutionTimeStamp.DateTime, OrderFee.Zero) { Status = OrderStatus.Canceled };
                OnOrderEvent(orderEvent);
                break;
            case "OrderAccepted":
                break;
        }
    }

    protected override void OnMessage(object sender, WebSocketMessage e)
    {
        throw new NotImplementedException();
    }
}
