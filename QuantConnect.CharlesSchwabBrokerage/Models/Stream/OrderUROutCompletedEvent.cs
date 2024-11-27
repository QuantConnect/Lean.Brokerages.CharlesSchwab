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

using System.Collections.Generic;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums.Stream;

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Stream;

/// <summary>
/// Represents an "OrderUROutCompleted" activity, including specific event data.
/// </summary>
/// <param name="SchwabOrderID">The Schwab order ID associated with this activity.</param>
/// <param name="AccountNumber">The account number associated with this activity.</param>
/// <param name="BaseEvent">The event associated with this account activity.</param>
public record OrderUROutCompleted(
    string SchwabOrderID,
    string AccountNumber,
    OrderUROutCompletedBaseEvent BaseEvent) : BaseAccountActivity<OrderUROutCompletedBaseEvent>(SchwabOrderID, AccountNumber, BaseEvent);

/// <summary>
/// Represents the base event information specifically for an "OrderUROutCompleted" event.
/// </summary>
/// <param name="EventType">The type of event, e.g., "OrderUROutCompleted".</param>
/// <param name="OrderUROutCompletedEvent">The details of the "OrderUROutCompleted" event.</param>
public record OrderUROutCompletedBaseEvent(
    string EventType,
    OrderUROutCompletedEvent OrderUROutCompletedEvent);

/// <summary>
/// Represents the details of an "OrderUROutCompleted" event.
/// </summary>
/// <param name="ExecutionTimeStamp">The timestamp of the execution event.</param>
/// <param name="OutCancelType">The type of cancel operation associated with this event.</param>
/// <param name="ValidationDetail">A collection of validation details associated with this event.</param>
public record OrderUROutCompletedEvent(
    ExecutionTimeStamp ExecutionTimeStamp,
    OrderOutCancelType OutCancelType,
    IReadOnlyCollection<ValidationDetail> ValidationDetail);

/// <summary>
/// Represents the validation details related to a specific order, including rule information.
/// </summary>
/// <param name="SchwabOrderID">The Schwab order ID associated with the validation.</param>
/// <param name="NgOMSRuleName">The rule name in the NGOMS (Next Generation Order Management System) that applied to this order.</param>
/// <param name="NgOMSRuleDescription">A description of the NGOMS rule that applied to this order.</param>
public record ValidationDetail(
    string SchwabOrderID,
    string NgOMSRuleName,
    string NgOMSRuleDescription);