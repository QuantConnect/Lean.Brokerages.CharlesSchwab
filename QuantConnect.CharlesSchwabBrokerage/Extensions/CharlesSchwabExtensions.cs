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
using QuantConnect.Securities;
using QuantConnect.Orders.TimeInForces;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums;
using CharlesSchwabOrderStatus = QuantConnect.Brokerages.CharlesSchwab.Models.Enums.OrderStatus;

namespace QuantConnect.Brokerages.CharlesSchwab.Extensions;

/// <summary>
/// Provides extension methods.
/// </summary
public static class CharlesSchwabExtensions
{
    /// <summary>
    /// Converts a Charles Schwab <see cref="AssetType"/> to its equivalent Lean <see cref="SecurityType"/>.
    /// </summary>
    /// <param name="assetType">The Charles Schwab asset type to convert.</param>
    /// <param name="optionUnderlyingSymbol">
    /// The underlying symbol for the option asset type. 
    /// Used to distinguish between regular options and index options.
    /// </param>
    /// <returns>The equivalent Lean <see cref="SecurityType"/>.</returns>
    public static SecurityType ConvertAssetTypeToSecurityType(this AssetType assetType, string optionUnderlyingSymbol = default) => assetType switch
    {
        AssetType.Equity => SecurityType.Equity,
        AssetType.Option when !optionUnderlyingSymbol.StartsWith(CharlesSchwabBrokerageSymbolMapper.IndexSymbol) => SecurityType.Option,
        AssetType.Option when optionUnderlyingSymbol.StartsWith(CharlesSchwabBrokerageSymbolMapper.IndexSymbol) => SecurityType.IndexOption,
        AssetType.Index => SecurityType.Index,
        _ => throw new NotSupportedException($"{nameof(CharlesSchwabExtensions)}.{nameof(ConvertAssetTypeToSecurityType)}: " +
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
        SecurityType.IndexOption => AssetType.Option,
        _ => throw new NotSupportedException($"{nameof(CharlesSchwabExtensions)}.{nameof(ConvertSecurityTypeToAssetType)}: " +
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
                throw new NotSupportedException($"{nameof(CharlesSchwabExtensions)}.{nameof(IsShort)}: The '{instruction}' is not supported. Please provide a valid instruction type.");
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
                throw new NotSupportedException($"{nameof(CharlesSchwabExtensions)}.{nameof(GetDurationByTimeInForce)}: The TimeInForce '{timeInForce}' is not supported.");
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
                throw new NotSupportedException($"{nameof(CharlesSchwabExtensions)}.{nameof(IsExtendedRegularTradingHoursBySessionType)}: The session type '{sessionType}' is not supported.");
        }
    }

    /// <summary>
    /// Determines whether the specified order status indicates that the order is open.
    /// </summary>
    /// <param name="orderStatus">The order status to check.</param>
    /// <returns>
    /// <c>true</c> if the order is open; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="NotImplementedException">
    /// Thrown when the order status is not recognized.
    /// </exception>
    public static bool IsOrderOpen(this CharlesSchwabOrderStatus orderStatus)
    {
        switch (orderStatus)
        {
            case CharlesSchwabOrderStatus.AwaitingParentOrder:
            case CharlesSchwabOrderStatus.AwaitingCondition:
            case CharlesSchwabOrderStatus.AwaitingStopCondition:
            case CharlesSchwabOrderStatus.AwaitingManualReview:
            case CharlesSchwabOrderStatus.Accepted:
            case CharlesSchwabOrderStatus.PendingActivation:
            case CharlesSchwabOrderStatus.Queued:
            case CharlesSchwabOrderStatus.Working:
            case CharlesSchwabOrderStatus.PendingReplace:
            case CharlesSchwabOrderStatus.New:
            case CharlesSchwabOrderStatus.AwaitingReleaseTime:
            case CharlesSchwabOrderStatus.PendingAcknowledgement:
            case CharlesSchwabOrderStatus.PendingRecall:
                return true;
            case CharlesSchwabOrderStatus.Replaced:
            case CharlesSchwabOrderStatus.AwaitingUrOut:
            case CharlesSchwabOrderStatus.Rejected:
            case CharlesSchwabOrderStatus.PendingCancel:
            case CharlesSchwabOrderStatus.Canceled:
            case CharlesSchwabOrderStatus.Filled:
            case CharlesSchwabOrderStatus.Expired:
            case CharlesSchwabOrderStatus.Unknown:
                return false;
            default:
                throw new NotImplementedException($"{nameof(CharlesSchwabExtensions)}.{nameof(IsOrderOpen)}: The order status '{orderStatus}' is not implemented.");
        }
    }

    /// <summary>
    /// Retrieves the time zone of the exchange for the given symbol.
    /// </summary>
    /// <param name="symbol">The symbol for which to get the exchange time zone.</param>
    /// <returns>
    /// The <see cref="NodaTime.DateTimeZone"/> representing the time zone of the exchange
    /// where the given symbol is traded.
    /// </returns>
    /// <remarks>
    /// This method uses the <see cref="MarketHoursDatabase"/> to fetch the exchange hours
    /// and extract the time zone information for the provided symbol.
    /// </remarks>
    public static NodaTime.DateTimeZone GetSymbolExchangeTimeZone(this Symbol symbol)
        => MarketHoursDatabase.FromDataFolder().GetExchangeHours(symbol.ID.Market, symbol, symbol.SecurityType).TimeZone;

    /// <summary>
    /// Converts the given <see cref="IConvertible"/> object to its string representation
    /// in an uppercase format using invariant culture.
    /// </summary>
    /// <param name="convertible">The <see cref="IConvertible"/> object to be converted.</param>
    /// <returns>
    /// A string representation of the object in uppercase using invariant culture, or an empty
    /// string if the input is <c>null</c>.
    /// </returns>
    public static string ToUpperStringInvariant(this IConvertible convertible)
    {
        if (convertible == null)
        {
            return string.Empty;
        }

        return convertible.ToStringInvariant().ToUpperInvariant();
    }

    /// <summary>
    /// Truncates the given <see cref="DateTime"/> to the specified <see cref="TimeSpan"/> precision.
    /// </summary>
    /// <param name="dateTime">The <see cref="DateTime"/> value to be truncated.</param>
    /// <param name="timeSpan">The <see cref="TimeSpan"/> indicating the precision to truncate to. 
    /// For example, use <c>TimeSpan.FromSeconds(1)</c> to truncate to the nearest second.</param>
    /// <returns>A new <see cref="DateTime"/> truncated to the specified precision. If <paramref name="timeSpan"/> is <see cref="TimeSpan.Zero"/>, the original <see cref="DateTime"/> is returned.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="timeSpan"/> has a value that exceeds the <see cref="DateTime.Ticks"/> range.</exception>
    public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
    {
        if (timeSpan == TimeSpan.Zero) return dateTime;
        return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
    }
}
