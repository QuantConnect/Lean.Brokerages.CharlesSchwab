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
using NodaTime;
using System.Text;
using QuantConnect.Data;
using QuantConnect.Util;
using QuantConnect.Logging;
using QuantConnect.Data.Market;
using System.Collections.Generic;
using QuantConnect.Data.Consolidators;
using QuantConnect.Brokerages.CharlesSchwab.Models;

namespace QuantConnect.Brokerages.CharlesSchwab;

public partial class CharlesSchwabBrokerage
{
    /// <summary>
    /// Indicates whether the warning for invalid <see cref="SecurityType"/> has been fired.
    /// </summary>
    private volatile bool _unsupportedSecurityTypeWarningFired;

    /// <summary>
    /// Indicates whether the warning for invalid <see cref="Resolution"/> has been fired.
    /// </summary>
    private volatile bool _unsupportedResolutionTypeWarningFired;

    /// <summary>
    /// Indicates whether the warning for invalid <see cref="TickType"/> has been fired.
    /// </summary>
    private volatile bool _unsupportedTickTypeTypeWarningFired;

    /// <summary>
    /// Indicates if a warning has been logged for minute-resolution requests outside the available data range.
    /// </summary>
    private volatile bool _minuteResolutionWarningLogged;

    /// <summary>
    /// Indicates whether a warning about an invalid time range has already been logged.
    /// </summary>
    private bool _invalidTimeRangeWarningLogged;

    /// <summary>
    /// The earliest date available for <see cref="Resolution.Minute"/> requests, limited to a maximum of 45 days in the past.
    /// </summary>
    private DateTime EarliestMinuteResolutionDate { get => DateTime.UtcNow.AddDays(-45).Date; }

    /// <summary>
    /// Gets the historical data for the requested symbols.
    /// </summary>
    /// <param name="request">The historical data request.</param>
    /// <returns>An enumerable of bars covering the span specified in the request, or null if unsupported types are encountered.</returns>
    public override IEnumerable<BaseData> GetHistory(HistoryRequest request)
    {
        // Charles Schwab does not retain price history for Option or IndexOption contracts.
        if (!CanSubscribe(request.Symbol) || request.Symbol.SecurityType.IsOption())
        {
            if (!_unsupportedSecurityTypeWarningFired)
            {
                _unsupportedSecurityTypeWarningFired = true;
                var error = new StringBuilder($"{nameof(CharlesSchwabBrokerage)}.{nameof(GetHistory)}: ");
                if (request.Symbol.IsCanonical())
                {
                    Log.Trace(error.Append($"The symbol '{request.Symbol}' is in canonical form, which is not supported for historical data retrieval.").ToString());
                }
                else
                {
                    Log.Trace(error.Append($"Unsupported SecurityType '{request.Symbol.SecurityType}' for symbol '{request.Symbol}'").ToString());
                }
            }
            return null;
        }

        if (request.StartTimeUtc >= request.EndTimeUtc)
        {
            if (!_invalidTimeRangeWarningLogged)
            {
                _invalidTimeRangeWarningLogged = true;
                OnMessage(new BrokerageMessageEvent(BrokerageMessageType.Warning, "InvalidDateRange",
                    "The history request start date must precede the end date, no history returned"));
            }

            return null;
        }

        if (request.Resolution < Resolution.Minute)
        {
            if (!_unsupportedResolutionTypeWarningFired)
            {
                _unsupportedResolutionTypeWarningFired = true;
                Log.Trace($"{nameof(CharlesSchwabBrokerage)}.{nameof(GetHistory)}: Unsupported Resolution '{request.Resolution}'");
            }
            return null;
        }

        if (request.TickType != TickType.Trade)
        {
            if (!_unsupportedTickTypeTypeWarningFired)
            {
                _unsupportedTickTypeTypeWarningFired = true;
                Log.Trace($"{nameof(CharlesSchwabBrokerage)}.{nameof(GetHistory)}: Unsupported TickType '{request.TickType}'");
            }
            return null;
        }

        if (request.Resolution == Resolution.Minute && (request.EndTimeUtc < EarliestMinuteResolutionDate || request.StartTimeUtc < EarliestMinuteResolutionDate))
        {
            if (!_minuteResolutionWarningLogged)
            {
                _minuteResolutionWarningLogged = true;
                Log.Trace($"{nameof(CharlesSchwabBrokerage)}.{nameof(GetHistory)}: The specified time range (StartTime: {request.StartTimeUtc}, EndTime: {request.EndTimeUtc}) exceeds the available data range for minute resolution. The response will be empty to prevent unnecessary processing.");
            }
            return null;
        }

        return GetTradeBarByResolution(request.Resolution, request.Symbol, request.StartTimeUtc, request.EndTimeUtc, request.IncludeExtendedMarketHours,
            request.ExchangeHours.TimeZone);
    }

