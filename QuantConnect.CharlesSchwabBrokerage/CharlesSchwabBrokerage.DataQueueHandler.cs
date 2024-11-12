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
using NodaTime;
using System.Linq;
using Newtonsoft.Json;
using QuantConnect.Data;
using QuantConnect.Orders;
using QuantConnect.Packets;
using QuantConnect.Logging;
using QuantConnect.Interfaces;
using QuantConnect.Orders.Fees;
using QuantConnect.Data.Market;
using System.Collections.Generic;
using System.Collections.Concurrent;
using QuantConnect.Brokerages.CharlesSchwab.Extensions;
using QuantConnect.Brokerages.CharlesSchwab.Models.Stream;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums.Stream;

namespace QuantConnect.Brokerages.CharlesSchwab;

public partial class CharlesSchwabBrokerage : IDataQueueHandler
{
    /// <summary>
    /// Aggregates ticks and bars based on given subscriptions.
    /// </summary>
    private IDataAggregator _aggregator;

    /// <summary>
    /// Use like synchronization context for threads
    /// </summary>
    private readonly object _synchronizationContext = new();

    /// <summary>
    /// A thread-safe dictionary that stores the order books by brokerage symbols.
    /// </summary>
    private readonly ConcurrentDictionary<string, DefaultOrderBook> _orderBooks = new();

    /// <summary>
    /// A thread-safe dictionary that maps a <see cref="Symbol"/> to a <see cref="DateTimeZone"/>.
    /// </summary>
    /// <remarks>
    /// This dictionary is used to store the time zone information for each symbol in a concurrent environment,
    /// ensuring thread safety when accessing or modifying the time zone data.
    /// </remarks>
    private readonly ConcurrentDictionary<Symbol, DateTimeZone> _exchangeTimeZoneByLeanSymbol = new();

    /// <summary>
    /// Sets the job we're subscribing for
    /// </summary>
    /// <param name="job">Job we're subscribing for</param>
    public void SetJob(LiveNodePacket job)
    {
        Initialize(
            baseUrl: job.BrokerageData["charles-schwab-api-url"],
            appKey: job.BrokerageData["charles-schwab-app-key"],
            secret: job.BrokerageData["charles-schwab-secret"],
            accountNumber: job.BrokerageData.TryGetValue("charles-schwab-account-number", out var accountNumber) ? accountNumber : string.Empty,
            redirectUrl: job.BrokerageData.TryGetValue("charles-schwab-redirect-url", out var redirectUrl) ? redirectUrl : string.Empty,
            authorizationCodeFromUrl: job.BrokerageData.TryGetValue("charles-schwab-authorization-code-from-url", out var authorizationCode) ? authorizationCode : string.Empty,
            refreshToken: job.BrokerageData.TryGetValue("charles-schwab-refresh-token", out var refreshToken) ? refreshToken : string.Empty,
            orderProvider: null,
            securityProvider: null
            );

        if (!IsConnected)
        {
            Connect();
        }
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
        SubscriptionManager.Subscribe(dataConfig);

        return enumerator;
    }

    /// <summary>
    /// Removes the specified configuration
    /// </summary>
    /// <param name="dataConfig">Subscription config to be removed</param>
    public void Unsubscribe(SubscriptionDataConfig dataConfig)
    {
        SubscriptionManager.Unsubscribe(dataConfig);
        _aggregator.Remove(dataConfig);
    }

    /// <summary>
    /// Handles updates for Level One market data and updates the corresponding order book with the new data.
    /// </summary>
    /// <param name="_">The sender of the event. Unused in this method.</param>
    /// <param name="levelOneContent">The <see cref="LevelOneContent"/> object containing updated market data for a specific symbol.</param>
    /// <remarks>
    /// This method processes market data updates, including ask and bid prices/sizes, and trade ticks. If the market data 
    /// is for an option (i.e., <see cref="LevelOneOptionContent"/>), it also processes open interest and indicative prices.
    /// The method logs an error if the provided symbol is not found in the order books.
    /// </remarks>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if <paramref name="levelOneContent"/> contains a symbol that is not present in the <c>_orderBooks</c>.
    /// </exception>
    private void OnLevelOneMarketDataUpdate(object _, LevelOneContent levelOneContent)
    {
        if (_orderBooks.TryGetValue(levelOneContent.Symbol, out var orderBook))
        {
            if (levelOneContent.AskSize > 0 && levelOneContent.AskPrice > 0)
            {
                orderBook.UpdateAskRow(levelOneContent.AskPrice, levelOneContent.AskSize);
            }
            else if (levelOneContent.AskSize == 0 && levelOneContent.AskPrice != 0)
            {
                orderBook.RemoveAskRow(levelOneContent.AskPrice);
            }

            if (levelOneContent.BidSize > 0 && levelOneContent.BidPrice > 0)
            {
                orderBook.UpdateBidRow(levelOneContent.BidPrice, levelOneContent.BidSize);
            }
            else if (levelOneContent.BidSize == 0 && levelOneContent.BidPrice != 0)
            {
                orderBook.RemoveBidRow(levelOneContent.BidPrice);
            }

            if (levelOneContent.LastSize > 0 && levelOneContent.LastPrice > 0)
            {
                EmitTradeTick(orderBook.Symbol, levelOneContent.LastPrice, levelOneContent.LastSize, levelOneContent.TradeTime);
            }

            if (levelOneContent is LevelOneOptionContent levelOneOption)
            {
                if (levelOneOption.OpenInterest != 0)
                {
                    EmitOpenInterestTick(orderBook.Symbol, levelOneOption.OpenInterest);
                }

                // TODO: Should we send trade?
                if (levelOneOption.IndicativeAskPrice != 0)
                {
                    orderBook.UpdateAskRow(levelOneOption.IndicativeAskPrice, 0m);
                }

                if (levelOneOption.IndicativeBidPrice != 0)
                {
                    orderBook.UpdateBidRow(levelOneOption.IndicativeBidPrice, 0m);
                }
            }
        }
        else
        {
            Log.Error($"{nameof(CharlesSchwabBrokerage)}.{nameof(OnLevelOneMarketDataUpdate)}: Symbol {levelOneContent.Symbol} not found in order books. This could indicate an unexpected symbol or a missing initialization step.");
        }
    }

