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
using QuantConnect.Data;
using QuantConnect.Util;
using QuantConnect.Orders;
using QuantConnect.Interfaces;
using QuantConnect.Securities;
using System.Collections.Generic;

namespace QuantConnect.Brokerages.CharlesSchwab;

/// <summary>
/// Represents the Charles Schwab Brokerage implementation.
/// </summary>
[BrokerageFactory(typeof(CharlesSchwabBrokerageFactory))]
public partial class CharlesSchwabBrokerage : Brokerage
{
    /// <summary>
    /// Returns true if we're currently connected to the broker
    /// </summary>
    public override bool IsConnected { get; }

    /// <summary>
    /// Parameterless constructor for brokerage
    /// </summary>
    /// <remarks>This parameterless constructor is required for brokerages implementing <see cref="IDataQueueHandler"/></remarks>
    public CharlesSchwabBrokerage()
        : this(Composer.Instance.GetPart<IDataAggregator>())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="aggregator">consolidate ticks</param>
    public CharlesSchwabBrokerage(IDataAggregator aggregator) : base("CharlesSchwab")
    {
        _aggregator = aggregator;
        _subscriptionManager = new EventBasedDataQueueHandlerSubscriptionManager();
        _subscriptionManager.SubscribeImpl += (s, t) => Subscribe(s);
        _subscriptionManager.UnsubscribeImpl += (s, t) => Unsubscribe(s);

        // Useful for some brokerages:

        // Brokerage helper class to lock websocket message stream while executing an action, for example placing an order
        // avoid race condition with placing an order and getting filled events before finished placing
        // _messageHandler = new BrokerageConcurrentMessageHandler<>();

        // Rate gate limiter useful for API/WS calls
        // _connectionRateLimiter = new RateGate();
    }

    #region Brokerage

    /// <summary>
    /// Gets all open orders on the account.
    /// NOTE: The order objects returned do not have QC order IDs.
    /// </summary>
    /// <returns>The open orders returned from IB</returns>
    public override List<Order> GetOpenOrders()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets all holdings for the account
    /// </summary>
    /// <returns>The current holdings from the account</returns>
    public override List<Holding> GetAccountHoldings()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the current cash balance for each currency held in the brokerage account
    /// </summary>
    /// <returns>The current cash balance for each currency available for trading</returns>
    public override List<CashAmount> GetCashBalance()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Places a new order and assigns a new broker ID to the order
    /// </summary>
    /// <param name="order">The order to be placed</param>
    /// <returns>True if the request for a new order has been placed, false otherwise</returns>
    public override bool PlaceOrder(Order order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Updates the order with the same id
    /// </summary>
    /// <param name="order">The new order information</param>
    /// <returns>True if the request was made for the order to be updated, false otherwise</returns>
    public override bool UpdateOrder(Order order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Cancels the order with the specified ID
    /// </summary>
    /// <param name="order">The order to cancel</param>
    /// <returns>True if the request was made for the order to be canceled, false otherwise</returns>
    public override bool CancelOrder(Order order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Connects the client to the broker's remote servers
    /// </summary>
    public override void Connect()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Disconnects the client from the broker's remote servers
    /// </summary>
    public override void Disconnect()
    {
        throw new NotImplementedException();
    }

    #endregion

    private bool CanSubscribe(Symbol symbol)
    {
        if (symbol.Value.IndexOfInvariant("universe", true) != -1 || symbol.IsCanonical())
        {
            return false;
        }

        throw new NotImplementedException();
    }

    /// <summary>
    /// Adds the specified symbols to the subscription
    /// </summary>
    /// <param name="symbols">The symbols to be added keyed by SecurityType</param>
    private bool Subscribe(IEnumerable<Symbol> symbols)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Removes the specified symbols to the subscription
    /// </summary>
    /// <param name="symbols">The symbols to be removed keyed by SecurityType</param>
    private bool Unsubscribe(IEnumerable<Symbol> symbols)
    {
        throw new NotImplementedException();
    }
}
