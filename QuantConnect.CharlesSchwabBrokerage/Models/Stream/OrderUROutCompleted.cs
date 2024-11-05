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
using QuantConnect.Util;
using System.Collections.Generic;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums.Stream;

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Stream;

public record BaseAccountActivity<T>(
    string SchwabOrderID,
    string AccountNumber,
    T BaseEvent);

public record OrderUROutCompleted(
    string SchwabOrderID,
    string AccountNumber,
    BaseEvent BaseEvent) : BaseAccountActivity<BaseEvent>(SchwabOrderID, AccountNumber, BaseEvent);


public record BaseEvent(
    string EventType,
    OrderUROutCompletedEvent OrderUROutCompletedEvent);

public record OrderUROutCompletedEvent(
    ExecutionTimeStamp ExecutionTimeStamp,
    OrderOutCancelType OutCancelType,
    IReadOnlyCollection<ValidationDetail> ValidationDetail);

public record ValidationDetail(
    string SchwabOrderID,
    string NgOMSRuleName,
    string NgOMSRuleDescription);

public record ExecutionTimeStamp(
    [JsonProperty("DateTimeString"), JsonConverter(typeof(DateTimeJsonConverter), "yyyy-MM-dd HH:mm:ss.fff")] DateTime DateTime);

