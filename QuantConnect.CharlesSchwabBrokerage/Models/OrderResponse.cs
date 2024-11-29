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
using Newtonsoft.Json;
using System.Collections.Generic;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums;

namespace QuantConnect.Brokerages.CharlesSchwab.Models;

/// <summary>
/// Represents an order placed in the Charles Schwab system.
/// </summary>
public class OrderResponse
{
    /// <summary>
    /// The session during which the order is placed.
    /// </summary>
    public SessionType Session { get; }

    /// <summary>
    /// The duration for which the order is valid.
    /// </summary>
    public Duration Duration { get; }

    /// <summary>
    /// The type of the order (e.g., Market, Limit).
    /// </summary>
    public OrderType OrderType { get; }

    /// <summary>
    /// The time at which the order is canceled, if applicable.
    /// </summary>
    public DateTime CancelTime { get; }

    /// <summary>
    /// The strategy used for complex orders.
    /// </summary>
    public string ComplexOrderStrategyType { get; }

    /// <summary>
    /// The total quantity of the order.
    /// </summary>
    public decimal Quantity { get; }

    /// <summary>
    /// The quantity of the order that has been filled.
    /// </summary>
    public decimal FilledQuantity { get; }

    /// <summary>
    /// The remaining quantity of the order.
    /// </summary>
    public decimal RemainingQuantity { get; }

    /// <summary>
    /// The requested destination for order routing.
    /// </summary>
    public string RequestedDestination { get; }

    /// <summary>
    /// The destination link name for order execution.
    /// </summary>
    public string DestinationLinkName { get; }

    /// <summary>
    /// The time when the order is released for execution.
    /// </summary>
    public DateTime ReleaseTime { get; }

    /// <summary>
    /// The stop price, if applicable.
    /// </summary>
    public decimal StopPrice { get; }

    /// <summary>
    /// The basis for linking the stop price.
    /// </summary>
    public string StopPriceLinkBasis { get; }

    /// <summary>
    /// The type for linking the stop price.
    /// </summary>
    public StopPriceLinkType StopPriceLinkType { get; }

    /// <summary>
    /// The offset applied to the stop price.
    /// </summary>
    public decimal StopPriceOffset { get; }

    /// <summary>
    /// The type of stop order.
    /// </summary>
    public string StopType { get; }

    /// <summary>
    /// The basis for linking the price.
    /// </summary>
    public string PriceLinkBasis { get; }

    /// <summary>
    /// The type for linking the price.
    /// </summary>
    public string PriceLinkType { get; }

    /// <summary>
    /// The price specified for the order.
    /// </summary>
    public decimal Price { get; }

    /// <summary>
    /// The tax lot method used for this order.
    /// </summary>
    public string TaxLotMethod { get; }

    /// <summary>
    /// A collection of order legs (individual transactions in a multi-leg order).
    /// </summary>
    public IReadOnlyList<OrderLeg> OrderLegCollection { get; }

    /// <summary>
    /// The activation price for the order, if applicable.
    /// </summary>
    public decimal ActivationPrice { get; }

    /// <summary>
    /// Special instructions associated with the order.
    /// </summary>
    public string SpecialInstruction { get; }

    /// <summary>
    /// The strategy type for the order.
    /// </summary>
    public OrderStrategyType OrderStrategyType { get; }

    /// <summary>
    /// The unique identifier for the order.
    /// </summary>
    public long OrderId { get; }

    /// <summary>
    /// Indicates whether the order can be canceled.
    /// </summary>
    public bool Cancelable { get; }

    /// <summary>
    /// Indicates whether the order can be edited.
    /// </summary>
    public bool Editable { get; }

    /// <summary>
    /// The current status of the order.
    /// </summary>
    public OrderStatus Status { get; }

    /// <summary>
    /// The time when the order was entered into the system.
    /// </summary>
    public DateTime EnteredTime { get; }

