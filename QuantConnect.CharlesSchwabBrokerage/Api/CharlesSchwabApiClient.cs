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
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using QuantConnect.Brokerages.CharlesSchwab.Models;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums;

namespace QuantConnect.Brokerages.CharlesSchwab.Api;

/// <summary>
/// Represents a client for interacting with the Charles Schwab API for trading functionalities.
/// </summary>
public class CharlesSchwabApiClient
{
    /// <summary>
    /// HttpClient is used for making HTTP requests and handling HTTP responses from web resources identified by a Uri.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// The account number associated with the Charles Schwab account.
    /// </summary>
    private readonly string _accountNumber;

    /// <summary>
    /// The base URL for the Charles Schwab Trader API.
    /// </summary>
    private readonly string _traderBaseUrl;

    /// <summary>
    /// The base URL for the Charles Schwab Market Data API.
    /// </summary>
    private readonly string _marketDataBaseUrl;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharlesSchwabApiClient"/> class.
    /// </summary>
    /// <param name="baseUrl">The base URL of the Charles Schwab API.</param>
    /// <param name="appKey">The application key for authentication with the API.</param>
    /// <param name="secret">The client secret for authentication with the API.</param>
    /// <param name="accountNumber">The account number for the Charles Schwab account.</param>
    /// <param name="redirectUri">The redirect URI used during the OAuth flow.</param>
    /// <param name="authorizationCodeFromUrl">The authorization code obtained from the authorization URL during the OAuth flow.</param>
    /// <param name="refreshToken">The refresh token used for token renewal when the access token expires.</param>
    /// <param name="httpClientHandler">An optional <see cref="HttpClientHandler"/> that can be provided for advanced configuration of the <see cref="HttpClient"/>. If null, a default handler is used.</param>
    public CharlesSchwabApiClient(string baseUrl, string appKey, string secret, string accountNumber, string redirectUri, string authorizationCodeFromUrl, string refreshToken,
       HttpClientHandler httpClientHandler = null)
    {
        _traderBaseUrl = baseUrl + "/trader/v1";
        _marketDataBaseUrl = baseUrl + "/marketdata/v1";
        _accountNumber = accountNumber;

        var httpClient = httpClientHandler ?? new HttpClientHandler();
        var tokenRefreshHandler = new CharlesSchwabTokenRefreshHandler(httpClient, baseUrl, appKey, secret, redirectUri, authorizationCodeFromUrl, refreshToken);
        _httpClient = new(tokenRefreshHandler);
    }

    /// <summary>
    /// Retrieves historical price data aggregated by minute for the specified symbol.
    /// </summary>
    /// <param name="symbol">The symbol for the equity being queried (e.g., stock symbol).</param>
    /// <param name="startDateUtc">The start date in UTC, represented in milliseconds since the UNIX epoch.</param>
    /// <param name="endDateUtc">The end date in UTC, represented in milliseconds since the UNIX epoch.</param>
    /// <param name="needExtendedHoursData">Indicates whether extended hours data is required.</param>
    /// <returns>The task result contains the historical price data aggregated by minute.</returns>
    public async Task<CandleResponse> GetMinutePriceHistory(string symbol, DateTime startDateUtc, DateTime endDateUtc, bool needExtendedHoursData)
    {
        return await GetPriceHistory(symbol, startDateUtc, endDateUtc, "minute", 1, needExtendedHoursData);
    }

    /// <summary>
    /// Retrieves historical price data aggregated by 30-minute intervals for the specified symbol.
    /// </summary>
    /// <param name="symbol">The symbol for the equity being queried (e.g., stock symbol).</param>
    /// <param name="startDateUtc">The start date in UTC, represented in milliseconds since the UNIX epoch.</param>
    /// <param name="endDateUtc">The end date in UTC, represented in milliseconds since the UNIX epoch.</param>
    /// <param name="needExtendedHoursData">Indicates whether extended hours data is required.</param>
    /// <returns>The task result contains the historical price data aggregated by 30-minute intervals.</returns>
    public async Task<CandleResponse> GetThirtyMinutesPriceHistory(string symbol, DateTime startDateUtc, DateTime endDateUtc, bool needExtendedHoursData)
    {
        return await GetPriceHistory(symbol, startDateUtc, endDateUtc, "minute", 30, needExtendedHoursData);
    }

