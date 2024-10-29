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

using System.Runtime.Serialization;

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Enums;

/// <summary>
/// Represents the types of assets that can be traded or held in a Charles Schwab account.
/// </summary>
public enum AssetType
{
    /// <summary>
    /// An equity asset, representing shares of a stock or similar security.
    /// </summary>
    [EnumMember(Value = "EQUITY")]
    Equity = 0,

    /// <summary>
    /// An option contract, which gives the right to buy or sell an underlying asset at a specified price.
    /// </summary>
    [EnumMember(Value = "OPTION")]
    Option = 1,

    /// <summary>
    /// An index, representing a statistical measure of the performance of a group of securities.
    /// </summary>
    [EnumMember(Value = "INDEX")]
    Index = 2,

    /// <summary>
    /// A mutual fund, a collective investment that pools money from many investors to purchase securities.
    /// </summary>
    [EnumMember(Value = "MUTUAL_FUND")]
    MutualFund = 3,

    /// <summary>
    /// A cash equivalent, representing highly liquid and low-risk assets like money market instruments.
    /// </summary>
    [EnumMember(Value = "CASH_EQUIVALENT")]
    CashEquivalent = 4,

    /// <summary>
    /// A fixed income security, representing debt instruments like bonds that pay fixed interest.
    /// </summary>
    [EnumMember(Value = "FIXED_INCOME")]
    FixedIncome = 5,

    /// <summary>
    /// A currency asset, which refers to foreign exchange or other currency holdings.
    /// </summary>
    [EnumMember(Value = "CURRENCY")]
    Currency = 6,

    /// <summary>
    /// A collective investment, representing pooled investment funds such as hedge funds.
    /// </summary> 
    [EnumMember(Value = "COLLECTIVE_INVESTMENT")]
    CollectiveInvestment = 7,
}