    /// <summary>
    /// The time when the order was closed.
    /// </summary>
    public DateTime CloseTime { get; }

    /// <summary>
    /// A custom tag for the order.
    /// </summary>
    public string Tag { get; }

    /// <summary>
    /// The account number associated with the order.
    /// </summary>
    public int AccountNumber { get; }

    /// <summary>
    /// A collection of order activities.
    /// </summary>
    [property: JsonProperty("orderActivityCollection")]
    public IReadOnlyCollection<OrderActivity> OrderActivity { get; }

    /// <summary>
    /// A collection of orders that are replacing this order, if applicable.
    /// </summary>
    public string[] ReplacingOrderCollection { get; }

    /// <summary>
    /// A collection of child order strategies associated with the main order.
    /// </summary>
    public string[] ChildOrderStrategies { get; }

    /// <summary>
    /// A description related to the order status.
    /// </summary>
    public string StatusDescription { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderResponse"/> class with the specified parameters.
    /// </summary>
    /// <param name="session">The session during which the order is placed.</param>
    /// <param name="duration">The duration for which the order is valid.</param>
    /// <param name="orderType">The type of the order (e.g., Market, Limit).</param>
    /// <param name="cancelTime">The time at which the order is canceled, if applicable.</param>
    /// <param name="complexOrderStrategyType">The strategy used for complex orders.</param>
    /// <param name="quantity">The total quantity of the order.</param>
    /// <param name="filledQuantity">The quantity of the order that has been filled.</param>
    /// <param name="remainingQuantity">The remaining quantity of the order.</param>
    /// <param name="requestedDestination">The requested destination for order routing.</param>
    /// <param name="destinationLinkName">The destination link name for order execution.</param>
    /// <param name="releaseTime">The time when the order is released for execution.</param>
    /// <param name="stopPrice">The stop price, if applicable.</param>
    /// <param name="stopPriceLinkBasis">The basis for linking the stop price.</param>
    /// <param name="stopPriceLinkType">The type for linking the stop price.</param>
    /// <param name="stopPriceOffset">The offset applied to the stop price.</param>
    /// <param name="stopType">The type of stop order.</param>
    /// <param name="priceLinkBasis">The basis for linking the price.</param>
    /// <param name="priceLinkType">The type for linking the price.</param>
    /// <param name="price">The price specified for the order.</param>
    /// <param name="taxLotMethod">The tax lot method used for this order.</param>
    /// <param name="orderLegCollection">A collection of order legs (individual transactions in a multi-leg order).</param>
    /// <param name="activationPrice">The activation price for the order, if applicable.</param>
    /// <param name="specialInstruction">Special instructions associated with the order.</param>
    /// <param name="orderStrategyType">The strategy type for the order.</param>
    /// <param name="orderId">The unique identifier for the order.</param>
    /// <param name="cancelable">Indicates whether the order can be canceled.</param>
    /// <param name="editable">Indicates whether the order can be edited.</param>
    /// <param name="status">The current status of the order.</param>
    /// <param name="enteredTime">The time when the order was entered into the system.</param>
    /// <param name="closeTime">The time when the order was closed.</param>
    /// <param name="tag">A custom tag for the order.</param>
    /// <param name="accountNumber">The account number associated with the order.</param>
    /// <param name="orderActivity">A collection of order activities.</param>
    /// <param name="replacingOrderCollection">A collection of orders that are replacing this order, if applicable.</param>
    /// <param name="childOrderStrategies">A collection of child order strategies associated with the main order.</param>
    /// <param name="statusDescription">A description related to the order status.</param>
    [JsonConstructor]
    public OrderResponse(SessionType session, Duration duration, OrderType orderType, DateTime cancelTime, string complexOrderStrategyType, decimal quantity, decimal filledQuantity, decimal remainingQuantity, string requestedDestination, string destinationLinkName, DateTime releaseTime, decimal stopPrice, string stopPriceLinkBasis, StopPriceLinkType stopPriceLinkType, decimal stopPriceOffset, string stopType, string priceLinkBasis, string priceLinkType, decimal price, string taxLotMethod, IReadOnlyList<OrderLeg> orderLegCollection, decimal activationPrice, string specialInstruction, OrderStrategyType orderStrategyType, long orderId, bool cancelable, bool editable, OrderStatus status, DateTime enteredTime, DateTime closeTime, string tag, int accountNumber, IReadOnlyCollection<OrderActivity> orderActivity, string[] replacingOrderCollection, string[] childOrderStrategies, string statusDescription)
    {
        Session = session;
        Duration = duration;
        OrderType = orderType;
        CancelTime = cancelTime;
        ComplexOrderStrategyType = complexOrderStrategyType;
        Quantity = quantity;
        FilledQuantity = filledQuantity;
        RemainingQuantity = remainingQuantity;
        RequestedDestination = requestedDestination;
        DestinationLinkName = destinationLinkName;
        ReleaseTime = releaseTime;
        StopPrice = stopPrice;
        StopPriceLinkBasis = stopPriceLinkBasis;
        StopPriceLinkType = stopPriceLinkType;
        StopPriceOffset = stopPriceOffset;
        StopType = stopType;
        PriceLinkBasis = priceLinkBasis;
        PriceLinkType = priceLinkType;
        Price = price;
        TaxLotMethod = taxLotMethod;
        OrderLegCollection = orderLegCollection;
        ActivationPrice = activationPrice;
        SpecialInstruction = specialInstruction;
        OrderStrategyType = orderStrategyType;
        OrderId = orderId;
        Cancelable = cancelable;
        Editable = editable;
        Status = status;
        EnteredTime = enteredTime;
        CloseTime = closeTime;
        Tag = tag;
        AccountNumber = accountNumber;
        OrderActivity = orderActivity;
        ReplacingOrderCollection = replacingOrderCollection;
        ChildOrderStrategies = childOrderStrategies;
        StatusDescription = statusDescription;
    }
}

/// <summary>
/// Represents an activity associated with an order, such as an execution or a modification.
/// </summary>
public class OrderActivity
{
    /// <summary>
    /// The type of activity (e.g., Execution, Modification).
    /// </summary>
    public string ActivityType { get; }

