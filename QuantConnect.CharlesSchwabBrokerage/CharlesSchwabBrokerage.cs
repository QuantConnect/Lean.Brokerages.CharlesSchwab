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
using QuantConnect.Logging;
using QuantConnect.Securities;
using QuantConnect.Configuration;
using System.Collections.Generic;
using QuantConnect.Brokerages.CharlesSchwab.Api;
using QuantConnect.Brokerages.CharlesSchwab.Extensions;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums;

namespace QuantConnect.Brokerages.CharlesSchwab;

/// <summary>
/// Represents the Charles Schwab Brokerage implementation.
/// </summary>
[BrokerageFactory(typeof(CharlesSchwabBrokerageFactory))]
public partial class CharlesSchwabBrokerage : Brokerage
{
    /// <summary>
    /// Represents the name of the market or broker being used, in this case, "Charles Schwab".
    /// </summary>
    private static readonly string MarketName = "CharlesSchwab"; /// TODO: Replace this with the reference to the market name from Lean, like `Market.CharlesSchwab`.

    /// <summary>
    /// Returns true if we're currently connected to the broker
    /// </summary>
    public override bool IsConnected { get; }

    /// <summary>
    /// Indicates whether the initialization process has already been completed.
    /// </summary>
    private bool _isInitialized;

    /// <summary>
    /// CharlesSchwab api client implementation.
    /// </summary>
    private CharlesSchwabApiClient _charlesSchwabApiClient;

    /// <summary>
    /// Provides the mapping between Lean symbols and brokerage specific symbols.
    /// </summary>
    private CharlesSchwabBrokerageSymbolMapper _symbolMapper;

    public CharlesSchwabBrokerage() : base(MarketName)
    {
    }

    public CharlesSchwabBrokerage(string baseUrl, string appKey, string secret, string accountNumber, string redirectUrl, string authorizationCodeFromUrl,
        string refreshToken) : base(MarketName)
    {
        Initialize(baseUrl, appKey, secret, accountNumber, redirectUrl, authorizationCodeFromUrl, refreshToken);
    }

    protected void Initialize(string baseUrl, string appKey, string secret, string accountNumber, string redirectUrl, string authorizationCodeFromUrl,
        string refreshToken)
    {
        if (_isInitialized)
        {
            return;
        }
        _isInitialized = true;

        _aggregator = Composer.Instance.GetPart<IDataAggregator>();
        if (_aggregator == null)
        {
            var aggregatorName = Config.Get("data-aggregator", "QuantConnect.Lean.Engine.DataFeeds.AggregationManager");
            Log.Trace($"{nameof(CharlesSchwabBrokerage)}.{nameof(Initialize)}: found no data aggregator instance, creating {aggregatorName}");
            _aggregator = Composer.Instance.GetExportedValueByTypeName<IDataAggregator>(aggregatorName);
        }


        _subscriptionManager = new EventBasedDataQueueHandlerSubscriptionManager();
        _subscriptionManager.SubscribeImpl += (s, t) => Subscribe(s);
        _subscriptionManager.UnsubscribeImpl += (s, t) => Unsubscribe(s);

        _symbolMapper = new CharlesSchwabBrokerageSymbolMapper();
        _charlesSchwabApiClient = new CharlesSchwabApiClient(baseUrl, appKey, secret, accountNumber, redirectUrl, authorizationCodeFromUrl, refreshToken);

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
        var brokerageOpenOrders = _charlesSchwabApiClient.GetOpenOrders().SynchronouslyAwaitTaskResult();
        var leanOrders = new List<Order>();
        foreach (var brokerageOrder in brokerageOpenOrders)
        {
            var leanOrder = default(Order);

            var leg = brokerageOrder.OrderLegCollection[0];
            // TODO: What about Market ?? 
            var leanSymbol = _symbolMapper.GetLeanSymbol(leg.Instrument.Symbol, leg.OrderLegType.ConvertCharlesSchwabAssetTypeToLeanSecurityType(), Market.USA);
            switch (brokerageOrder.OrderType)
            {
                case CharlesSchwabOrderType.Market:
                    leanOrder = new MarketOrder(leanSymbol, leg.Quantity, brokerageOrder.ReleaseTime, brokerageOrder.Tag);
                    break;
                case CharlesSchwabOrderType.Limit:
                    leanOrder = new LimitOrder(leanSymbol, leg.Quantity, brokerageOrder.Price, brokerageOrder.ReleaseTime, brokerageOrder.Tag);
                    break;
                case CharlesSchwabOrderType.Stop:
                    leanOrder = new StopMarketOrder(leanSymbol, leg.Quantity, brokerageOrder.StopPrice, brokerageOrder.ReleaseTime, brokerageOrder.Tag);
                    break;
                case CharlesSchwabOrderType.StopLimit:
                    leanOrder = new StopLimitOrder(leanSymbol, leg.Quantity, brokerageOrder.StopPrice, brokerageOrder.Price, brokerageOrder.ReleaseTime, brokerageOrder.Tag);
                    break;
                // TODO: case CharlesSchwabOrderType.TrailingStop:
                case CharlesSchwabOrderType.MarketOnClose:
                    leanOrder = new MarketOnCloseOrder(leanSymbol, leg.Quantity, brokerageOrder.ReleaseTime, brokerageOrder.Tag);
                    break;
            }
            // TODO: Validate the ternary conditional operator
            leanOrder.Status = brokerageOrder.FilledQuantity > 0m && brokerageOrder.FilledQuantity != brokerageOrder.Quantity ? OrderStatus.PartiallyFilled : OrderStatus.Submitted;
            leanOrder.BrokerId.Add(brokerageOrder.OrderId.ToStringInvariant());
        }
        return leanOrders;
    }

    /// <summary>
    /// Gets all holdings for the account
    /// </summary>
    /// <returns>The current holdings from the account</returns>
    public override List<Holding> GetAccountHoldings()
    {
        var positions = _charlesSchwabApiClient.GetAccountBalanceAndPosition().SynchronouslyAwaitTaskResult().Positions;

        var holdings = new List<Holding>();
        foreach (var position in positions)
        {
            // TODO: SymbolMapper
            var leanSymbol = _symbolMapper.GetLeanSymbol(position.Instrument.Symbol, SecurityType.Equity, Market.USA);

            holdings.Add(new Holding()
            {
                AveragePrice = position.AveragePrice,
                CurrencySymbol = Currencies.USD,
                MarketValue = position.MarketValue,
                // TODO: Api Response received sign with quantity?
                Quantity = position.ShortQuantity != 0 ? position.ShortQuantity : position.LongQuantity,
                Symbol = leanSymbol,
                UnrealizedPnL = position.CurrentDayProfitLoss,
                UnrealizedPnLPercent = position.CurrentDayProfitLossPercentage
            });
        }

        return holdings;
    }

    /// <summary>
    /// Gets the current cash balance for each currency held in the brokerage account
    /// </summary>
    /// <returns>The current cash balance for each currency available for trading</returns>
    public override List<CashAmount> GetCashBalance()
    {
        var currentBalance = _charlesSchwabApiClient.GetAccountBalanceAndPosition().SynchronouslyAwaitTaskResult().CurrentBalances;
        return new List<CashAmount>() { new CashAmount(currentBalance.CashBalance, Currencies.USD) };
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
