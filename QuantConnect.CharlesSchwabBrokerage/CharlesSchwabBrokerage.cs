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
using System.IO;
using RestSharp;
using System.Net;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Threading;
using QuantConnect.Api;
using QuantConnect.Data;
using QuantConnect.Util;
using QuantConnect.Orders;
using Newtonsoft.Json.Linq;
using QuantConnect.Logging;
using QuantConnect.Securities;
using QuantConnect.Interfaces;
using QuantConnect.Orders.Fees;
using System.Collections.Generic;
using QuantConnect.Configuration;
using System.Security.Cryptography;
using System.Net.NetworkInformation;
using System.Collections.Concurrent;
using QuantConnect.Brokerages.CharlesSchwab.Api;
using QuantConnect.Brokerages.CharlesSchwab.Extensions;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums;
using QuantConnect.Brokerages.CharlesSchwab.Models.Stream;
using QuantConnect.Brokerages.CharlesSchwab.Models.Requests;
using CharlesSchwabOrderType = QuantConnect.Brokerages.CharlesSchwab.Models.Enums.OrderType;

namespace QuantConnect.Brokerages.CharlesSchwab;

/// <summary>
/// Represents the Charles Schwab Brokerage implementation.
/// </summary>
[BrokerageFactory(typeof(CharlesSchwabBrokerageFactory))]
public partial class CharlesSchwabBrokerage : BaseWebsocketsBrokerage
{
    /// <summary>
    /// Represents the name of the market or broker being used, in this case, "Charles Schwab".
    /// </summary>
    private static readonly string MarketName = "CharlesSchwab"; /// TODO: Replace this with the reference to the market name from Lean, like `Market.CharlesSchwab`.

    /// <summary>
    /// Returns true if we're currently connected to the broker
    /// </summary>
    public override bool IsConnected => WebSocket.IsOpen;

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

    /// <summary>
    /// Handles incoming account content messages and processes them using the <see cref="BrokerageConcurrentMessageHandler{T}"/>.
    /// </summary>
    private BrokerageConcurrentMessageHandler<AccountContent> _messageHandler;

    /// <summary>
    /// Signals to a <see cref="CancellationToken"/> that it should be canceled.
    /// </summary>
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    /// <summary>
    /// A thread-safe collection used to temporarily store brokerage IDs to bypass log errors 
    /// if an order is not found or has been successfully processed.
    /// </summary>
    private readonly ConcurrentDictionary<string, bool> _tempUpdateBrokerageId = new();

    /// <summary>
    /// A synchronization event used to signal the completion of an order update process.
    /// </summary>
    private readonly AutoResetEvent _pendingUpdateOrderEvent = new(false);

    /// <summary>
    /// Order provider
    /// </summary>
    private IOrderProvider _orderProvider;

    /// <summary>
    /// Represents a type capable of fetching the holdings for the specified symbol
    /// </summary>
    private ISecurityProvider _securityProvider;

    /// <summary>
    /// Parameterless constructor for brokerage
    /// </summary>
    public CharlesSchwabBrokerage() : base(MarketName)
    {
    }

    /// <summary>
    /// Constructor for the Charles Schwab brokerage.
    /// </summary>
    /// <param name="baseUrl">The URL to connect to brokerage environment</param>
    /// <param name="appKey">The API app key for authentication.</param>
    /// <param name="secret">The API key secret for authentication.</param>
    /// <param name="accountNumber">The specific user account number.</param>
    /// <param name="redirectUrl">The redirect URL to generate great link to get right "authorizationCodeFromUrl"</param>
    /// <param name="authorizationCodeFromUrl">The authorization code obtained from the URL.</param>
    /// <param name="refreshToken">The refresh token used to obtain new access tokens for authentication.</param>
    /// <param name="algorithm">The algorithm instance is required to retrieve account type.</param>
    public CharlesSchwabBrokerage(string baseUrl, string appKey, string secret, string accountNumber, string redirectUrl, string authorizationCodeFromUrl,
        string refreshToken, IAlgorithm algorithm)
        : this(baseUrl, appKey, secret, accountNumber, redirectUrl, authorizationCodeFromUrl, refreshToken, algorithm?.Portfolio?.Transactions, algorithm?.Portfolio)
    { }

