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

using System;
using QuantConnect.Orders;
using QuantConnect.Interfaces;
using QuantConnect.Orders.TimeInForces;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums;

namespace QuantConnect.Brokerages.CharlesSchwab.Extensions;

/// <summary>
/// Provides extension methods.
/// </summary
public static class CharlesSchwaExtensions
{
    /// <summary>
    /// Converts a Charles Schwab asset type to its equivalent Lean SecurityType.
    /// </summary>
    /// <param name="assetType">The Charles Schwab asset type to convert.</param>
    /// <returns>The equivalent Lean <see cref="SecurityType"/>.</returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when the provided <paramref name="assetType"/> is not supported.
    /// </exception>
    public static SecurityType ConvertAssetTypeToSecurityType(this AssetType assetType) => assetType switch
    {
        AssetType.Equity => SecurityType.Equity,
        AssetType.Option => SecurityType.Option,
        AssetType.Index => SecurityType.Index,
        _ => throw new NotSupportedException($"{nameof(CharlesSchwaExtensions)}.{nameof(ConvertAssetTypeToSecurityType)}: " +
            $"The AssetType '{assetType}' is not supported.")
    };

    /// <summary>
    /// Converts a <see cref="SecurityType"/> to its corresponding <see cref="AssetType"/>.
    /// </summary>
    /// <param name="securityType">The security type to convert.</param>
    /// <returns>
    /// Returns the corresponding <see cref="AssetType"/> for the specified <paramref name="securityType"/>.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when an unsupported <see cref="SecurityType"/> is provided.
    /// </exception>
    public static AssetType ConvertSecurityTypeToAssetType(this SecurityType securityType) => securityType switch
    {
        SecurityType.Equity => AssetType.Equity,
        SecurityType.Option => AssetType.Option,
        SecurityType.Index => AssetType.Index,
        _ => throw new NotSupportedException($"{nameof(CharlesSchwaExtensions)}.{nameof(ConvertSecurityTypeToAssetType)}: " +
            $"The SecurityType '{securityType}' is not supported.")
    };

