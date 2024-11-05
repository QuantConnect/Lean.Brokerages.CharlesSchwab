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
/// Represents a base account activity with general information such as Schwab Order ID and Account Number.
/// </summary>
/// <typeparam name="T">The type of event associated with the account activity.</typeparam>
/// <param name="SchwabOrderID">The Schwab order ID associated with this activity.</param>
/// <param name="AccountNumber">The account number associated with this activity.</param>
/// <param name="BaseEvent">The event associated with this account activity.</param>
public record BaseAccountActivity<T>(
    string SchwabOrderID,
    string AccountNumber,
    T BaseEvent);