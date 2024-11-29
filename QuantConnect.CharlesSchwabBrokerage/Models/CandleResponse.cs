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
using Newtonsoft.Json;
using System.Collections.Generic;
using QuantConnect.Brokerages.CharlesSchwab.Converters;

namespace QuantConnect.Brokerages.CharlesSchwab.Models;

/// <summary>
/// Represents a collection of candlestick data for a given symbol from Charles Schwab's API.
/// </summary>
public class CandleResponse
{
    /// <summary>
    /// The list of candlestick data for the symbol.
    /// </summary>
    [JsonProperty("candles")]
    public IReadOnlyCollection<Candle> Candles { get; }

    /// <summary>
    /// The symbol for which the candlestick data is retrieved.
    /// </summary>
    [JsonProperty("symbol")]
    public string Symbol { get; }

    /// <summary>
    /// Indicates whether the candlestick data is empty or not.
    /// </summary>
    [JsonProperty("empty")]
    public bool Empty { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CandleResponse"/> class.
    /// </summary>
    /// <param name="candles">The list of candlestick data.</param>
    /// <param name="symbol">The symbol for which the data is retrieved.</param>
    /// <param name="empty">Indicates if the candle data is empty.</param>
    [JsonConstructor]
    public CandleResponse(IReadOnlyCollection<Candle> candles, string symbol, bool empty) => (Candles, Symbol, Empty) = (candles, symbol, empty);
}

/// <summary>
/// Represents a single candlestick entry in a financial chart.
/// </summary>
public class Candle
{
    /// <summary>
    /// The opening price of the candle.
    /// </summary>
    [JsonProperty("open")]
    public decimal Open { get; }

    /// <summary>
    /// The highest price reached during the candle period.
    /// </summary>
    [JsonProperty("high")]
    public decimal High { get; }

    /// <summary>
    /// The lowest price reached during the candle period.
    /// </summary>
    [JsonProperty("low")]
    public decimal Low { get; }

    /// <summary>
    /// The closing price of the candle.
    /// </summary>
    [JsonProperty("close")]
    public decimal Close { get; }

    /// <summary>
    /// The total volume of transactions during the candle period.
    /// </summary>
    [JsonProperty("volume")]
    public decimal Volume { get; }

    /// <summary>
    /// The timestamp of the candlestick, in Unix time converted to <see cref="DateTime"/>.
    /// </summary>
    [JsonProperty("datetime"), JsonConverter(typeof(CharlesSchwabUnixMillisecondsConverter))]
    public DateTime DateTime { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Candle"/> class with specified values.
    /// </summary>
    /// <param name="open">The opening price of the candle.</param>
    /// <param name="high">The highest price reached during the candle period.</param>
    /// <param name="low">The lowest price reached during the candle period.</param>
    /// <param name="close">The closing price of the candle.</param>
    /// <param name="volume">The total volume of transactions during the candle period.</param>
    /// <param name="dateTime">The timestamp of the candlestick, converted from Unix time to <see cref="DateTime"/>.</param>
    [JsonConstructor]
    public Candle(decimal open, decimal high, decimal low, decimal close, decimal volume, DateTime dateTime)
        => (Open, High, Low, Close, Volume, DateTime) = (open, high, low, close, volume, dateTime);
}