    /// <summary>
    /// Retrieves historical trade bars based on the specified resolution.
    /// </summary>
    /// <param name="resolution">The resolution of the data, such as Minute, Hour, or Daily.</param>
    /// <param name="symbol">The symbol for which historical data is requested.</param>
    /// <param name="startDateTime">The start date and time of the requested historical data period, in UTC.</param>
    /// <param name="endDateTime">The end date and time of the requested historical data period, in UTC.</param>
    /// <param name="includeExtendedMarketHours">Indicates whether to include data from extended market hours.</param>
    /// <param name="exchangeHoursTimeZone">The time zone of the exchange for accurate date-time conversion.</param>
    /// <returns>An enumerable of <see cref="TradeBar"/> objects for the requested period, resolution, and symbol.</returns>
    /// <exception cref="NotSupportedException">Thrown if an unsupported <see cref="Resolution"/> type is provided.</exception>
    private IEnumerable<BaseData> GetTradeBarByResolution(Resolution resolution, Symbol symbol, DateTime startDateTime, DateTime endDateTime, bool includeExtendedMarketHours, DateTimeZone exchangeHoursTimeZone)
    {
        var brokerageSymbol = _symbolMapper.GetBrokerageSymbol(symbol);

        var histories = (resolution switch
        {
            Resolution.Minute => _charlesSchwabApiClient.GetPriceHistory(brokerageSymbol, startDateTime, endDateTime, "minute", 1, includeExtendedMarketHours),
            Resolution.Daily => _charlesSchwabApiClient.GetPriceHistory(brokerageSymbol, startDateTime, endDateTime, "daily", 1, includeExtendedMarketHours, "month"),
            // API provides a maximum resolution of 30-minute bars; consolidate them for 1-hour intervals (bellow).
            Resolution.Hour => _charlesSchwabApiClient.GetPriceHistory(brokerageSymbol, startDateTime, endDateTime, "minute", 30, includeExtendedMarketHours),
            _ => throw new NotSupportedException($"{nameof(CharlesSchwabBrokerage)}.{nameof(GetTradeBarByResolution)}: Unsupported time Resolution type '{resolution}'")
        }).SynchronouslyAwaitTaskResult().Candles;

        var period = resolution.ToTimeSpan();
        if (resolution == Resolution.Hour)
        {
            return GetTradeBarWithUsingConsolidator(ConvertCandlesToTradeBars(histories, symbol, period, exchangeHoursTimeZone), period);
        }
        else
        {
            return ConvertCandlesToTradeBars(histories, symbol, period, exchangeHoursTimeZone);
        }
    }

    /// <summary>
    /// Converts a collection of candles into trade bars based on the specified period and exchange time zone.
    /// </summary>
    /// <param name="candles">The collection of <see cref="CandleResponse"/> data points to convert.</param>
    /// <param name="symbol">The symbol for the generated trade bars.</param>
    /// <param name="period">The time span representing the duration of each trade bar.</param>
    /// <param name="exchangeHoursTimeZone">The exchange's time zone for converting the candle date-time from UTC to local time.</param>
    /// <returns>An enumerable of <see cref="TradeBar"/> instances representing the candles.</returns>
    private static IEnumerable<BaseData> ConvertCandlesToTradeBars(IReadOnlyCollection<Candle> candles, Symbol symbol, TimeSpan period, DateTimeZone exchangeHoursTimeZone)
    {
        foreach (var candle in candles)
        {
            yield return new TradeBar(candle.DateTime.ConvertFromUtc(exchangeHoursTimeZone), symbol, candle.Open, candle.High, candle.Low, candle.Close, candle.Volume, period);
        }
    }

    /// <summary>
    /// Consolidates trade bars to the specified period and returns the consolidated data.
    /// </summary>
    /// <param name="tradeBars">The enumerable of trade bars to be consolidated.</param>
    /// <param name="period">The target period for consolidation.</param>
    /// <returns>An enumerable of consolidated <see cref="BaseData"/>.</returns>
    private IEnumerable<BaseData> GetTradeBarWithUsingConsolidator(IEnumerable<BaseData> tradeBars, TimeSpan period)
    {
        var consolidatedData = default(BaseData);
        var tradeBarConsolidator = new TradeBarConsolidator(period);
        EventHandler<TradeBar> onHourTradeBarConsolidator = (_, tradeBar) => { consolidatedData = tradeBar; };
        tradeBarConsolidator.DataConsolidated += onHourTradeBarConsolidator;

        foreach (var tradeBar in tradeBars)
        {
            tradeBarConsolidator.Update(tradeBar);
            if (consolidatedData != null)
            {
                yield return consolidatedData;
                consolidatedData = null;
            }
        }

        tradeBarConsolidator.DataConsolidated -= onHourTradeBarConsolidator;
        tradeBarConsolidator.DisposeSafely();
    }
}