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
public class OrderUROutCompleted : BaseAccountActivity<OrderUROutCompletedBaseEvent>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderUROutCompleted"/> class.
    /// </summary>
    /// <param name="schwabOrderID">The Schwab order ID associated with this activity.</param>
    /// <param name="accountNumber">The account number associated with this activity.</param>
    /// <param name="baseEvent">The event associated with this account activity.</param>
    public OrderUROutCompleted(string schwabOrderID, string accountNumber, OrderUROutCompletedBaseEvent baseEvent) : base(schwabOrderID, accountNumber, baseEvent)
    {
    }
}

/// <summary>
/// Represents the base event information specifically for an "OrderUROutCompleted" event.
/// </summary>
public class OrderUROutCompletedBaseEvent
{
    /// <summary>
    /// The type of event, e.g., "OrderUROutCompleted".
    /// </summary>
    public string EventType { get; }

    /// <summary>
    /// The details of the "OrderUROutCompleted" event.
    /// </summary>
    public OrderUROutCompletedEvent OrderUROutCompletedEvent { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderUROutCompletedBaseEvent"/> class.
    /// </summary>
    /// <param name="eventType">The type of event, e.g., "OrderUROutCompleted".</param>
    /// <param name="orderUROutCompletedEvent">The details of the "OrderUROutCompleted" event.</param>
    public OrderUROutCompletedBaseEvent(string eventType, OrderUROutCompletedEvent orderUROutCompletedEvent)
    {
        EventType = eventType;
        OrderUROutCompletedEvent = orderUROutCompletedEvent;
    }
}

/// <summary>
/// Represents the details of an "OrderUROutCompleted" event.
/// </summary>
public class OrderUROutCompletedEvent
{
    /// <summary>
    /// The timestamp of the execution event.
    /// </summary>
    public ExecutionTimeStamp? ExecutionTimeStamp { get; }

    /// <summary>
    /// The type of cancel operation associated with this event.
    /// </summary>
    public OrderOutCancelType OutCancelType { get; }

    /// <summary>
    /// A collection of validation details associated with this event.
    /// </summary>
    public IReadOnlyCollection<ValidationDetail> ValidationDetail { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderUROutCompletedEvent"/> class.
    /// </summary>
    /// <param name="executionTimeStamp">The timestamp of the execution event.</param>
    /// <param name="outCancelType">The type of cancel operation associated with this event.</param>
    /// <param name="validationDetail">A collection of validation details associated with this event.</param>
    public OrderUROutCompletedEvent(ExecutionTimeStamp executionTimeStamp, OrderOutCancelType outCancelType, IReadOnlyCollection<ValidationDetail> validationDetail)
    {
        ExecutionTimeStamp = executionTimeStamp;
        OutCancelType = outCancelType;
        ValidationDetail = validationDetail;
    }
}

/// <summary>
/// Represents the validation details related to a specific order, including rule information.
/// </summary>
public readonly struct ValidationDetail
{
    /// <summary>
    /// The Schwab order ID associated with the validation.
    /// </summary>
    public string SchwabOrderID { get; }

    /// <summary>
    /// The rule name in the NGOMS (Next Generation Order Management System) that applied to this order.
    /// </summary>
    public string NgOMSRuleName { get; }

    /// <summary>
    /// A description of the NGOMS rule that applied to this order.
    /// </summary>
    public string NgOMSRuleDescription { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationDetail"/> struct.
    /// </summary>
    /// <param name="schwabOrderID">The Schwab order ID associated with the validation.</param>
    /// <param name="ngOMSRuleName">The rule name in NGOMS that applied to this order.</param>
    /// <param name="ngOMSRuleDescription">A description of the NGOMS rule that applied to this order.</param>
    public ValidationDetail(string schwabOrderID, string ngOMSRuleName, string ngOMSRuleDescription)
    {
        SchwabOrderID = schwabOrderID;
        NgOMSRuleName = ngOMSRuleName;
        NgOMSRuleDescription = ngOMSRuleDescription;
    }
}