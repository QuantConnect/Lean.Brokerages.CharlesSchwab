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
using System.Linq;
using QuantConnect.Logging;
using QuantConnect.Interfaces;
using System.Collections.Generic;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums;

namespace QuantConnect.Brokerages.CharlesSchwab;

/// <summary>
/// Represents a brokerage integration with Charles Schwab, providing options and symbol lookup functionality.
/// </summary>
public partial class CharlesSchwabBrokerage : IDataQueueUniverseProvider
{
    /// <summary>
    /// Returns a collection of option Symbols that are available at the data source for the given Symbol.
    /// </summary>
    /// <param name="symbol">The Symbol to lookup options for.</param>
    /// <param name="_">A boolean to indicate whether to include expired contracts. This parameter is not used as Charles Schwab only returns non-expired contracts.</param>
    /// <param name="securityCurrency">The expected currency of the security, if applicable.</param>
    /// <returns>An enumerable collection of <see cref="Symbol"/> objects that are associated with the provided Symbol.</returns>
    public IEnumerable<Symbol> LookupSymbols(Symbol symbol, bool _, string securityCurrency = null)
    {
        if (!symbol.SecurityType.IsOption())
        {
            Log.Error($"{nameof(CharlesSchwabBrokerage)}.{nameof(LookupSymbols)}: The provided symbol is not an option. SecurityType: " + symbol.SecurityType);
            return Enumerable.Empty<Symbol>();
        }

        var underlyingSymbol = _symbolMapper.GetBrokerageSymbol(symbol.Underlying);
        return GetOptionChainByUnderlyingSymbol(underlyingSymbol);
    }

    /// <summary>
    /// Retrieves the option chain for a given underlying symbol.
    /// </summary>
    /// <param name="underlyingSymbol">The underlying symbol for which to retrieve the option chain.</param>
    /// <returns>An enumerable collection of <see cref="Symbol"/> objects representing the options available for the underlying symbol.</returns>
    /// <remarks>
    /// This method fetches both call and put option chains from the Charles Schwab API client and converts them to Lean <see cref="Symbol"/> objects.
    /// </remarks>
    private IEnumerable<Symbol> GetOptionChainByUnderlyingSymbol(string underlyingSymbol)
    {
        var optionCallChain = _charlesSchwabApiClient.GetOptionChainBySymbolAndOptionRight(underlyingSymbol, OptionRight.Call).SynchronouslyAwaitTaskResult();
        var optionPutChain = _charlesSchwabApiClient.GetOptionChainBySymbolAndOptionRight(underlyingSymbol, OptionRight.Put).SynchronouslyAwaitTaskResult();

        var securityType = GetOptionSecurityTypeByAssetType(optionCallChain.AssetType);

        foreach (var expDateMap in optionCallChain.CallExpDateMap.Values.Concat(optionPutChain.PutExpDateMap.Values))
        {
            foreach (var strikesMetaData in expDateMap.Values)
            {
                foreach (var option in strikesMetaData)
                {
                    yield return _symbolMapper.GetLeanSymbol(option.Symbol, securityType, Market.USA);
                }
            }
        }
    }

    /// <summary>
    /// Determines the appropriate <see cref="SecurityType"/> based on the given <see cref="AssetType"/>.
    /// </summary>
    /// <param name="assetType">The asset type to evaluate.</param>
    /// <returns>The corresponding <see cref="SecurityType"/> for the provided asset type.</returns>
    /// <exception cref="NotSupportedException">Thrown when the asset type is not supported.</exception>
    private static SecurityType GetOptionSecurityTypeByAssetType(AssetType assetType) => assetType switch
    {
        AssetType.Equity => SecurityType.Option,
        AssetType.Index => SecurityType.IndexOption,
        _ => throw new NotSupportedException($"{nameof(CharlesSchwabBrokerage)}.{nameof(GetOptionSecurityTypeByAssetType)}: The AssetType '{assetType}' is not supported.")
    };

    /// <summary>
    /// Returns whether selection can take place or not.
    /// </summary>
    /// <remarks>This is useful to avoid a selection taking place during invalid times, for example IB reset times or when not connected,
    /// because if allowed selection would fail since IB isn't running and would kill the algorithm</remarks>
    /// <returns>True if selection can take place</returns>
    public bool CanPerformSelection()
    {
        return IsConnected;
    }
}
