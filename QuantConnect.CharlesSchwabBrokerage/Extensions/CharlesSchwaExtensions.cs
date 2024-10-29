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
}