    /// <summary>
    /// The type of execution (e.g., Full, Partial).
    /// </summary>
    public string ExecutionType { get; }

    /// <summary>
    /// The quantity involved in the activity.
    /// </summary>
    public decimal Quantity { get; }

    /// <summary>
    /// The remaining quantity after the activity.
    /// </summary>
    public decimal OrderRemainingQuantity { get; }

    /// <summary>
    /// A collection of execution legs for the activity.
    /// </summary>
    public IReadOnlyCollection<ExecutionLeg> ExecutionLegs { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderActivity"/> class with the specified parameters.
    /// </summary>
    /// <param name="activityType">The type of activity (e.g., Execution, Modification).</param>
    /// <param name="executionType">The type of execution (e.g., Full, Partial).</param>
    /// <param name="quantity">The quantity involved in the activity.</param>
    /// <param name="orderRemainingQuantity">The remaining quantity after the activity.</param>
    /// <param name="executionLegs">A collection of execution legs for the activity.</param>
    [JsonConstructor]
    public OrderActivity(string activityType, string executionType, decimal quantity, decimal orderRemainingQuantity, IReadOnlyCollection<ExecutionLeg> executionLegs)
    {
        ActivityType = activityType;
        ExecutionType = executionType;
        Quantity = quantity;
        OrderRemainingQuantity = orderRemainingQuantity;
        ExecutionLegs = executionLegs;
    }
}

/// <summary>
/// Represents a leg of an execution, which is a part of a larger trade execution.
/// </summary>
public class ExecutionLeg
{
    /// <summary>
    /// The identifier of the execution leg.
    /// </summary>
    public int LegId { get; }

