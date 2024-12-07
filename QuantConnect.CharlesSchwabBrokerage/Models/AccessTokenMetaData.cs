﻿/*
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

namespace QuantConnect.Brokerages.CharlesSchwab.Models;

/// <summary>
/// Represents an access token used for authenticating and authorizing requests.
/// </summary>
public class AccessTokenMetaData
{
    /// <summary>
    /// The number of seconds the access token is valid for.
    /// </summary>
    [JsonProperty("expires_in", Required = Required.Always)]
    public int ExpiresInSec { get; }

    /// <summary>
    /// The expiration date and time of the access token.
    /// </summary>
    public DateTime AccessTokenExpires { get; }

    /// <summary>
    /// The type of access token (e.g., Bearer).
    /// </summary>
    [JsonProperty("token_type", Required = Required.Always)]
    public string TokenType { get; }

    /// <summary>
    /// The scope of access granted by this token.
    /// </summary>
    [JsonProperty("scope", Required = Required.Always)]
    public string Scope { get; }

    /// <summary>
    /// The refresh token used to obtain a new access token (valid for 7 days).
    /// </summary>
    [JsonProperty("refresh_token", Required = Required.Always)]
    public string RefreshToken { get; }

    /// <summary>
    /// The access token used to access a user's protected resources (valid for 30 minutes).
    /// </summary>
    [JsonProperty("access_token", Required = Required.Always)]
    public string AccessToken { get; }

    /// <summary>
    /// The JSON Web Token (JWT) containing user information and claims.
    /// </summary>
    [JsonProperty("id_token", Required = Required.Always)]
    public string IDToken { get; }

    /// <summary>
    /// Initialize new instance of <see cref="Models.AccessTokenMetaData"/>
    /// </summary>
    /// <param name="expiresInSec">The number of seconds the access token is valid for.</param>
    /// <param name="tokenType">The type of access token (e.g., Bearer).</param>
    /// <param name="scope">The scope of access granted by this token.</param>
    /// <param name="refreshToken">The refresh token used to obtain a new access token (valid for 7 days).</param>
    /// <param name="accessToken">The access token used to access a user's protected resources (valid for 30 minutes).</param>
    /// <param name="idToken">The JSON Web Token (JWT) containing user information and claims.</param>
    [JsonConstructor]
    public AccessTokenMetaData(int expiresInSec, string tokenType, string scope, string refreshToken, string accessToken, string idToken)
    {
        ExpiresInSec = expiresInSec;
        TokenType = tokenType;
        Scope = scope;
        RefreshToken = refreshToken;
        AccessToken = accessToken;
        IDToken = idToken;
        AccessTokenExpires = DateTime.UtcNow.AddSeconds(expiresInSec - 10); // A 10-second buffer for expiration
    }
}

