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
/// Represents an order placed in the Charles Schwab.
/// </summary>
/// <param name="Session">The session during which the order is placed.</param>
/// <param name="Duration">The duration for which the order is valid.</param>
/// <param name="OrderType">The type of the order (e.g., Market, Limit).</param>
/// <param name="CancelTime">The time at which the order is canceled, if applicable.</param>
/// <param name="ComplexOrderStrategyType">The strategy used for complex orders.</param>
/// <param name="Quantity">The total quantity of the order.</param>
/// <param name="FilledQuantity">The quantity of the order that has been filled.</param>
/// <param name="RemainingQuantity">The remaining quantity of the order.</param>
/// <param name="RequestedDestination">The requested destination for order routing.</param>
/// <param name="DestinationLinkName">The destination link name for order execution.</param>
/// <param name="ReleaseTime">The time when the order is released for execution.</param>
/// <param name="StopPrice">The stop price, if applicable.</param>
/// <param name="StopPriceLinkBasis">The basis for linking the stop price.</param>
/// <param name="StopPriceLinkType">The type for linking the stop price.</param>
/// <param name="StopPriceOffset">The offset applied to the stop price.</param>
/// <param name="StopType">The type of stop order.</param>
/// <param name="PriceLinkBasis">The basis for linking the price.</param>
/// <param name="PriceLinkType">The type for linking the price.</param>
/// <param name="Price">The price specified for the order.</param>
/// <param name="TaxLotMethod">The tax lot method used for this order.</param>
/// <param name="OrderLegCollection">A collection of order legs (individual transactions in a multi-leg order).</param>
/// <param name="ActivationPrice">The activation price for the order, if applicable.</param>
/// <param name="SpecialInstruction">Special instructions associated with the order.</param>
/// <param name="OrderStrategyType">The strategy type for the order.</param>
/// <param name="OrderId">The unique identifier for the order.</param>
/// <param name="Cancelable">Indicates whether the order can be canceled.</param>
/// <param name="Editable">Indicates whether the order can be edited.</param>
/// <param name="Status">The current status of the order.</param>
/// <param name="EnteredTime">The time when the order was entered into the system.</param>
/// <param name="CloseTime">The time when the order was closed.</param>
/// <param name="Tag">A custom tag for the order.</param>
/// <param name="AccountNumber">The account number associated with the order.</param>
/// <param name="OrderActivity">A collection of order activities.</param>
/// <param name="ReplacingOrderCollection">A collection of orders that are replacing this order, if applicable.</param>
/// <param name="ChildOrderStrategies">A collection of child order strategies associated with the main order.</param>
/// <param name="StatusDescription">An description related to the order status.</param>
public record OrderResponse(
    [property: JsonProperty("session")] SessionType Session,
    [property: JsonProperty("duration")] Duration Duration,
    [property: JsonProperty("orderType")] OrderType OrderType,
    [property: JsonProperty("cancelTime")] DateTime CancelTime,
    [property: JsonProperty("complexOrderStrategyType")] string ComplexOrderStrategyType,
    [property: JsonProperty("quantity")] decimal Quantity,
    [property: JsonProperty("filledQuantity")] decimal FilledQuantity,
    [property: JsonProperty("remainingQuantity")] decimal RemainingQuantity,
    [property: JsonProperty("requestedDestination")] string RequestedDestination,
    [property: JsonProperty("destinationLinkName")] string DestinationLinkName,
    [property: JsonProperty("releaseTime")] DateTime ReleaseTime,
    [property: JsonProperty("stopPrice")] decimal StopPrice,
    [property: JsonProperty("stopPriceLinkBasis")] string StopPriceLinkBasis,
    [property: JsonProperty("stopPriceLinkType")] StopPriceLinkType StopPriceLinkType,
    [property: JsonProperty("stopPriceOffset")] decimal StopPriceOffset,
    [property: JsonProperty("stopType")] string StopType,
    [property: JsonProperty("priceLinkBasis")] string PriceLinkBasis,
    [property: JsonProperty("priceLinkType")] string PriceLinkType,
    [property: JsonProperty("price")] decimal Price,
    [property: JsonProperty("taxLotMethod")] string TaxLotMethod,
    [property: JsonProperty("orderLegCollection")] IReadOnlyList<OrderLeg> OrderLegCollection,
    [property: JsonProperty("activationPrice")] decimal ActivationPrice,
    [property: JsonProperty("specialInstruction")] string SpecialInstruction,
    [property: JsonProperty("orderStrategyType")] OrderStrategyType OrderStrategyType,
    [property: JsonProperty("orderId")] long OrderId,
    [property: JsonProperty("cancelable")] bool Cancelable,
    [property: JsonProperty("editable")] bool Editable,
    [property: JsonProperty("status")] CharlesSchwabOrderStatus Status,
    [property: JsonProperty("enteredTime")] DateTime EnteredTime,
    [property: JsonProperty("closeTime")] DateTime CloseTime,
    [property: JsonProperty("tag")] string Tag,
    [property: JsonProperty("accountNumber")] int AccountNumber,
    [property: JsonProperty("orderActivityCollection")] IReadOnlyCollection<OrderActivity> OrderActivity,
    [property: JsonProperty("replacingOrderCollection")] string[] ReplacingOrderCollection,
    [property: JsonProperty("childOrderStrategies")] string[] ChildOrderStrategies,
    [property: JsonProperty("statusDescription")] string StatusDescription
);