    /// <summary>
    /// The price at which the leg was executed.
    /// </summary>
    public decimal Price { get; }

    /// <summary>
    /// The quantity of the leg executed.
    /// </summary>
    public decimal Quantity { get; }

    /// <summary>
    /// The mismarked quantity, if applicable.
    /// </summary>
    public decimal MismarkedQuantity { get; }

    /// <summary>
    /// The identifier of the financial instrument for this leg.
    /// </summary>
    public int InstrumentId { get; }

    /// <summary>
    /// The time when the leg was executed.
    /// </summary>
    public DateTime Time { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionLeg"/> class with the specified parameters.
    /// </summary>
    /// <param name="legId">The identifier of the execution leg.</param>
    /// <param name="price">The price at which the leg was executed.</param>
    /// <param name="quantity">The quantity of the leg executed.</param>
    /// <param name="mismarkedQuantity">The mismarked quantity, if applicable.</param>
    /// <param name="instrumentId">The identifier of the financial instrument for this leg.</param>
    /// <param name="time">The time when the leg was executed.</param>
    [JsonConstructor]
    public ExecutionLeg(int legId, decimal price, decimal quantity, decimal mismarkedQuantity, int instrumentId, DateTime time)
    {
        LegId = legId;
        Price = price;
        Quantity = quantity;
        MismarkedQuantity = mismarkedQuantity;
        InstrumentId = instrumentId;
        Time = time;
    }
}

/// <summary>
/// Represents a leg of an order, typically used in multi-leg orders such as options trades.
/// </summary>
public class OrderLeg
{
    /// <summary>
    /// The type of asset involved in this leg.
    /// </summary>
    public AssetType OrderLegType { get; }

    /// <summary>
    /// The identifier for this order leg.
    /// </summary>
    public int LegId { get; }

    /// <summary>
    /// The financial instrument associated with this leg.
    /// </summary>
    public Instrument Instrument { get; }

    /// <summary>
    /// The trading instruction (e.g., Buy, Sell).
    /// </summary>
    public Instruction Instruction { get; }

    /// <summary>
    /// The position effect (e.g., Open, Close).
    /// </summary>
    public string PositionEffect { get; }

    /// <summary>
    /// The quantity for this leg.
    /// </summary>
    public decimal Quantity { get; }

    /// <summary>
    /// The type of quantity used (e.g., Shares, Contracts).
    /// </summary>
    public string QuantityType { get; }

    /// <summary>
    /// Dividend or capital gains specification.
    /// </summary>
    public string DivCapGains { get; }

    /// <summary>
    /// The symbol for the asset being exchanged to, if applicable.
    /// </summary>
    public string ToSymbol { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderLeg"/> class with the specified parameters.
    /// </summary>
    /// <param name="orderLegType">The type of asset involved in this leg.</param>
    /// <param name="legId">The identifier for this order leg.</param>
    /// <param name="instrument">The financial instrument associated with this leg.</param>
    /// <param name="instruction">The trading instruction (e.g., Buy, Sell).</param>
    /// <param name="positionEffect">The position effect (e.g., Open, Close).</param>
    /// <param name="quantity">The quantity for this leg.</param>
    /// <param name="quantityType">The type of quantity used (e.g., Shares, Contracts).</param>
    /// <param name="divCapGains">Dividend or capital gains specification.</param>
    /// <param name="toSymbol">The symbol for the asset being exchanged to, if applicable.</param>
    [JsonConstructor]
    public OrderLeg(AssetType orderLegType, int legId, Instrument instrument, Instruction instruction, string positionEffect, decimal quantity, string quantityType, string divCapGains, string toSymbol)
    {
        OrderLegType = orderLegType;
        LegId = legId;
        Instrument = instrument;
        Instruction = instruction;
        PositionEffect = positionEffect;
        Quantity = quantity;
        QuantityType = quantityType;
        DivCapGains = divCapGains;
        ToSymbol = toSymbol;
    }
}