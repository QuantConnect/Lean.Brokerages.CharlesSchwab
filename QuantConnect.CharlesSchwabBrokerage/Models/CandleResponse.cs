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
/// <param name="Candles">The list of candlestick data.</param>
/// <param name="Symbol">The symbol for which the data is retrieved.</param>
/// <param name="Empty">Indicates if the candle data is empty or not.</param>
public record CandleResponse(
    [JsonProperty("candles")] IReadOnlyCollection<Candle> Candles,
    [JsonProperty("symbol")] string Symbol,
    [JsonProperty("empty")] bool Empty);

/// <summary>
/// Represents a single candlestick entry in a financial chart.
/// </summary>
/// <param name="Open">The opening price of the candle.</param>
/// <param name="High">The highest price reached during the candle period.</param>
/// <param name="Low">The lowest price reached during the candle period.</param>
/// <param name="Close">The closing price of the candle.</param>
/// <param name="Volume">The total volume of transactions during the candle period.</param>
/// <param name="DateTime">The timestamp of the candlestick, in Unix time converted to <see cref="DateTime"/>.</param>
public record Candle(
    [JsonProperty("open")] decimal Open,
    [JsonProperty("high")] decimal High,
    [JsonProperty("low")] decimal Low,
    [JsonProperty("close")] decimal Close,
    [JsonProperty("volume")] decimal Volume,
    [JsonProperty("datetime"), JsonConverter(typeof(CharlesSchwabUnixMillisecondsConverter))] DateTime DateTime);
