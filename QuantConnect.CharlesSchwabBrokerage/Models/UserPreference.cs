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
public class UserPreference
{
    /// <summary>
    /// The collection of accounts associated with the user.
    /// </summary>
    public IReadOnlyCollection<Account> Accounts { get; }

    /// <summary>
    /// The collection of streamer information associated with the user.
    /// </summary>
    public IReadOnlyCollection<StreamerInfo> StreamerInfo { get; }

    /// <summary>
    /// The collection of offers available to the user.
    /// </summary>
    public IReadOnlyCollection<Offer> Offers { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserPreference"/> class.
    /// </summary>
    /// <param name="accounts">The collection of accounts associated with the user.</param>
    /// <param name="streamerInfo">The collection of streamer information associated with the user.</param>
    /// <param name="offers">The collection of offers available to the user.</param>
    [JsonConstructor]
    public UserPreference(IReadOnlyCollection<Account> accounts, IReadOnlyCollection<StreamerInfo> streamerInfo, IReadOnlyCollection<Offer> offers)
        => (Accounts, StreamerInfo, Offers) = (accounts, streamerInfo, offers);
}

/// <summary>
/// Represents an offer with permissions for market data.
/// </summary>
public readonly struct Offer
{
    /// <summary>
    /// Gets a value indicating whether Level 2 permissions are granted.
    /// </summary>
    public bool Level2Permissions { get; }

    /// <summary>
    /// Gets the market data permission level.
    /// </summary>
    public string MktDataPermission { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Offer"/> struct.
    /// </summary>
    /// <param name="level2Permissions">A value indicating whether Level 2 permissions are granted.</param>
    /// <param name="mktDataPermission">The market data permission level.</param>
    [JsonConstructor]
    public Offer(bool level2Permissions, string mktDataPermission) => (Level2Permissions, MktDataPermission) = (level2Permissions, mktDataPermission);
}

/// <summary>
/// Represents streamer information for a user.
/// </summary>
public class StreamerInfo
{
    /// <summary>
    /// Gets the URL for the streamer socket.
    /// </summary>
    public string StreamerSocketUrl { get; }

    /// <summary>
    /// Gets the Schwab client customer ID.
    /// </summary>
    public string SchwabClientCustomerId { get; }

    /// <summary>
    /// Gets the Schwab client correlation ID.
    /// </summary>
    public string SchwabClientCorrelId { get; }

    /// <summary>
    /// Gets the Schwab client channel.
    /// </summary>
    public string SchwabClientChannel { get; }

    /// <summary>
    /// Gets the Schwab client function ID.
    /// </summary>
    public string SchwabClientFunctionId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamerInfo"/> class.
    /// </summary>
    /// <param name="streamerSocketUrl">The URL for the streamer socket.</param>
    /// <param name="schwabClientCustomerId">The Schwab client customer ID.</param>
    /// <param name="schwabClientCorrelId">The Schwab client correlation ID.</param>
    /// <param name="schwabClientChannel">The Schwab client channel.</param>
    /// <param name="schwabClientFunctionId">The Schwab client function ID.</param>
    [JsonConstructor]
    public StreamerInfo(string streamerSocketUrl, string schwabClientCustomerId, string schwabClientCorrelId, string schwabClientChannel, string schwabClientFunctionId)
    {
        StreamerSocketUrl = streamerSocketUrl;
        SchwabClientCustomerId = schwabClientCustomerId;
        SchwabClientCorrelId = schwabClientCorrelId;
        SchwabClientChannel = schwabClientChannel;
        SchwabClientFunctionId = schwabClientFunctionId;
    }
}

/// <summary>
/// Represents an account associated with the user.
/// </summary>
public class Account
{
    /// <summary>
    /// Gets the account number.
    /// </summary>
    public string AccountNumber { get; }

    /// <summary>
    /// Gets a value indicating whether this is the primary account.
    /// </summary>
    public bool PrimaryAccount { get; }

    /// <summary>
    /// Gets the type of the account.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the nickname of the account.
    /// </summary>
    public string NickName { get; }

    /// <summary>
    /// Gets the display account ID.
    /// </summary>
    public string DisplayAcctId { get; }

    /// <summary>
    /// Gets the auto position effect setting for the account.
    /// </summary>
    public string AutoPositionEffect { get; }

    /// <summary>
    /// Gets the color associated with the account.
    /// </summary>
    public string AccountColor { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Account"/> class.
    /// </summary>
    /// <param name="accountNumber">The account number.</param>
    /// <param name="primaryAccount">A value indicating whether this is the primary account.</param>
    /// <param name="type">The type of the account.</param>
    /// <param name="nickName">The nickname of the account.</param>
    /// <param name="displayAcctId">The display account ID.</param>
    /// <param name="autoPositionEffect">The auto position effect setting for the account.</param>
    /// <param name="accountColor">The color associated with the account.</param>
    [JsonConstructor]
    public Account(string accountNumber, bool primaryAccount, string type, string nickName, string displayAcctId, string autoPositionEffect, string accountColor)
    {
        AccountNumber = accountNumber;
        PrimaryAccount = primaryAccount;
        Type = type;
        NickName = nickName;
        DisplayAcctId = displayAcctId;
        AutoPositionEffect = autoPositionEffect;
        AccountColor = accountColor;
    }
}