    /// <summary>
    /// Processes an update to an order by delegating it to the message handler.
    /// </summary>
    /// <param name="_">The sender of the event. This parameter is unused.</param>
    /// <param name="accountContent">The <see cref="AccountContent"/> containing the update information for the order.</param>
    private void OnOrderUpdate(object _, AccountContent accountContent)
    {
        _messageHandler.HandleNewMessage(accountContent);
    }

    /// <summary>
    /// Processes user messages related to order updates and fills.
    /// </summary>
    /// <param name="accountContent">The <see cref="AccountContent"/> containing details about the user message.</param>
    /// <remarks>
    /// This method handles different types of user messages including completed order updates and order fills.
    /// Depending on the <see cref="MessageType"/>, it updates the order status and emits an order event.
    /// </remarks>
    private void OnUserMessage(AccountContent accountContent)
    {
        switch (accountContent.MessageType)
        {
            case MessageType.OrderUROutCompleted:
                var orderUROut = JsonConvert.DeserializeObject<OrderUROutCompleted>(accountContent.MessageData);

                if (!TryGetLeanOrderByBrokerageId(orderUROut.SchwabOrderID, out var leanOrder))
                {
                    // If order was removed successfully, we should skip log error message 
                    if (!_tempUpdateBrokerageId.TryRemove(orderUROut.SchwabOrderID, out _))
                    {
                        Log.Error($"{nameof(CharlesSchwabBrokerage)}.{nameof(TryGetLeanOrderByBrokerageId)}: Order not found: {orderUROut.SchwabOrderID}. Order detail: {accountContent.MessageData}");
                    }
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
                    Log.Error($"{nameof(CharlesSchwabBrokerage)}.{nameof(TryGetLeanOrderByBrokerageId)}: Order not found: {orderFill.SchwabOrderID}. Order detail: {accountContent.MessageData}");
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
            return false;
        }

        return true;
    }

    /// <summary>
    /// Not used
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    protected override void OnMessage(object sender, WebSocketMessage e)
    {
        // This method is currently not used.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Subscribes to real-time market data for the specified symbols.
    /// </summary>
    /// <param name="symbols">An <see cref="IEnumerable{T}"/> collection of <see cref="Symbol"/> objects representing the symbols to subscribe to.</param>
    /// <returns><c>true</c> if the subscription process completes successfully.</returns>
    /// <exception cref="NotImplementedException">
    /// Thrown if the security type of a symbol is not supported for the subscription process.
    /// </exception>
    protected override bool Subscribe(IEnumerable<Symbol> symbols)
    {
        foreach (var symbol in symbols)
        {
            var brokerageSymbol = AddOrderBook(symbol);
            switch (symbol.SecurityType)
            {
                case SecurityType.Equity:
                case SecurityType.Index:
                    (WebSocket as CharlesSchwabWebSocketClientWrapper).SubscribeOnLevelOneEquity(brokerageSymbol);
                    break;
                case SecurityType.Option:
                case SecurityType.IndexOption:
                    (WebSocket as CharlesSchwabWebSocketClientWrapper).SubscribeOnLevelOneOption(brokerageSymbol);
                    break;
                default:
                    throw new NotImplementedException($"{nameof(CharlesSchwabBrokerage)}.{nameof(Subscribe)}: The security type '{symbol.SecurityType}' is not supported subscription process.");
            }
        }
        return true;
    }

    /// <summary>
    /// Unsubscribes from real-time market data for the specified symbols.
    /// </summary>
    /// <param name="symbols">An <see cref="IEnumerable{T}"/> collection of <see cref="Symbol"/> objects representing the symbols to unsubscribe from.</param>
    /// <returns><c>true</c> if the unsubscription process completes successfully.</returns>
    /// <exception cref="NotImplementedException">
    /// Thrown if the security type of a symbol is not supported for the unsubscription process.
    /// </exception>
    private bool Unsubscribe(IEnumerable<Symbol> symbols)
    {
        foreach (var symbol in symbols)
        {
            var brokerageSymbol = RemoveOrderBook(symbol);
            switch (symbol.SecurityType)
            {
                case SecurityType.Equity:
                case SecurityType.Index:
                    (WebSocket as CharlesSchwabWebSocketClientWrapper).UnSubscribeOnLevelOneEquity(brokerageSymbol);
                    break;
                case SecurityType.Option:
                case SecurityType.IndexOption:
                    (WebSocket as CharlesSchwabWebSocketClientWrapper).UnSubscribeOnLevelOneOption(brokerageSymbol);
                    break;
                default:
                    throw new NotImplementedException($"{nameof(CharlesSchwabBrokerage)}.{nameof(Unsubscribe)}: The security type '{symbol.SecurityType}' is not supported unSubscription process.");
            }
        }

        return true;
    }

    /// <summary>
    /// Adds an order book for the specified symbol if it does not already exist.
    /// </summary>
    /// <param name="symbol">The symbol for which the order book is to be added.</param>
    private string AddOrderBook(Symbol symbol)
    {
        var exchangeTimeZone = symbol.GetSymbolExchangeTimeZone();
        _exchangeTimeZoneByLeanSymbol[symbol] = exchangeTimeZone;

        var brokerageSymbol = _symbolMapper.GetBrokerageSymbol(symbol);

        if (!_orderBooks.TryGetValue(brokerageSymbol, out var orderBook))
        {
            _orderBooks[brokerageSymbol] = new DefaultOrderBook(symbol);
            _orderBooks[brokerageSymbol].BestBidAskUpdated += OnBestBidAskUpdated;
        }

        return brokerageSymbol;
    }

    /// <summary>
    /// Removes the order book for the specified symbol if it exists.
    /// </summary>
    /// <param name="symbol">The symbol for which the order book is to be removed.</param>
    private string RemoveOrderBook(Symbol symbol)
    {
        _exchangeTimeZoneByLeanSymbol.Remove(symbol, out _);

        var brokerageSymbol = _symbolMapper.GetBrokerageSymbol(symbol);

        if (_orderBooks.TryRemove(brokerageSymbol, out var orderBook))
        {
            orderBook.BestBidAskUpdated -= OnBestBidAskUpdated;
        }

        return brokerageSymbol;
    }

    /// <summary>
    /// Handles updates to the best bid and ask prices and updates the aggregator with a new quote tick.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="bestBidAskUpdatedEvent">The event arguments containing best bid and ask details.</param>
    private void OnBestBidAskUpdated(object sender, BestBidAskUpdatedEventArgs bestBidAskUpdatedEvent)
    {
        if (!_exchangeTimeZoneByLeanSymbol.TryGetValue(bestBidAskUpdatedEvent.Symbol, out var exchangeTimeZone))
        {
            return;
        }

        var tick = new Tick
        {
            AskPrice = bestBidAskUpdatedEvent.BestAskPrice,
            BidPrice = bestBidAskUpdatedEvent.BestBidPrice,
            Time = DateTime.UtcNow.ConvertFromUtc(exchangeTimeZone),
            Symbol = bestBidAskUpdatedEvent.Symbol,
            TickType = TickType.Quote,
            AskSize = bestBidAskUpdatedEvent.BestAskSize,
            BidSize = bestBidAskUpdatedEvent.BestBidSize
        };
        tick.SetValue();

        lock (_synchronizationContext)
        {
            _aggregator.Update(tick);
        }
    }

    /// <summary>
    /// Emits a open interest tick with the provided details and updates the aggregator.
    /// </summary>
    /// <param name="symbol">The symbol of the traded instrument.</param>
    /// <param name="openInterest">The open interest size.</param>
    private void EmitOpenInterestTick(Symbol symbol, decimal openInterest)
    {
        if (!_exchangeTimeZoneByLeanSymbol.TryGetValue(symbol, out var exchangeTimeZone))
        {
            return;
        }

        var openInterestTick = new Tick(DateTime.UtcNow.ConvertFromUtc(exchangeTimeZone), symbol, openInterest);

        lock (_synchronizationContext)
        {
            _aggregator.Update(openInterestTick);
        }
    }

    /// <summary>
    /// Emits a trade tick with the provided details and updates the aggregator.
    /// </summary>
    /// <param name="symbol">The symbol of the traded instrument.</param>
    /// <param name="price">The trade price.</param>
    /// <param name="size">The trade size.</param>
    /// <param name="tradeTime">The time of the trade.</param>
    private void EmitTradeTick(Symbol symbol, decimal price, decimal size, DateTime tradeTime)
    {
        if (!_exchangeTimeZoneByLeanSymbol.TryGetValue(symbol, out var exchangeTimeZone))
        {
            return;
        }

        var tradeTick = new Tick
        {
            Value = price,
            Time = tradeTime.ConvertFromUtc(exchangeTimeZone),
            Symbol = symbol,
            TickType = TickType.Trade,
            Quantity = size
        };

        lock (_synchronizationContext)
        {
            _aggregator.Update(tradeTick);
        }
    }
}
