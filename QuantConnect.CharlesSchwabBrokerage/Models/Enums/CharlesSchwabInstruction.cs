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

using System.Runtime.Serialization;

namespace QuantConnect.Brokerages.CharlesSchwab.Models.Enums;

public enum CharlesSchwabInstruction
{
    /// <summary>
    /// Buy an asset.
    /// </summary>
    [EnumMember(Value = "BUY")]
    Buy = 0,

    /// <summary>
    /// Sell an asset.
    /// </summary>
    [EnumMember(Value = "SELL")]
    Sell = 1,

    /// <summary>
    /// Buy shares to cover a previously executed short sale.
    /// </summary>
    [EnumMember(Value = "BUY_TO_COVER")]
    BuyToCover = 2,

    /// <summary>
    /// Sell an asset that is not owned (short sale).
    /// </summary>
    [EnumMember(Value = "SELL_SHORT")]
    SellShort = 3,

    /// <summary>
    /// Buy to open a new position <see cref="AssetType.Option"/>.
    /// </summary>
    [EnumMember(Value = "BUY_TO_OPEN")]
    BuyToOpen = 4,

    /// <summary>
    /// Buy to close an existing position <see cref="AssetType.Option"/>.
    /// </summary>
    [EnumMember(Value = "BUY_TO_CLOSE")]
    BuyToClose = 5,

    /// <summary>
    /// Sell to open a new position, <see cref="AssetType.Option"/>.
    /// </summary>
    [EnumMember(Value = "SELL_TO_OPEN")]
    SellToOpen = 6,
    /// <summary>
    /// Sell to close an existing position, <see cref="AssetType.Option"/>.
    /// </summary>
    [EnumMember(Value = "SELL_TO_CLOSE")]
    SellToClose = 7,

    /// <summary>
    /// Exchange one asset for another.
    /// </summary>
    [EnumMember(Value = "EXCHANGE")]
    Exchange = 8,

    /// <summary>
    /// Sell an asset short with an exemption from certain regulations.
    /// </summary>
    [EnumMember(Value = "SELL_SHORT_EXEMPT")]
    SellShortExempt = 9
}
