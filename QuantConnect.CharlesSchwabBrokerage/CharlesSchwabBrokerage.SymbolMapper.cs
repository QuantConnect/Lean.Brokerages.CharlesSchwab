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

namespace QuantConnect.Brokerages.CharlesSchwab;

/// <summary>
/// Provides the mapping between Lean symbols and brokerage specific symbols.
/// </summary>
public class CharlesSchwabBrokerageSymbolMapper : ISymbolMapper
{
    public string GetBrokerageSymbol(Symbol symbol)
    {
        switch (symbol.SecurityType)
        {
            case SecurityType.Equity:
                return symbol.Value;
            default:
                throw new NotImplementedException($"{nameof(CharlesSchwabBrokerageSymbolMapper)}.{nameof(GetBrokerageSymbol)}: " +
                    $"The security type '{symbol.SecurityType}' is not supported.");
        }
    }

    public Symbol GetLeanSymbol(string brokerageSymbol, SecurityType securityType, string market, DateTime expirationDate = default, decimal strike = 0, OptionRight optionRight = OptionRight.Call)
    {
        switch (securityType)
        {
            case SecurityType.Equity:
                return Symbol.Create(brokerageSymbol, SecurityType.Equity, market);
            default:
                throw new NotImplementedException($"{nameof(CharlesSchwabBrokerageSymbolMapper)}.{nameof(GetLeanSymbol)}: " +
                    $"The security type '{securityType}' with brokerage symbol '{brokerageSymbol}' is not supported.");
        }
    }
}
