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

using Newtonsoft.Json;

namespace QuantConnect.Brokerages.CharlesSchwab.Models;

/// <summary>
/// Represents the response containing the account number and its associated hash value.
/// </summary>
public class AccountNumberResponse
{
    /// <summary>
    /// The account number.
    /// </summary>
    public string AccountNumber { get; }

    /// <summary>
    /// The hash value associated with the account number.
    /// </summary>
    public string HashValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountNumberResponse"/> struct.
    /// </summary>
    /// <param name="accountNumber">The account number.</param>
    /// <param name="hashValue">The hash value associated with the account number.</param>
    [JsonConstructor]
    public AccountNumberResponse(string accountNumber, string hashValue) => (AccountNumber, HashValue) = (accountNumber, hashValue);
}
