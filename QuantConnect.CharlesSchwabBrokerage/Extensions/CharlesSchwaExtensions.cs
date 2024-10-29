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

using QuantConnect.Brokerages.CharlesSchwab.Models.Enums;
using System;

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
}
