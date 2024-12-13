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

namespace QuantConnect.Brokerages.CharlesSchwab.Models;

/// <summary>
/// Represents the details of an option deliverable associated with a financial instrument.
/// </summary>
public class OptionDeliverables
{
    /// <summary>
    /// The underlying ticker symbol of the instrument, representing the trading identifier used on exchanges.
    /// </summary>
    public string Symbol { get; }

    /// <summary>
    /// The number of units deliverable for the option.
    /// </summary>
    public decimal DeliverableUnits { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionDeliverables"/> class with the specified parameters.
    /// </summary>
    /// <param name="symbol">The underlying ticker symbol of the instrument.</param>
    /// <param name="deliverableUnits">The number of units deliverable for the option.</param>
    public OptionDeliverables(string symbol, decimal deliverableUnits) => (Symbol, DeliverableUnits) = (symbol, deliverableUnits);
}