/// <summary>
/// Represents an activity associated with an order, such as an execution or a modification.
/// </summary>
/// <param name="ActivityType">The type of activity (e.g., Execution, Modification).</param>
/// <param name="ExecutionType">The type of execution (e.g., Full, Partial).</param>
/// <param name="Quantity">The quantity involved in the activity.</param>
/// <param name="OrderRemainingQuantity">The remaining quantity after the activity.</param>
/// <param name="ExecutionLegs">A collection of execution legs for the activity.</param>
public record OrderActivity(
    [property: JsonProperty("activityType")] string ActivityType,
    [property: JsonProperty("executionType")] string ExecutionType,
    [property: JsonProperty("quantity")] decimal Quantity,
    [property: JsonProperty("orderRemainingQuantity")] decimal OrderRemainingQuantity,
    [property: JsonProperty("executionLegs")] IReadOnlyCollection<ExecutionLeg> ExecutionLegs
    );

/// <summary>
/// Represents a leg of an execution, which is a part of a larger trade execution.
/// </summary>
/// <param name="LegId">The identifier of the execution leg.</param>
/// <param name="Price">The price at which the leg was executed.</param>
/// <param name="Quantity">The quantity of the leg executed.</param>
/// <param name="MismarkedQuantity">The mismarked quantity, if applicable.</param>
/// <param name="InstrumentId">The identifier of the financial instrument for this leg.</param>
/// <param name="Time">The time when the leg was executed.</param>
public record ExecutionLeg(
    [property: JsonProperty("legId")] int LegId,
    [property: JsonProperty("price")] decimal Price,
    [property: JsonProperty("quantity")] decimal Quantity,
    [property: JsonProperty("mismarkedQuantity")] decimal MismarkedQuantity,
    [property: JsonProperty("instrumentId")] int InstrumentId,
    [property: JsonProperty("time")] DateTime Time
    );

/// <summary>
/// Represents a leg of an order, typically used in multi-leg orders such as options trades.
/// </summary>
/// <param name="OrderLegType">The type of asset involved in this leg.</param>
/// <param name="LegId">The identifier for this order leg.</param>
/// <param name="Instrument">The financial instrument associated with this leg.</param>
/// <param name="Instruction">The trading instruction (e.g., Buy, Sell).</param>
/// <param name="PositionEffect">The position effect (e.g., Open, Close).</param>
/// <param name="Quantity">The quantity for this leg.</param>
/// <param name="QuantityType">The type of quantity used (e.g., Shares, Contracts).</param>
/// <param name="DivCapGains">Dividend or capital gains specification.</param>
/// <param name="ToSymbol">The symbol for the asset being exchanged to, if applicable.</param>
public record OrderLeg(
    [property: JsonProperty("orderLegType")] AssetType OrderLegType,
    [property: JsonProperty("legId")] int LegId,
    [property: JsonProperty("instrument")] Instrument Instrument,
    [property: JsonProperty("instruction")] Instruction Instruction,
    [property: JsonProperty("positionEffect")] string PositionEffect,
    [property: JsonProperty("quantity")] decimal Quantity,
    [property: JsonProperty("quantityType")] string QuantityType,
    [property: JsonProperty("divCapGains")] string DivCapGains,
    [property: JsonProperty("toSymbol")] string ToSymbol
    );
