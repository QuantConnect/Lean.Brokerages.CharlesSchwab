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
public class ErrorResponse
{
    /// <summary>
    /// The error message returned by the API.
    /// </summary>
    [JsonProperty("message")]
    public string Error { get; }

    /// <summary>
    /// A collection of detailed error descriptions returned by the API.
    /// </summary>
    [JsonProperty("errors")]
    public IReadOnlyCollection<string> ErrorDescription { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorResponse"/> class with specified error details.
    /// </summary>
    /// <param name="error">The error message returned by the API.</param>
    /// <param name="errorDescription">A collection of detailed error descriptions returned by the API.</param>
    [JsonConstructor]
    public ErrorResponse(string error, IReadOnlyCollection<string> errorDescription) => (Error, ErrorDescription) = (error, errorDescription);
}

/// <summary>
/// Represents a response containing a collection of errors with metadata from the Charles Schwab API.
/// </summary>
public class ErrorsResponse
{
    /// <summary>
    /// A collection of <see cref="ErrorMetaData"/> objects that provide details about each error.
    /// </summary>
    [JsonProperty("errors")]
    public IReadOnlyCollection<ErrorMetaData> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorsResponse"/> class with the specified error metadata.
    /// </summary>
    /// <param name="errors">A collection of <see cref="ErrorMetaData"/> objects that contain information about the errors.</param>
    [JsonConstructor]
    public ErrorsResponse(IReadOnlyCollection<ErrorMetaData> errors) => Errors = errors;
}

/// <summary>
/// Represents detailed metadata about an individual error returned by the Charles Schwab API.
/// </summary>
public class ErrorMetaData
{
    /// <summary>
    /// The unique identifier for the error.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; }

    /// <summary>
    /// The HTTP status code associated with the error.
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; }

    /// <summary>
    /// A short, human-readable summary of the error.
    /// </summary>
    [JsonProperty("title")]
    public string Title { get; }

    /// <summary>
    /// A detailed description of the error.
    /// </summary>
    [JsonProperty("detail")]
    public string Detail { get; }

    /// <summary>
    /// The source of the error, providing additional context.
    /// </summary>
    [JsonProperty("source")]
    public Source Source { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorMetaData"/> class with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier for the error.</param>
    /// <param name="status">The HTTP status code associated with the error.</param>
    /// <param name="title">A short, human-readable summary of the error.</param>
    /// <param name="detail">A detailed description of the error.</param>
    /// <param name="source">The source of the error, providing additional context.</param>
    [JsonConstructor]
    public ErrorMetaData(string id, string status, string title, string detail, Source source)
        => (Id, Status, Title, Detail, Source) = (id, status, title, detail, source);
}

/// <summary>
/// Represents the source of an error in an API response, indicating where the error originated.
/// </summary>
public readonly struct Source
{
    /// <summary>
    /// The parameter that caused the error.
    /// </summary>
    [JsonProperty("parameter")]
    public string Parameter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Source"/> struct with the specified parameter.
    /// </summary>
    /// <param name="parameter">The parameter that caused the error.</param>
    [JsonConstructor]
    public Source(string parameter) => Parameter = parameter;
}