    /// <summary>
    /// Sets the <see cref="TimeInForce"/> for the given <see cref="CharlesSchwabOrderProperties"/>
    /// based on the specified duration.
    /// </summary>
    /// <param name="orderProperties">The order properties to update.</param>
    /// <param name="brokerageDuration">The duration for the order's time in force.</param>
    /// <param name="goodTilDateTime">
    /// The expiration date when <paramref name="brokerageDuration"/> is <see cref="Duration.GoodTillCancel"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> if <see cref="TimeInForce"/> was set; otherwise, <c>false</c>.
    /// </returns>
    public static bool GetLeanTimeInForce(this CharlesSchwabOrderProperties orderProperties, Duration brokerageDuration, DateTime goodTilDateTime)
    {
        switch (brokerageDuration)
        {
            case Duration.Day:
                orderProperties.TimeInForce = TimeInForce.Day;
                return true;
            case Duration.GoodTillCancel:
                orderProperties.TimeInForce = TimeInForce.GoodTilDate(goodTilDateTime);
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Determines whether the specified trade action type is a short sell action.
    /// </summary>
    /// <param name="instruction">The trade action type to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the trade action type is one of the short sell actions; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when the trade action type is not recognized or supported.
    /// </exception>
    public static bool IsShort(this Instruction instruction)
    {
        switch (instruction)
        {
            case Instruction.Sell:
            case Instruction.SellShort:
            case Instruction.SellToOpen:
            case Instruction.SellToClose:
                return true;

            case Instruction.Buy:
            case Instruction.BuyToCover:
            case Instruction.BuyToClose:
            case Instruction.BuyToOpen:
                return false;

            default:
                throw new NotSupportedException($"{nameof(CharlesSchwaExtensions)}.{nameof(IsShort)}: The '{instruction}' is not supported. Please provide a valid instruction type.");
        }
    }

    /// <summary>
    /// Determines the session type based on the <paramref name="orderProperties"/> provided.
    /// </summary>
    /// <param name="orderProperties">The order properties to evaluate, expected to be of type <see cref="CharlesSchwabOrderProperties"/>.</param>
    /// <param name="defaultSessionType">The default session type to return if <paramref name="orderProperties"/> is not configured for extended trading hours.</param>
    /// <returns>
    /// Returns <see cref="SessionType.Seamless"/> if <paramref name="orderProperties"/> is an instance of
    /// <see cref="CharlesSchwabOrderProperties"/> with <c>ExtendedRegularTradingHours</c> set to <c>true</c>;
    /// otherwise, returns <paramref name="defaultSessionType"/>.
    /// </returns>
    public static SessionType GetExtendedHoursSessionTypeOrDefault(this IOrderProperties orderProperties, SessionType defaultSessionType)
    {
        if (orderProperties is CharlesSchwabOrderProperties { ExtendedRegularTradingHours: true })
        {
            return SessionType.Seamless;
        }
        return defaultSessionType;
    }

    /// <summary>
    /// Maps an <see cref="OrderDirection"/> to the corresponding <see cref="Instruction"/>.
    /// </summary>
    /// <param name="orderDirection">The order direction to convert.</param>
    /// <returns>
    /// Returns the corresponding <see cref="Instruction"/> for the specified <paramref name="orderDirection"/>.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when an unsupported <see cref="OrderDirection"/> is provided.
    /// </exception>
    public static Instruction GetInstructionByDirection(this OrderDirection orderDirection)
    {
        switch (orderDirection)
        {
            case OrderDirection.Sell:
                return Instruction.Sell;
            case OrderDirection.Buy:
                return Instruction.Buy;
            default:
                throw new NotSupportedException($"{nameof(CharlesSchwaExtensions)}.{nameof(GetInstructionByDirection)}: The specified order direction '{orderDirection}' is not supported.");
        }
    }

    /// <summary>
    /// Gets the duration and optional cancellation time based on the TimeInForce value.
    /// </summary>
    /// <param name="timeInForce">The TimeInForce value.</param>
    /// <returns>A tuple containing the Duration and optional expiry DateTime.</returns>
    public static (Duration Duration, DateTime? ExpiryDateTime) GetDurationByTimeInForce(this TimeInForce timeInForce)
    {
        var expiryDateTime = default(DateTime?); // Use nullable DateTime for clarity
        var duration = default(Duration);
        switch (timeInForce)
        {
            case DayTimeInForce:
                duration = Duration.Day;
                break;
            case GoodTilCanceledTimeInForce:
                duration = Duration.GoodTillCancel;
                break;
            case GoodTilDateTimeInForce goodTilDateTime:
                duration = Duration.GoodTillCancel;
                expiryDateTime = goodTilDateTime.Expiry;
                break;
            default:
                throw new NotSupportedException($"{nameof(CharlesSchwaExtensions)}.{nameof(GetDurationByTimeInForce)}: The TimeInForce '{timeInForce}' is not supported.");
        }
        return (duration, expiryDateTime);
    }

    /// <summary>
    /// Determines whether the specified session type is considered an extended trading session
    /// (either pre-market or after-hours trading).
    /// </summary>
    /// <param name="sessionType">The session type to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the session type is pre-market, post-market, or seamless (extended trading hours); 
    /// <c>false</c> if it is a regular trading session.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the specified <paramref name="sessionType"/> is not recognized.
    /// </exception>
    public static bool IsExtendedRegularTradingHoursBySessionType(this SessionType sessionType)
    {
        switch (sessionType)
        {
            case SessionType.Am:
            case SessionType.Pm:
            case SessionType.Seamless:
                return true;
            case SessionType.Normal:
                return false;
            default:
                throw new NotSupportedException($"{nameof(CharlesSchwaExtensions)}.{nameof(IsExtendedRegularTradingHoursBySessionType)}: The session type '{sessionType}' is not supported.");
        }
    }
}