    /// <summary>
    /// Retrieves historical price data aggregated by daily intervals for the specified symbol.
    /// The data is aggregated monthly.
    /// </summary>
    /// <param name="symbol">The symbol for the equity being queried (e.g., stock symbol).</param>
    /// <param name="startDateUtc">The start date in UTC, represented in milliseconds since the UNIX epoch.</param>
    /// <param name="endDateUtc">The end date in UTC, represented in milliseconds since the UNIX epoch.</param>
    /// <param name="needExtendedHoursData">Indicates whether extended hours data is required.</param>
    /// <returns>The task result contains the historical price data aggregated by daily intervals.</returns>
    public async Task<CandleResponse> GetDailyPriceHistory(string symbol, DateTime startDateUtc, DateTime endDateUtc, bool needExtendedHoursData)
    {
        return await GetPriceHistory(symbol, startDateUtc, endDateUtc, "daily", 1, needExtendedHoursData, "month");
    }

    /// <summary>
    /// Retrieves a collection of open orders for the account.
    /// </summary>
    /// <returns>
    ///The read-only collection of open <see cref="CharlesSchwabOrder"/> objects.
    /// </returns>
    public async Task<IReadOnlyCollection<CharlesSchwabOrder>> GetOpenOrders()
    {
        // Docs remark: Date must be within 60 days from today's date.
        var fromEnteredTime = DateTime.UtcNow.AddDays(-60).ToIso8601Invariant();
        var toEnteredTime = DateTime.UtcNow.ToIso8601Invariant();

        return await RequestTraderAsync<IReadOnlyCollection<CharlesSchwabOrder>>(
            HttpMethod.Get,
            $"/accounts/{_accountNumber}/orders?fromEnteredTime={fromEnteredTime}&toEnteredTime={toEnteredTime}&status={CharlesSchwabOrderStatus.Working.ToStringInvariant().ToUpperInvariant()}");
    }

    /// <summary>
    /// Retrieves the balance and positions of the securities account.
    /// </summary>
    /// <returns>A <see cref="SecuritiesAccount"/> object that represents the account balance and positions.</returns>
    public async Task<SecuritiesAccount> GetAccountBalanceAndPosition()
    {
        return (await GetAccountByNumber(_accountNumber)).SecuritiesAccount;
    }

    /// <summary>
    /// Retrieves a meta data account by account number.
    /// </summary>
    /// <param name="accountNumber">The account number to retrieve information for.</param>
    /// <returns>A <see cref="CharlesSchwabSecuritiesAccount"/> containing the account's metadata and positions.</returns>
    private async Task<CharlesSchwabSecuritiesAccount> GetAccountByNumber(string accountNumber)
    {
        return await RequestTraderAsync<CharlesSchwabSecuritiesAccount>(HttpMethod.Get, $"/accounts/{accountNumber}?fields=positions");
    }

    /// <summary>
    /// Retrieves historical price data including Open, High, Low, Close, and Volume for the specified symbol.
    /// The frequency and periodType parameters determine the granularity of the data.
    /// </summary>
    /// <param name="symbol">The symbol for the equity being queried (e.g., stock symbol).</param>
    /// <param name="startDateUtc">The start date in UTC, represented in milliseconds since the UNIX epoch (e.g., 1451624400000).</param>
    /// <param name="endDateUtc">The end date in UTC, represented in milliseconds since the UNIX epoch.
    /// If not provided, defaults to the previous business day.</param>
    /// <param name="frequencyType">The type of frequency to aggregate the data (e.g., minute, daily, weekly, monthly).</param>
    /// <param name="frequency">The interval or duration of the frequency (e.g., 1 minute, 30 minutes).</param>
    /// <param name="needExtendedHoursData">Indicates whether extended hours data is required.</param>
    /// <param name="periodType">Optional: The period for which the data is requested (e.g., day, month, year, ytd). Defaults to null.</param>
    /// <returns>The task result contains the historical price data for the specified symbol.</returns>
    private async Task<CandleResponse> GetPriceHistory(string symbol, DateTime startDateUtc, DateTime endDateUtc, string frequencyType,
        int frequency, bool needExtendedHoursData, string periodType = null)
    {
        var breakPoint = new StringBuilder($"/pricehistory?symbol={symbol}");

        if (!string.IsNullOrEmpty(periodType))
        {
            breakPoint.Append($"&periodType={periodType}");
        }

        breakPoint.Append($"&frequencyType={frequencyType}&frequency={frequency}");
        breakPoint.Append("&startDate=" + Time.DateTimeToUnixTimeStampMilliseconds(startDateUtc));
        breakPoint.Append("&endDate=" + Time.DateTimeToUnixTimeStampMilliseconds(endDateUtc));
        breakPoint.Append("&needExtendedHoursData=" + needExtendedHoursData);

        return await RequestMarketDataAsync<CandleResponse>(HttpMethod.Get, breakPoint.ToString());
    }

