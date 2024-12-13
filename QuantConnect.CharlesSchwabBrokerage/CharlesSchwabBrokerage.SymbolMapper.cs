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
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace QuantConnect.Brokerages.CharlesSchwab;

/// <summary>
/// Provides the mapping between Lean symbols and brokerage specific symbols.
/// </summary>
public class CharlesSchwabBrokerageSymbolMapper : ISymbolMapper
{
    /// <summary>
    /// The symbol prefix used to identify index-related securities in Charles Schwab data.
    /// </summary>
    public const char IndexSymbol = '$';

    /// <summary>
    /// A concurrent dictionary that maps brokerage symbols to Lean symbols.
    /// </summary>
    private ConcurrentDictionary<string, Symbol> _leanSymbolByBrokerageSymbol = new();

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
        if (TryGetBrokerageSymbol(symbol, out var brokerageSymbol))
        {
            return brokerageSymbol;
        }

        switch (symbol.SecurityType)
        {
            case SecurityType.Equity:
                brokerageSymbol = symbol.Value;
                break;
            case SecurityType.Index:
                brokerageSymbol = GenerateBrokerageIndex(symbol);
                break;
            case SecurityType.Option:
                brokerageSymbol = GenerateBrokerageOption(symbol);
                break;
            case SecurityType.IndexOption:
                brokerageSymbol = GenerateBrokerageOption(symbol);
                break;
            default:
                throw new NotImplementedException($"{nameof(CharlesSchwabBrokerageSymbolMapper)}.{nameof(GetBrokerageSymbol)}: " +
                    $"The security type '{symbol.SecurityType}' is not supported.");
        }

        _leanSymbolByBrokerageSymbol[brokerageSymbol] = symbol;

        return brokerageSymbol;
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
        if (_leanSymbolByBrokerageSymbol.TryGetValue(brokerageSymbol, out var leanSymbol))
        {
            return leanSymbol;
        }

        switch (securityType)
        {
            case SecurityType.Equity:
                leanSymbol = Symbol.Create(brokerageSymbol, securityType, market);
                break;
            case SecurityType.Option:
            case SecurityType.IndexOption:
                leanSymbol = SymbolRepresentation.ParseOptionTickerOSI(brokerageSymbol, securityType, securityType.DefaultOptionStyle(), market);
                break;
            default:
                throw new NotImplementedException($"{nameof(CharlesSchwabBrokerageSymbolMapper)}.{nameof(GetLeanSymbol)}: " +
                    $"The security type '{securityType}' with brokerage symbol '{brokerageSymbol}' is not supported.");
        }

        _leanSymbolByBrokerageSymbol[brokerageSymbol] = leanSymbol;
        return leanSymbol;
    }

    /// <summary>
    /// Generates a brokerage option string based on the Lean symbol.
    /// </summary>
    /// <param name="symbol">The symbol object containing information about the option.</param>
    /// <returns>A string representing the brokerage option. (e.g.: AAPL  251219C00200000)</returns>
    /// <remarks>
    /// The Option symbol format: RRRRRRYYMMDDsWWWWWddd
    /// <list type="bullet">
    ///    <item>
    ///        <term>R</term>
    ///        <description>is the space-filled root</description>
    ///    </item>
    ///     <item>
    ///        <term>YY</term>
    ///        <description>is the expiration year</description>
    ///    </item>
    ///     <item>
    ///        <term>MM</term>
    ///        <description>is the expiration month</description>
    ///    </item>
    ///     <item>
    ///        <term>DD</term>
    ///        <description>is the expiration day</description>
    ///    </item>
    ///     <item>
    ///        <term>s</term>
    ///        <description>is the side: C/P(call/put)</description>
    ///    </item>
    ///     <item>
    ///        <term>WWWWW</term>
    ///        <description>is the whole portion of the strike price</description>
    ///    </item>
    ///     <item>
    ///        <term>nnn</term>
    ///        <description>is the decimal portion of the strike price</description>
    ///    </item>
    ///</list>
    /// </remarks>
    private string GenerateBrokerageOption(Symbol symbol)
    {
        var strike = symbol.ID.StrikePrice.ToStringInvariant("00000.000").Replace(".", "");
        return $"{symbol.Underlying.Value,-6}{symbol.ID.Date:yyMMdd}{symbol.ID.OptionRight.ToString()[0]}{strike}";
    }

    /// <summary>
    /// Generates a brokerage index string based on the provided symbol.
    /// </summary>
    /// <param name="symbol">The symbol object containing information about the index.</param>
    /// <returns>A string representing the brokerage index.</returns>
    /// <example>{$SPX}</example>
    private string GenerateBrokerageIndex(Symbol symbol)
    {
        return IndexSymbol + symbol.Value;
    }

    /// <summary>
    /// Attempts to retrieve the brokerage symbol associated with a given <see cref="Symbol"/>.
    /// </summary>
    /// <param name="symbol">The <see cref="Symbol"/> for which the brokerage symbol is to be retrieved.</param>
    /// <param name="brokerageSymbol">The brokerage symbol corresponding to the given <see cref="Symbol"/> if found; otherwise, an empty string.</param>
    /// <returns>True if the brokerage symbol is found; otherwise, false.</returns>
    private bool TryGetBrokerageSymbol(Symbol symbol, out string brokerageSymbol)
    {
        brokerageSymbol = _leanSymbolByBrokerageSymbol.FirstOrDefault(k => k.Value == symbol).Key;
        return string.IsNullOrEmpty(brokerageSymbol) ? false : true;
    }
}
