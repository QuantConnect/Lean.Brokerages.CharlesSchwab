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
using System.Collections.Generic;

namespace QuantConnect.Brokerages.CharlesSchwab;

/// <summary>
/// Provides the mapping between Lean symbols and brokerage specific symbols.
/// </summary>
public class CharlesSchwabBrokerageSymbolMapper : ISymbolMapper
{
    /// <summary>
    /// Represents a set of supported security types.
    /// </summary>
    /// <remarks>
    /// This HashSet contains the supported security types that are allowed within the system.
    /// </remarks>
    public readonly HashSet<SecurityType> SupportedSecurityType = new()
    {
        SecurityType.Index,
        SecurityType.Equity,
        SecurityType.Future,
        SecurityType.Option,
        SecurityType.IndexOption,
    };

    /// <summary>
    /// Converts a Lean symbol instance to a brokerage symbol
    /// </summary>
    /// <param name="symbol">A Lean symbol instance</param>
    /// <returns> The brokerage symbol</returns>
    /// <exception cref="NotImplementedException">The lean security type is not implemented.</exception>
    public string GetBrokerageSymbol(Symbol symbol)
    {
        switch (symbol.SecurityType)
        {
            case SecurityType.Equity:
                return symbol.Value;
            case SecurityType.Future:
                return GenerateBrokerageFuture(symbol);
            case SecurityType.Index:
                return GenerateBrokerageIndex(symbol);
            case SecurityType.Option:
                return GenerateBrokerageOption(symbol);
            case SecurityType.IndexOption:
                return GenerateBrokerageOption(symbol);
            default:
                throw new NotImplementedException($"{nameof(CharlesSchwabBrokerageSymbolMapper)}.{nameof(GetBrokerageSymbol)}: " +
                    $"The security type '{symbol.SecurityType}' is not supported.");
        }
    }

    /// <summary>
    /// Converts a brokerage symbol to a Lean symbol instance
    /// </summary>
    /// <param name="brokerageSymbol">The brokerage symbol</param>
    /// <param name="securityType">The security type</param>
    /// <param name="market">The market</param>
    /// <param name="expirationDate">Expiration date of the security(if applicable)</param>
    /// <param name="strike">The strike of the security (if applicable)</param>
    /// <param name="optionRight">The option right of the security (if applicable)</param>
    /// <returns>A new Lean Symbol instance</returns>
    /// <exception cref="NotImplementedException">The security type is not implemented or not supported.</exception>
    public Symbol GetLeanSymbol(string brokerageSymbol, SecurityType securityType, string market, DateTime expirationDate = default, decimal strike = 0, OptionRight optionRight = OptionRight.Call)
    {
        switch (securityType)
        {
            case SecurityType.Equity:
                return Symbol.Create(brokerageSymbol, securityType, market);
            case SecurityType.Option:
                return SymbolRepresentation.ParseOptionTickerOSI(brokerageSymbol, securityType, securityType.DefaultOptionStyle(), market);
            default:
                throw new NotImplementedException($"{nameof(CharlesSchwabBrokerageSymbolMapper)}.{nameof(GetLeanSymbol)}: " +
                    $"The security type '{securityType}' with brokerage symbol '{brokerageSymbol}' is not supported.");
        }
    }

    /// <summary>
    /// Generates a brokerage option string based on the Lean symbol.
    /// </summary>
    /// <param name="symbol">The symbol object containing information about the option.</param>
    /// <returns>A string representing the brokerage option.</returns>
    /// <example>{AAPL 240510C167.5}</example>
    private string GenerateBrokerageOption(Symbol symbol)
    {
        var strike = symbol.ID.StrikePrice.ToStringInvariant("00000.000").Replace(".", "");
        return $"{symbol.Underlying.Value,-6}{symbol.ID.Date:yyMMdd}{symbol.ID.OptionRight.ToString()[0]}{strike}";
    }

    /// <summary>
    /// Generates a brokerage future string based on the provided symbol.
    /// </summary>
    /// <param name="symbol">The symbol object containing information about the future.</param>
    /// <returns>A string representing the brokerage future.</returns>
    /// <example>{/ESZ24}</example>
    private string GenerateBrokerageFuture(Symbol symbol)
    {
        return $"/{symbol.ID.Symbol}{SymbolRepresentation.FuturesMonthLookup[symbol.ID.Date.Month]}{symbol.ID.Date:yy}";
    }

    /// <summary>
    /// Generates a brokerage index string based on the provided symbol.
    /// </summary>
    /// <param name="symbol">The symbol object containing information about the index.</param>
    /// <returns>A string representing the brokerage index.</returns>
    /// <example>{$SPX}</example>
    private string GenerateBrokerageIndex(Symbol symbol)
    {
        return "$" + symbol.Value;
    }
}