    /// <summary>
    /// Constructor for the Charles Schwab brokerage.
    /// </summary>
    /// <param name="baseUrl">The URL to connect to brokerage environment</param>
    /// <param name="appKey">The API app key for authentication.</param>
    /// <param name="secret">The API key secret for authentication.</param>
    /// <param name="accountNumber">The specific user account number.</param>
    /// <param name="redirectUrl">The redirect URL to generate great link to get right "authorizationCodeFromUrl"</param>
    /// <param name="authorizationCodeFromUrl">The authorization code obtained from the URL.</param>
    /// <param name="refreshToken">The refresh token used to obtain new access tokens for authentication.</param>
    /// <param name="orderProvider">The order provider.</param>
    /// <param name="securityProvider">The type capable of fetching the holdings for the specified symbol.</param>
    public CharlesSchwabBrokerage(string baseUrl, string appKey, string secret, string accountNumber, string redirectUrl, string authorizationCodeFromUrl,
        string refreshToken, IOrderProvider orderProvider, ISecurityProvider securityProvider) : base(MarketName)
    {
        Initialize(baseUrl, appKey, secret, accountNumber, redirectUrl, authorizationCodeFromUrl, refreshToken, orderProvider, securityProvider);
    }

    /// <summary>
    /// Initializer for the Charles Schwab brokerage.
    /// </summary>
    /// <param name="baseUrl">The URL to connect to brokerage environment</param>
    /// <param name="appKey">The API app key for authentication.</param>
    /// <param name="secret">The API key secret for authentication.</param>
    /// <param name="accountNumber">The specific user account number.</param>
    /// <param name="redirectUrl">The redirect URL to generate great link to get right "authorizationCodeFromUrl"</param>
    /// <param name="authorizationCodeFromUrl">The authorization code obtained from the URL.</param>
    /// <param name="refreshToken">The refresh token used to obtain new access tokens for authentication.</param>
    /// <param name="orderProvider">The order provider.</param>
    /// <param name="securityProvider">The type capable of fetching the holdings for the specified symbol.</param>
    protected void Initialize(string baseUrl, string appKey, string secret, string accountNumber, string redirectUrl, string authorizationCodeFromUrl,
        string refreshToken, IOrderProvider orderProvider, ISecurityProvider securityProvider)
    {
        if (_isInitialized)
        {
            return;
        }
        _isInitialized = true;

        _orderProvider = orderProvider;
        _securityProvider = securityProvider;
        _aggregator = Composer.Instance.GetPart<IDataAggregator>();
        if (_aggregator == null)
        {
            var aggregatorName = Config.Get("data-aggregator", "QuantConnect.Lean.Engine.DataFeeds.AggregationManager");
            Log.Trace($"{nameof(CharlesSchwabBrokerage)}.{nameof(Initialize)}: found no data aggregator instance, creating {aggregatorName}");
            _aggregator = Composer.Instance.GetExportedValueByTypeName<IDataAggregator>(aggregatorName);
        }

        ValidateSubscription();

        _symbolMapper = new CharlesSchwabBrokerageSymbolMapper();
        _charlesSchwabApiClient = new CharlesSchwabApiClient(baseUrl, appKey, secret, accountNumber, redirectUrl, authorizationCodeFromUrl, refreshToken);

        WebSocket = new CharlesSchwabWebSocketClientWrapper(_charlesSchwabApiClient, OnOrderUpdate, OnLevelOneMarketDataUpdate, OnReSubscriptionProcess);
        _messageHandler = new BrokerageConcurrentMessageHandler<AccountContent>(OnUserMessage);

        SubscriptionManager = new EventBasedDataQueueHandlerSubscriptionManager()
        {
            SubscribeImpl = (symbols, _) => Subscribe(symbols),
            UnsubscribeImpl = (symbols, _) => Unsubscribe(symbols)
        };

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
        var brokerageOpenOrders = _charlesSchwabApiClient.GetAllOrders().SynchronouslyAwaitTaskResult();
        var leanOrders = new List<Order>();
        foreach (var brokerageOrder in brokerageOpenOrders.Where(o => o.Status.IsOrderOpen()))
        {
            var leanOrder = default(Order);

            var orderProperties = new CharlesSchwabOrderProperties()
            {
                ExtendedRegularTradingHours = brokerageOrder.Session.IsExtendedRegularTradingHoursBySessionType()
            };

            if (!orderProperties.GetLeanTimeInForce(brokerageOrder.Duration, brokerageOrder.CancelTime))
            {
                OnMessage(new BrokerageMessageEvent(BrokerageMessageType.Warning, -1, $"Detected unsupported Lean TimeInForce of '{brokerageOrder.Duration}', ignoring. Using default: TimeInForce.GoodTilCanceled"));
            }

            var leg = brokerageOrder.OrderLegCollection[0];

            if (!TryGetLeanSymbol(leg.Instrument.Symbol, leg.Instrument.AssetType, out var leanSymbol, out var exceptionMessage))
            {
                OnMessage(new BrokerageMessageEvent(BrokerageMessageType.Warning, 1, $"{exceptionMessage}. Order details: {brokerageOrder}."));
                continue;
            }

            var orderQuantity = leg.Instruction.IsShort() ? decimal.Negate(leg.Quantity) : leg.Quantity;
            switch (brokerageOrder.OrderType)
            {
                case CharlesSchwabOrderType.Market:
                    leanOrder = new MarketOrder(leanSymbol, orderQuantity, brokerageOrder.EnteredTime, brokerageOrder.Tag, orderProperties);
                    break;
                case CharlesSchwabOrderType.Limit:
                    leanOrder = new LimitOrder(leanSymbol, orderQuantity, brokerageOrder.Price, brokerageOrder.EnteredTime, brokerageOrder.Tag, orderProperties);
                    break;
                case CharlesSchwabOrderType.Stop:
                    leanOrder = new StopMarketOrder(leanSymbol, orderQuantity, brokerageOrder.StopPrice, brokerageOrder.EnteredTime, brokerageOrder.Tag, orderProperties);
                    break;
                default:
                    Log.Trace($"{nameof(CharlesSchwabBrokerage)}.{nameof(GetOpenOrders)}: Skipping unsupported order type '{brokerageOrder.OrderType}'. Order details: {brokerageOrder}.");
                    continue;
            }
            leanOrder.Status = brokerageOrder.FilledQuantity > 0m && brokerageOrder.FilledQuantity != brokerageOrder.Quantity ? Orders.OrderStatus.PartiallyFilled : Orders.OrderStatus.Submitted;
            leanOrder.BrokerId.Add(brokerageOrder.OrderId.ToStringInvariant());

            leanOrders.Add(leanOrder);
        }
        return leanOrders;
    }

