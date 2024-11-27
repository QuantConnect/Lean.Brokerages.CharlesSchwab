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
/// Represents user preferences containing account details, streamer information, and offers.
/// </summary>
/// <param name="Accounts">The collection of accounts associated with the user.</param>
/// <param name="StreamerInfo">The collection of streamer information associated with the user.</param>
/// <param name="Offers">The collection of offers available to the user.</param>
public record UserPreference(
    [property: JsonProperty("accounts")] IReadOnlyCollection<Account> Accounts,
    [property: JsonProperty("streamerInfo")] IReadOnlyCollection<StreamerInfo> StreamerInfo,
    [property: JsonProperty("offers")] IReadOnlyCollection<Offer> Offers);

/// <summary>
/// Represents an offer with permissions for market data.
/// </summary>
/// <param name="Level2Permissions">A value indicating whether Level 2 permissions are granted.</param>
/// <param name="MktDataPermission">The market data permission level.</param>
public record Offer(
    [property: JsonProperty("level2Permissions")] bool Level2Permissions,
    [property: JsonProperty("mktDataPermission")] string MktDataPermission);

/// <summary>
/// Represents streamer information for a user.
/// </summary>
/// <param name="StreamerSocketUrl">The URL for the streamer socket.</param>
/// <param name="SchwabClientCustomerId">The Schwab client customer ID.</param>
/// <param name="SchwabClientCorrelId">The Schwab client correlation ID.</param>
/// <param name="SchwabClientChannel">The Schwab client channel.</param>
/// <param name="SchwabClientFunctionId">The Schwab client function ID.</param>
public record StreamerInfo(
    [property: JsonProperty("streamerSocketUrl")] string StreamerSocketUrl,
    [property: JsonProperty("schwabClientCustomerId")] string SchwabClientCustomerId,
    [property: JsonProperty("schwabClientCorrelId")] string SchwabClientCorrelId,
    [property: JsonProperty("schwabClientChannel")] string SchwabClientChannel,
    [property: JsonProperty("schwabClientFunctionId")] string SchwabClientFunctionId);

/// <summary>
/// Represents an account associated with the user.
/// </summary>
/// <param name="AccountNumber">The account number.</param>
/// <param name="PrimaryAccount">A value indicating whether this is the primary account.</param>
/// <param name="Type">The type of the account.</param>
/// <param name="NickName">The nickname of the account.</param>
/// <param name="DisplayAcctId">The display account ID.</param>
/// <param name="AutoPositionEffect">The auto position effect setting for the account.</param>
/// <param name="AccountColor">The color associated with the account.</param>
public record Account(
    [property: JsonProperty("accountNumber")] string AccountNumber,
    [property: JsonProperty("primaryAccount")] bool PrimaryAccount,
    [property: JsonProperty("type")] string Type,
    [property: JsonProperty("nickName")] string NickName,
    [property: JsonProperty("displayAcctId")] string DisplayAcctId,
    [property: JsonProperty("autoPositionEffect")] string AutoPositionEffect,
    [property: JsonProperty("accountColor")] string AccountColor);
