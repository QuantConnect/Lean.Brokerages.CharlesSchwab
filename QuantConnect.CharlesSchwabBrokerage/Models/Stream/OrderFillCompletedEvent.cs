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
/// <param name="EventType">The type of event, typically "OrderFillCompletedEvent".</param>
/// <param name="OrderFillCompletedEventOrderLegQuantityInfo">Information about the quantity for each leg of the order fill event.</param>
public record OrderFillCompletedBaseEvent(
    string EventType,
    OrderFillCompletedEventOrderLegQuantityInfo OrderFillCompletedEventOrderLegQuantityInfo);

/// <summary>
/// Represents an "OrderFillCompletedEvent" activity, including event details specific to order fills.
/// </summary>
/// <param name="SchwabOrderID">The Schwab order ID associated with this fill completion activity.</param>
/// <param name="AccountNumber">The account number associated with this fill completion activity.</param>
/// <param name="BaseEvent">The event details specific to the order fill completion.</param>
public record OrderFillCompletedEvent(
    string SchwabOrderID,
    string AccountNumber,
    OrderFillCompletedBaseEvent BaseEvent) : BaseAccountActivity<OrderFillCompletedBaseEvent>(SchwabOrderID, AccountNumber, BaseEvent);

/// <summary>
/// Represents information about the quantity for each leg of an "OrderFillCompletedEvent" event.
/// </summary>
/// <param name="ExecutionInfo">Execution information, including the timestamp for the order fill.</param>
public record OrderFillCompletedEventOrderLegQuantityInfo(
    ExecutionInfo ExecutionInfo);

/// <summary>
/// Represents the timestamp of an execution event.
/// </summary>
/// <param name="ExecutionTimeStamp">The date and time of the execution in a specified format.</param>
public record ExecutionInfo(
    ExecutionTimeStamp ExecutionTimeStamp);

