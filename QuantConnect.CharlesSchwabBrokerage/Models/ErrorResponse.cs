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
using System.Collections.Generic;

namespace QuantConnect.Brokerages.CharlesSchwab.Models;

/// <summary>
/// Represents an error response from the Charles Schwab API.
/// </summary>
/// <param name="Error">The error message returned by the API.</param>
/// <param name="ErrorDescription">A collection of detailed error descriptions returned by the API.</param>
public record ErrorResponse(
    [JsonProperty("message")] string Error,
    [JsonProperty("errors")] IReadOnlyCollection<string> ErrorDescription
    );

/// <summary>
/// Represents a response containing a collection of errors with metadata from the Charles Schwab API.
/// </summary>
/// <param name="Errors">A collection of <see cref="ErrorMetaData"/> objects that provide details about each error.</param>
public record ErrorsResponse(
    [JsonProperty("errors")] IReadOnlyCollection<ErrorMetaData> Errors
    );

/// <summary>
/// Represents detailed metadata about an individual error returned by the Charles Schwab API.
/// </summary>
/// <param name="Id">The unique identifier for the error.</param>
/// <param name="Status">The HTTP status code associated with the error.</param>
/// <param name="Title">A short, human-readable summary of the error.</param>
/// <param name="Detail">A detailed description of the error.</param>
/// <param name="Source">The source of the error, providing additional context.</param>
public record ErrorMetaData(
    [JsonProperty("id")] string Id,
    [JsonProperty("status")] string Status,
    [JsonProperty("title")] string Title,
    [JsonProperty("detail")] string Detail,
    [JsonProperty("source")] Source Source
    );

/// <summary>
/// Represents the source of an error in an API response, indicating where the error originated.
/// </summary>
/// <param name="Parameter">The parameter that caused the error.</param>
public record struct Source([JsonProperty("parameter")] string Parameter);
