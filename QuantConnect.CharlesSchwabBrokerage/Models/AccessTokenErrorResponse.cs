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
/// Represents a token error response from the Charles Schwab API.
/// </summary>
public class AccessTokenErrorResponse
{
    /// <summary>
    /// The error message returned by the API.
    /// </summary>
    public string Error { get; }

    /// <summary>
    /// A detailed error description returned by the API.
    /// </summary>
    [JsonProperty("error_description")]
    public string ErrorDescription { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessTokenErrorResponse"/> struct.
    /// </summary>
    /// <param name="error">The error message returned by the API.</param>
    /// <param name="errorDescription">A detailed error description returned by the API.</param>
    [JsonConstructor]
    public AccessTokenErrorResponse(string error, string errorDescription) => (Error, ErrorDescription) = (error, errorDescription);
}