    /// <summary>
    /// Sends an HTTP request to the trader API and deserializes the response into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type into which the response will be deserialized.</typeparam>
    /// <param name="httpMethod">The HTTP method to use for the request (e.g., GET, POST).</param>
    /// <param name="endpoint">The specific API endpoint including the path and query parameters.</param>
    /// <returns>An object of type <typeparamref name="T"/> containing the deserialized response data.</returns>
    /// <exception cref="ArgumentException">Thrown when the API returns an unsuccessful status code along with error details.</exception>
    /// <exception cref="Exception">Thrown when there is an error during the HTTP request or response handling.</exception>
    private async Task<T> RequestMarketDataAsync<T>(HttpMethod httpMethod, string endpoint)
    {
        return await RequestAsync<T>(httpMethod, _marketDataBaseUrl, endpoint);
    }

    /// <summary>
    /// Sends an HTTP request to the trader API and deserializes the response into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type into which the response will be deserialized.</typeparam>
    /// <param name="httpMethod">The HTTP method to use for the request (e.g., GET, POST).</param>
    /// <param name="endpoint">The specific API endpoint including the path and query parameters.</param>
    /// <returns>An object of type <typeparamref name="T"/> containing the deserialized response data.</returns>
    /// <exception cref="ArgumentException">Thrown when the API returns an unsuccessful status code along with error details.</exception>
    /// <exception cref="Exception">Thrown when there is an error during the HTTP request or response handling.</exception>
    private async Task<T> RequestTraderAsync<T>(HttpMethod httpMethod, string endpoint)
    {
        return await RequestAsync<T>(httpMethod, _traderBaseUrl, endpoint);
    }

    /// <summary>
    /// Sends an HTTP request and deserializes the response into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type into which the response will be deserialized.</typeparam>
    /// <param name="httpMethod">The HTTP method to use for the request (e.g., GET, POST).</param>
    /// <param name="baseUrl">The base URL for the API endpoint.</param>
    /// <param name="endpoint">The specific API endpoint including the path and query parameters.</param>
    /// <returns>An object of type <typeparamref name="T"/> containing the deserialized response data.</returns>
    /// <exception cref="ArgumentException">Thrown when the API returns an unsuccessful status code along with error details.</exception>
    /// <exception cref="Exception">Thrown when there is an error during the HTTP request or response handling.</exception>
    private async Task<T> RequestAsync<T>(HttpMethod httpMethod, string baseUrl, string endpoint)
    {
        using (var requestMessage = new HttpRequestMessage(httpMethod, baseUrl + endpoint))
        {
            try
            {
                var responseMessage = await _httpClient.SendAsync(requestMessage);

                var jsonContent = await responseMessage.Content.ReadAsStringAsync();

                if (!responseMessage.IsSuccessStatusCode)
                {
                    var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(jsonContent);
                    throw new ArgumentException($"{nameof(CharlesSchwabApiClient)}.{nameof(RequestAsync)}: {errorResponse.Error} - " +
                                   $"{string.Join('\n', errorResponse.ErrorDescription)}.");
                }

                return JsonConvert.DeserializeObject<T>(jsonContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(CharlesSchwabApiClient)}.{nameof(RequestAsync)}: {ex.Message}");
            }
        }
    }
}