    /// <summary>
    /// Gets all holdings for the account
    /// </summary>
    /// <returns>The current holdings from the account</returns>
    public override List<Holding> GetAccountHoldings()
    {
        var positions = _charlesSchwabApiClient.GetAccountBalanceAndPosition().SynchronouslyAwaitTaskResult()?.Positions;

        var holdings = new List<Holding>();
        if (positions == null)
        {
            return holdings;
        }

        foreach (var position in positions)
        {
            if (!TryGetLeanSymbol(position.Instrument.Symbol, position.Instrument.AssetType, out var leanSymbol, out var exceptionMessage))
            {
                OnMessage(new BrokerageMessageEvent(BrokerageMessageType.Warning, 1, $"{exceptionMessage}. Position details: {position}."));
                continue;
            }

            holdings.Add(new Holding()
            {
                AveragePrice = position.AveragePrice,
                CurrencySymbol = Currencies.USD,
                MarketPrice = position.MarketValue,
                MarketValue = (position.ShortQuantity != 0 ? position.ShortQuantity : position.LongQuantity) * position.MarketValue,
                ConversionRate = 1.0m,
                Quantity = position.ShortQuantity != 0 ? decimal.Negate(position.ShortQuantity) : position.LongQuantity,
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
        if (!CanSubmitOrder(order.Symbol))
        {
            OnMessage(new BrokerageMessageEvent(BrokerageMessageType.Warning, -1, $"Symbol is not supported {order.Symbol}"));
            return false;
        }

        var orderRequest = CreateBrokerageOrderRequest(order);

        _messageHandler.WithLockedStream(() =>
        {
            var brokerageOrderId = default(string);
            try
            {
                brokerageOrderId = _charlesSchwabApiClient.PlaceOrder(orderRequest).SynchronouslyAwaitTaskResult();
            }
            catch (Exception ex)
            {
                OnOrderEvent(new OrderEvent(order, DateTime.UtcNow, OrderFee.Zero, $"Charles Schwab: Place Order Event: {ex.Message}")
                {
                    Status = Orders.OrderStatus.Invalid
                });
                return;
            }

            order.BrokerId.Add(brokerageOrderId);

            OnOrderEvent(new OrderEvent(order, DateTime.UtcNow, OrderFee.Zero, "Charles Schwab: Place Order Event")
            {
                Status = Orders.OrderStatus.Submitted
            });
        });

        return true;
    }

    /// <summary>
    /// Updates the order with the same id
    /// </summary>
    /// <param name="order">The new order information</param>
    /// <returns>True if the request was made for the order to be updated, false otherwise</returns>
    public override bool UpdateOrder(Order order)
    {
        var oldBrokerageId = order.BrokerId.Last();

        var orderRequest = CreateBrokerageOrderRequest(order);

        var newBrokerageId = default(string);
        var catchException = default(bool);
        _messageHandler.WithLockedStream(() =>
        {
            try
            {
                newBrokerageId = _charlesSchwabApiClient.UpdateOrder(oldBrokerageId, orderRequest).SynchronouslyAwaitTaskResult();
            }
            catch (Exception ex)
            {
                catchException = true;
                OnMessage(new BrokerageMessageEvent(BrokerageMessageType.Warning, -1, "Charles Schwab.Update Order: " + ex.Message));
                return;
            }

            _tempUpdateBrokerageId[oldBrokerageId] = false;
            _tempUpdateBrokerageId[newBrokerageId] = true;
        });

        if (catchException)
        {
            return false;
        }

        var updated = default(bool);
        if (_pendingUpdateOrderEvent.WaitOne(TimeSpan.FromSeconds(5), _cancellationTokenSource.Token))
        {
            _messageHandler.WithLockedStream(() =>
            {
                updated = true;

                order.BrokerId.Remove(oldBrokerageId);
                order.BrokerId.Add(newBrokerageId);

                OnOrderIdChangedEvent(new() { BrokerId = order.BrokerId, OrderId = order.Id });

                OnOrderEvent(new OrderEvent(order, DateTime.UtcNow, OrderFee.Zero, "Charles Schwab: Update Order Event")
                {
                    Status = Orders.OrderStatus.UpdateSubmitted
                });

                // the order update process has completed successfully.
                _tempUpdateBrokerageId.TryRemove(newBrokerageId, out var _);
            });
        }
        else
        {
            _tempUpdateBrokerageId.TryRemove(oldBrokerageId, out _);
        }

        return updated;
    }

    /// <summary>
    /// Cancels the order with the specified ID
    /// </summary>
    /// <param name="order">The order to cancel</param>
    /// <returns>True if the request was made for the order to be canceled, false otherwise</returns>
    public override bool CancelOrder(Order order)
    {
        if (order.Status == Orders.OrderStatus.Filled)
        {
            OnMessage(new BrokerageMessageEvent(BrokerageMessageType.Warning, -1, "Charles Schwab.Cancel Order: Order already filled"));
            return false;
        }

        if (order.Status == Orders.OrderStatus.Canceled)
        {
            OnMessage(new BrokerageMessageEvent(BrokerageMessageType.Warning, -1, "Charles Schwab.Cancel Order: Order already canceled"));
            return false;
        }

        var brokerageId = order.BrokerId.Last();

        var canceled = default(bool);
        _messageHandler.WithLockedStream(() =>
        {
            try
            {
                canceled = _charlesSchwabApiClient.CancelOrderById(brokerageId).SynchronouslyAwaitTaskResult();
            }
            catch (Exception ex)
            {
                OnMessage(new BrokerageMessageEvent(BrokerageMessageType.Warning, -1, "Charles Schwab.Cancel Order: " + ex.Message));
            }
        });

        return canceled;
    }

    /// <summary>
    /// Connects the client to the broker's remote servers
    /// </summary>
    public override void Connect()
    {
        if (IsConnected)
        {
            return;
        }

        base.Connect();
    }

    /// <summary>
    /// Disconnects the client from the broker's remote servers
    /// </summary>
    public override void Disconnect()
    {
        WebSocket.Close();
    }

    #endregion

    /// <summary>
    /// Determines whether a symbol can be subscribed to.
    /// </summary>
    /// <param name="symbol">The symbol to check for subscription eligibility.</param>
    /// <returns>
    ///   <c>true</c> if the symbol can be subscribed to; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method checks if the provided symbol is eligible for subscription based on certain criteria.
    /// Symbols containing the substring "universe" or those identified as canonical are not eligible for subscription.
    /// </remarks>
    private bool CanSubscribe(Symbol symbol)
    {
        if (symbol.Value.IndexOfInvariant("universe", true) != -1 || symbol.IsCanonical())
        {
            return false;
        }

        return _symbolMapper.SupportedSecurityType.Contains(symbol.SecurityType);
    }

    /// <summary>
    /// Determines whether an order can be submitted for a given symbol.
    /// </summary>
    /// <param name="symbol">The symbol to check for order submission eligibility.</param>
    /// <returns>
    ///   <c>true</c> if the order can be submitted for the symbol; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method checks if the provided symbol meets the criteria for order submission.
    /// Symbols with a security type of <see cref="SecurityType.Index"/> are not eligible.
    /// Additionally, the method ensures that the symbol is eligible for subscription by calling <see cref="CanSubscribe(Symbol)"/>.
    /// </remarks>
    private bool CanSubmitOrder(Symbol symbol)
    {
        return symbol.SecurityType != SecurityType.Index && CanSubscribe(symbol);
    }

    /// <summary>
    /// Determines the appropriate trading instruction based on the order direction, security type, and holdings quantity.
    /// </summary>
    /// <param name="orderDirection">The direction of the order (e.g., Buy or Sell).</param>
    /// <param name="securityType">The type of security being traded (e.g., Equity, Index, Option).</param>
    /// <param name="holdingsQuantity">The current quantity of holdings for the security.</param>
    /// <returns>
    /// The corresponding <see cref="Instruction"/> for the given order direction, security type, and holdings quantity.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when the specified order position or order direction is not supported.
    /// </exception>
    private static Instruction GetInstructionByDirection(OrderDirection orderDirection, SecurityType securityType, decimal holdingsQuantity)
    {
        var orderPosition = GetOrderPosition(orderDirection, holdingsQuantity);

        switch (securityType)
        {
            case SecurityType.Equity:
            case SecurityType.Index:
            case SecurityType.Option:
            case SecurityType.IndexOption:
                return orderPosition switch
                {
                    OrderPosition.BuyToOpen => securityType.IsOption() ? Instruction.BuyToOpen : Instruction.Buy,
                    OrderPosition.SellToOpen => securityType.IsOption() ? Instruction.SellToOpen : Instruction.SellShort,
                    OrderPosition.BuyToClose => securityType.IsOption() ? Instruction.BuyToClose : Instruction.BuyToCover,
                    OrderPosition.SellToClose => securityType.IsOption() ? Instruction.SellToClose : Instruction.Sell,
                    _ => throw new NotSupportedException($"{nameof(CharlesSchwabBrokerage)}.{nameof(GetInstructionByDirection)}: The specified order position '{orderPosition}' is not supported.")
                };
            default:
                return orderDirection switch
                {
                    OrderDirection.Sell => Instruction.Sell,
                    OrderDirection.Buy => Instruction.Buy,
                    _ => throw new NotSupportedException($"{nameof(CharlesSchwabBrokerage)}.{nameof(GetInstructionByDirection)}: The specified order direction '{orderDirection}' is not supported.")
                };
        }
    }

    /// <summary>
    /// Creates an order request object for the specified order.
    /// </summary>
    /// <param name="order">The Lean <see cref="Order"/> for which the request is being created.</param>
    /// <returns>An <see cref="OrderBaseRequest"/> object representing the brokerage order request.</returns>
    /// <exception cref="NotSupportedException">Thrown when the order type is not supported.</exception>
    private OrderBaseRequest CreateBrokerageOrderRequest(Order order)
    {
        var (duration, cancelTime) = order.Properties.TimeInForce.GetDurationByTimeInForce();
        var sessionType = order.Properties.GetExtendedHoursSessionTypeOrDefault(SessionType.Normal);

        var holdingQuantity = _securityProvider.GetHoldingsQuantity(order.Symbol);
        var instruction = GetInstructionByDirection(order.Direction, order.SecurityType, holdingQuantity);

        var symbol = _symbolMapper.GetBrokerageSymbol(order.Symbol);
        var assetType = order.SecurityType.ConvertSecurityTypeToAssetType();

        OrderBaseRequest orderRequest;
        switch (order)
        {
            case MarketOrder:
                orderRequest = new MarketOrderRequest(instruction, order.AbsoluteQuantity, symbol, assetType);
                break;
            case LimitOrder lo:
                orderRequest = new LimitOrderRequest(sessionType, duration, instruction, order.AbsoluteQuantity, symbol, assetType, lo.LimitPrice);
                break;
            case StopMarketOrder smo when order.Type == Orders.OrderType.StopMarket:
                orderRequest = new StopMarketOrderRequest(duration, instruction, order.AbsoluteQuantity, symbol, assetType, smo.StopPrice);
                break;
            default:
                throw new NotSupportedException($"{nameof(CharlesSchwabBrokerage)}.{nameof(CreateBrokerageOrderRequest)}: The order type '{order.GetType().Name}' is not supported.");
        };

        if (cancelTime.HasValue)
        {
            orderRequest.CancelTime = cancelTime.Value;
        }

        return orderRequest;
    }

    /// <summary>
    /// Attempts to map a given symbol and asset type to a Lean <see cref="Symbol"/>.
    /// </summary>
    /// <param name="symbol">The string representation of the financial instrument's symbol.</param>
    /// <param name="assetType">The type of the asset (e.g., Stock, Option).</param>
    /// <param name="leanSymbol">The resulting Lean <see cref="Symbol"/> if the mapping is successful.</param>
    /// <param name="exceptionMessage">The error message if the mapping fails.</param>
    /// <returns>
    /// <c>true</c> if the mapping succeeds; otherwise, <c>false</c>.
    /// </returns>
    private bool TryGetLeanSymbol(string symbol, AssetType assetType, out Symbol leanSymbol, out string exceptionMessage)
    {
        leanSymbol = default;
        exceptionMessage = default;
        try
        {
            leanSymbol = _symbolMapper.GetLeanSymbol(symbol, assetType.ConvertAssetTypeToSecurityType(), Market.USA);
            return true;
        }
        catch (Exception ex)
        {
            exceptionMessage = ex.Message;
            return false;
        }
    }

    #region ValidateSubscription
    private class ModulesReadLicenseRead : QuantConnect.Api.RestResponse
    {
        [JsonProperty(PropertyName = "license")]
        public string License;
        [JsonProperty(PropertyName = "organizationId")]
        public string OrganizationId;
    }

    /// <summary>
    /// Validate the user of this project has permission to be using it via our web API.
    /// </summary>
    private static void ValidateSubscription()
    {
        try
        {
            const int productId = 367;
            var userId = Globals.UserId;
            var token = Globals.UserToken;
            var organizationId = Globals.OrganizationID;
            // Verify we can authenticate with this user and token
            var api = new ApiConnection(userId, token);
            if (!api.Connected)
            {
                throw new ArgumentException("Invalid api user id or token, cannot authenticate subscription.");
            }
            // Compile the information we want to send when validating
            var information = new Dictionary<string, object>()
                {
                    {"productId", productId},
                    {"machineName", Environment.MachineName},
                    {"userName", Environment.UserName},
                    {"domainName", Environment.UserDomainName},
                    {"os", Environment.OSVersion}
                };
            // IP and Mac Address Information
            try
            {
                var interfaceDictionary = new List<Dictionary<string, object>>();
                foreach (var nic in NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up))
                {
                    var interfaceInformation = new Dictionary<string, object>();
                    // Get UnicastAddresses
                    var addresses = nic.GetIPProperties().UnicastAddresses
                        .Select(uniAddress => uniAddress.Address)
                        .Where(address => !IPAddress.IsLoopback(address)).Select(x => x.ToString());
                    // If this interface has non-loopback addresses, we will include it
                    if (!addresses.IsNullOrEmpty())
                    {
                        interfaceInformation.Add("unicastAddresses", addresses);
                        // Get MAC address
                        interfaceInformation.Add("MAC", nic.GetPhysicalAddress().ToString());
                        // Add Interface name
                        interfaceInformation.Add("name", nic.Name);
                        // Add these to our dictionary
                        interfaceDictionary.Add(interfaceInformation);
                    }
                }
                information.Add("networkInterfaces", interfaceDictionary);
            }
            catch (Exception)
            {
                // NOP, not necessary to crash if fails to extract and add this information
            }
            // Include our OrganizationId is specified
            if (!string.IsNullOrEmpty(organizationId))
            {
                information.Add("organizationId", organizationId);
            }
            var request = new RestRequest("modules/license/read", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("application/json", JsonConvert.SerializeObject(information), ParameterType.RequestBody);
            api.TryRequest(request, out ModulesReadLicenseRead result);
            if (!result.Success)
            {
                throw new InvalidOperationException($"Request for subscriptions from web failed, Response Errors : {string.Join(',', result.Errors)}");
            }

            var encryptedData = result.License;
            // Decrypt the data we received
            DateTime? expirationDate = null;
            long? stamp = null;
            bool? isValid = null;
            if (encryptedData != null)
            {
                // Fetch the org id from the response if we are null, we need it to generate our validation key
                if (string.IsNullOrEmpty(organizationId))
                {
                    organizationId = result.OrganizationId;
                }
                // Create our combination key
                var password = $"{token}-{organizationId}";
                var key = SHA256.HashData(Encoding.UTF8.GetBytes(password));
                // Split the data
                var info = encryptedData.Split("::");
                var buffer = Convert.FromBase64String(info[0]);
                var iv = Convert.FromBase64String(info[1]);
                // Decrypt our information
                using var aes = new AesManaged();
                var decryptor = aes.CreateDecryptor(key, iv);
                using var memoryStream = new MemoryStream(buffer);
                using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                using var streamReader = new StreamReader(cryptoStream);
                var decryptedData = streamReader.ReadToEnd();
                if (!decryptedData.IsNullOrEmpty())
                {
                    var jsonInfo = JsonConvert.DeserializeObject<JObject>(decryptedData);
                    expirationDate = jsonInfo["expiration"]?.Value<DateTime>();
                    isValid = jsonInfo["isValid"]?.Value<bool>();
                    stamp = jsonInfo["stamped"]?.Value<int>();
                }
            }
            // Validate our conditions
            if (!expirationDate.HasValue || !isValid.HasValue || !stamp.HasValue)
            {
                throw new InvalidOperationException("Failed to validate subscription.");
            }

            var nowUtc = DateTime.UtcNow;
            var timeSpan = nowUtc - Time.UnixTimeStampToDateTime(stamp.Value);
            if (timeSpan > TimeSpan.FromHours(12))
            {
                throw new InvalidOperationException("Invalid API response.");
            }
            if (!isValid.Value)
            {
                throw new ArgumentException($"Your subscription is not valid, please check your product subscriptions on our website.");
            }
            if (expirationDate < nowUtc)
            {
                throw new ArgumentException($"Your subscription expired {expirationDate}, please renew in order to use this product.");
            }
        }
        catch (Exception e)
        {
            Log.Error($"ValidateSubscription(): Failed during validation, shutting down. Error : {e.Message}");
            Environment.Exit(1);
        }
    }
    #endregion
}
