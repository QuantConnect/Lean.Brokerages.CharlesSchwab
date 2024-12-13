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

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Stream;

/// <summary>
/// Represents the base event for an "OrderFillCompletedEvent" activity, containing the event type and quantity information.
/// </summary>
public class OrderFillCompletedBaseEvent
{
    /// <summary>
    /// Gets the type of event, typically "OrderFillCompletedEvent".
    /// </summary>
    public string EventType { get; }

    /// <summary>
    /// Gets information about the quantity for each leg of the order fill event.
    /// </summary>
    public OrderFillCompletedEventOrderLegQuantityInfo OrderFillCompletedEventOrderLegQuantityInfo { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderFillCompletedBaseEvent"/> class.
    /// </summary>
    /// <param name="eventType">The type of event, typically "OrderFillCompletedEvent".</param>
    /// <param name="orderFillCompletedEventOrderLegQuantityInfo">Information about the quantity for each leg of the order fill event.</param>
    public OrderFillCompletedBaseEvent(string eventType, OrderFillCompletedEventOrderLegQuantityInfo orderFillCompletedEventOrderLegQuantityInfo)
        => (EventType, OrderFillCompletedEventOrderLegQuantityInfo) = (eventType, orderFillCompletedEventOrderLegQuantityInfo);

}

/// <summary>
/// Represents an "OrderFillCompletedEvent" activity, including event details specific to order fills.
/// </summary>
public class OrderFillCompletedEvent : BaseAccountActivity<OrderFillCompletedBaseEvent>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderFillCompletedEvent"/> class.
    /// </summary>
    /// <param name="schwabOrderID">The Schwab order ID associated with this fill completion activity.</param>
    /// <param name="accountNumber">The account number associated with this fill completion activity.</param>
    /// <param name="baseEvent">The event details specific to the order fill completion.</param>
    public OrderFillCompletedEvent(string schwabOrderID, string accountNumber, OrderFillCompletedBaseEvent baseEvent) : base(schwabOrderID, accountNumber, baseEvent)
    { }
}

/// <summary>
/// Represents information about the quantity for each leg of an "OrderFillCompletedEvent" event.
/// </summary>
public readonly struct OrderFillCompletedEventOrderLegQuantityInfo
{
    /// <summary>
    /// The identifier of the execution leg.
    /// This ID is used to uniquely identify the execution leg in the context of an order fill event.
    /// </summary>
    public string LegId { get; }

    /// <summary>
    /// Gets the execution information, including the timestamp for the order fill.
    /// </summary>
    public ExecutionInfo ExecutionInfo { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderFillCompletedEventOrderLegQuantityInfo"/> struct.
    /// This constructor sets the identifier for the execution leg and its associated execution information.
    /// </summary>
    /// <param name="legId">The identifier of the execution leg.</param>
    /// <param name="executionInfo">The execution information, including the timestamp for the order fill.</param>

    public OrderFillCompletedEventOrderLegQuantityInfo(string legId, ExecutionInfo executionInfo) => (LegId, ExecutionInfo) = (legId, executionInfo);
}

/// <summary>
/// Represents the timestamp of an execution event.
/// </summary>
public readonly struct ExecutionInfo
{
    /// <summary>
    /// Gets the timestamp of the execution event.
    /// </summary>
    public ExecutionTimeStamp ExecutionTimeStamp { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionInfo"/> struct.
    /// </summary>
    /// <param name="executionTimeStamp">The date and time of the execution in a specified format.</param>
    public ExecutionInfo(ExecutionTimeStamp executionTimeStamp) => ExecutionTimeStamp = executionTimeStamp;
}

