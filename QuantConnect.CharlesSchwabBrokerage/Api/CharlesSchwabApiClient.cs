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
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Collections.Generic;
using QuantConnect.Brokerages.CharlesSchwab.Models;
using QuantConnect.Brokerages.CharlesSchwab.Extensions;
using QuantConnect.Brokerages.CharlesSchwab.Models.Requests;

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
    /// The account hash number associated with the Charles Schwab account.
    /// </summary>
    private readonly Lazy<string> _accountHashNumber;

    /// <summary>
    /// The base URL for the Charles Schwab Trader API.
    /// </summary>
    private readonly string _traderBaseUrl;

    /// <summary>
    /// The base URL for the Charles Schwab Market Data API.
    /// </summary>
    private readonly string _marketDataBaseUrl;

    /// <summary>
    /// Handler responsible for refreshing OAuth tokens using Charles Schwab's API.
    /// </summary>
    private readonly CharlesSchwabTokenRefreshHandler _tokenRefreshHandler;

    /// <summary>
    /// Provides JSON serializer settings for order requests, ensuring that DateTime values are handled in UTC format.
    /// </summary>
    private readonly JsonSerializerSettings _orderRequestJsonSerializerSettings = new() { DateTimeZoneHandling = DateTimeZoneHandling.Utc };

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
        _accountHashNumber = new Lazy<string>(() =>
        {
            // Charles Schwab's web UI returns account numbers with hyphens.
            accountNumber = accountNumber.Replace("-", "");
            return GetAccountNumbers().SynchronouslyAwaitTaskResult().Single(an => an.AccountNumber == accountNumber).HashValue;
        });
        var httpClient = httpClientHandler ?? new HttpClientHandler();
        _tokenRefreshHandler = new CharlesSchwabTokenRefreshHandler(httpClient, baseUrl, appKey, secret, redirectUri, authorizationCodeFromUrl, refreshToken);
        _httpClient = new(_tokenRefreshHandler);
    }

    /// <summary>
    /// Asynchronously retrieves an access token for authenticated requests, refreshing if necessary.
    /// </summary>
    /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
    /// <returns>The access token to get access on Charles Schwab API.</returns>
    internal async Task<string> GetAccessToken(CancellationToken cancellationToken)
    {
        return await _tokenRefreshHandler.GetAccessToken(cancellationToken);
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
    public async Task<CandleResponse> GetPriceHistory(string symbol, DateTime startDateUtc, DateTime endDateUtc, string frequencyType,
        int frequency, bool needExtendedHoursData, string periodType = null)
    {
        var breakPoint = new StringBuilder($"/pricehistory?symbol={symbol}");

        if (!string.IsNullOrEmpty(periodType))
        {
            breakPoint.Append($"&periodType={periodType}");
        }

        breakPoint.Append($"&frequencyType={frequencyType}&frequency={frequency}");
        breakPoint.Append("&startDate=" + Time.DateTimeToUnixTimeStampMilliseconds(startDateUtc.Truncate(TimeSpan.FromMilliseconds(1))));
        breakPoint.Append("&endDate=" + Time.DateTimeToUnixTimeStampMilliseconds(endDateUtc.Truncate(TimeSpan.FromMilliseconds(1))));
        breakPoint.Append("&needExtendedHoursData=" + needExtendedHoursData);

        return await RequestMarketDataAsync<CandleResponse>(HttpMethod.Get, breakPoint.ToString());
    }

    /// <summary>
    /// Retrieves a collection of all orders for the account.
    /// </summary>
    /// <returns>
    ///The read-only collection of <see cref="OrderResponse"/> objects.
    /// </returns>
    public async Task<IReadOnlyCollection<OrderResponse>> GetAllOrders()
    {
        var dateTimeUtcNow = DateTime.UtcNow;
        // Docs remark: Date must be within 60 days from today's date.
        var fromEnteredTime = dateTimeUtcNow.AddDays(-60).ToIso8601Invariant();
        var toEnteredTime = dateTimeUtcNow.ToIso8601Invariant();

        return await RequestTraderAsync<IReadOnlyCollection<OrderResponse>>(
            HttpMethod.Get,
            $"/accounts/{_accountHashNumber.Value}/orders?fromEnteredTime={fromEnteredTime}&toEnteredTime={toEnteredTime}");
    }

    /// <summary>
    /// Asynchronously cancels an order with the specified order ID for the associated account.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order to be canceled.</param>
    /// <returns>
    /// A <see cref="Task{Boolean}"/> indicating whether the cancellation request was sent successfully. 
    /// Returns <c>true</c> if the order was canceled successfully.
    /// </returns>
    public async Task<bool> CancelOrderById(string orderId)
    {
        var response = await RequestTraderAsync<string>(HttpMethod.Delete, $"/accounts/{_accountHashNumber.Value}/orders/{orderId}");
        return string.IsNullOrEmpty(response);
    }

    /// <summary>
    /// Submits a new order for the account linked to this instance, using the details provided in the <paramref name="orderRequest"/>.
    /// </summary>
    /// <param name="orderRequest">
    /// An instance of <see cref="OrderBaseRequest"/> containing the order details, such as the symbol, quantity, price, and order type.
    /// </param>
    /// <returns>
    /// On success, returns a string representing the unique order ID of the newly placed order.
    /// If the order placement fails, returns an empty string.
    /// </returns>
    /// <remarks>
    /// This method extracts the new order ID from the 'Location' header in the HTTP response. 
    /// Expected format of the Location header: "/trader/v1/accounts/{_accountHashNumber}/orders/{new_order_id}".
    /// </remarks>
    public async Task<string> PlaceOrder(OrderBaseRequest orderRequest)
    {
        var httpResponseMessage = await SendRequestAsync(HttpMethod.Post, _traderBaseUrl, $"/accounts/{_accountHashNumber.Value}/orders",
            JsonConvert.SerializeObject(orderRequest, _orderRequestJsonSerializerSettings));
        return httpResponseMessage.Headers.Location.Segments.Last();
    }

    /// <summary>
    /// Replace an existing order for an account. The existing order will be replaced by the new order.
    /// Once replaced, the old order will be canceled and a new order will be created.
    /// </summary>
    /// <param name="orderId">The ID of the order being retrieved.</param>
    /// <param name="orderRequest">An instance of <see cref="OrderBaseRequest"/> containing the order details, such as the symbol, quantity, price, and order type.</param>
    /// <returns></returns>
    public async Task<string> UpdateOrder(string orderId, OrderBaseRequest orderRequest)
    {
        var httpResponseMessage = await SendRequestAsync(HttpMethod.Put, _traderBaseUrl, $"/accounts/{_accountHashNumber.Value}/orders/{orderId}",
            JsonConvert.SerializeObject(orderRequest, _orderRequestJsonSerializerSettings));
        return httpResponseMessage.Headers.Location.Segments.Last();
    }

    /// <summary>
    /// Retrieves the user preferences from the API.
    /// </summary>
    /// <returns>
    /// The task result contains the user preferences.
    /// </returns>
    public async Task<UserPreference> GetUserPreference()
    {
        return await RequestTraderAsync<UserPreference>(HttpMethod.Get, "/userPreference");
    }

    /// <summary>
    /// Retrieves the option chain data for the specified symbol and option type (call or put).
    /// </summary>
    /// <param name="symbol">The symbol for which the option chain is requested.</param>
    /// <param name="optionRight">
    /// An <see cref="OptionRight"/> enumeration value indicating whether to fetch call or put options.
    /// </param>
    /// <returns>
    /// An <see cref="OptionChainResponse"/> object containing the option chain data for the specified symbol and contract type.
    /// </returns>
    public async Task<OptionChainResponse> GetOptionChainBySymbolAndOptionRight(string symbol, OptionRight optionRight)
    {
        var contractType = optionRight.ToUpperStringInvariant();
        return await RequestMarketDataAsync<OptionChainResponse>(HttpMethod.Get, $"/chains?symbol={symbol}&contractType={contractType}&includeUnderlyingQuote=false");
    }

    /// <summary>
    /// Retrieves the balance and positions of the securities account.
    /// </summary>
    /// <returns>A <see cref="SecuritiesAccount"/> object that represents the account balance and positions.</returns>
    public async Task<SecuritiesAccount> GetAccountBalanceAndPosition()
    {
        return (await GetAccountByNumber(_accountHashNumber.Value)).SecuritiesAccount;
    }

    /// <summary>
    /// Retrieves a meta data account by account number.
    /// </summary>
    /// <param name="accountHashNumber">The account hash number to retrieve information for.</param>
    /// <returns>A <see cref="SecuritiesAccountResponse"/> containing the account's metadata and positions.</returns>
    private async Task<SecuritiesAccountResponse> GetAccountByNumber(string accountHashNumber)
    {
        return await RequestTraderAsync<SecuritiesAccountResponse>(HttpMethod.Get, $"/accounts/{accountHashNumber}?fields=positions");
    }

    /// <summary>
    /// Retrieves a read-only collection of account numbers and their hash values from the API.
    /// </summary>
    /// <returns>
    /// A read-only collection of <see cref="AccountNumberResponse"/> objects, each containing an account number and its hash value.
    /// </returns>
    private async Task<IReadOnlyCollection<AccountNumberResponse>> GetAccountNumbers()
    {
        return await RequestTraderAsync<IReadOnlyCollection<AccountNumberResponse>>(HttpMethod.Get, "/accounts/accountNumbers");
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
    /// <param name="jsonBody">The JSON body of the request.</param>
    /// <returns>An object of type <typeparamref name="T"/> containing the deserialized response data.</returns>
    /// <exception cref="ArgumentException">Thrown when the API returns an unsuccessful status code along with error details.</exception>
    /// <exception cref="Exception">Thrown when there is an error during the HTTP request or response handling.</exception>
    private async Task<T> RequestTraderAsync<T>(HttpMethod httpMethod, string endpoint, string jsonBody = null)
    {
        return await RequestAsync<T>(httpMethod, _traderBaseUrl, endpoint, jsonBody);
    }

    /// <summary>
    /// Sends an HTTP request to a specified API endpoint and deserializes the response into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type into which the response content will be deserialized.</typeparam>
    /// <param name="httpMethod">The HTTP method to use, such as GET or POST.</param>
    /// <param name="baseUrl">The base URL of the API.</param>
    /// <param name="endpoint">The endpoint path, including any query parameters.</param>
    /// <param name="jsonBody">An optional JSON payload to include in the request body, relevant for methods like POST or PUT.</param>
    /// <returns>Containing the deserialized response as an object of type <typeparamref name="T"/>.</returns>
    private async Task<T> RequestAsync<T>(HttpMethod httpMethod, string baseUrl, string endpoint, string jsonBody = null)
    {
        var responseMessage = await SendRequestAsync(httpMethod, baseUrl, endpoint, jsonBody);
        var jsonContent = await responseMessage.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(jsonContent);
    }

    /// <summary>
    /// Sends an HTTP request to a specified API endpoint and retrieves the raw HTTP response.
    /// </summary>
    /// <param name="httpMethod">The HTTP method to use for the request, such as GET or POST.</param>
    /// <param name="baseUrl">The base URL of the API.</param>
    /// <param name="endpoint">The endpoint path, including any query parameters.</param>
    /// <param name="jsonBody">An optional JSON payload to include in the request body, applicable for methods like POST or PUT.</param>
    /// <returns>
    ///Returning an <see cref="HttpResponseMessage"/> that contains the raw response from the server.
    /// </returns>
    private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod httpMethod, string baseUrl, string endpoint, string jsonBody = null)
    {
        using (var requestMessage = new HttpRequestMessage(httpMethod, baseUrl + endpoint))
        {
            if (jsonBody != null)
            {
                requestMessage.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }

            try
            {
                var responseMessage = await _httpClient.SendAsync(requestMessage);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    var errorMessage = new StringBuilder();
                    foreach (var contentEncoding in responseMessage.Content.Headers.ContentEncoding)
                    {
                        switch (contentEncoding)
                        {
                            case "json":
                                errorMessage.Append(await GetErrorMessageByJsonContent(responseMessage));
                                break;
                            case "gzip":
                                var jsonContent = default(string);
                                using (var responseStream = await responseMessage.Content.ReadAsStreamAsync())
                                {
                                    using (var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
                                    {
                                        using (var reader = new StreamReader(decompressedStream))
                                        {
                                            jsonContent = await reader.ReadToEndAsync();
                                        }
                                    }
                                }

                                foreach (var error in JsonConvert.DeserializeObject<ErrorsResponse>(jsonContent).Errors)
                                {
                                    errorMessage.Append(error.ToString());
                                }
                                break;
                        }
                    }

                    if (errorMessage.Length == 0 && responseMessage.Content.Headers.ContentType.MediaType.Contains("application/json"))
                    {
                        errorMessage.Append(await GetErrorMessageByJsonContent(responseMessage));
                    }

                    errorMessage.AppendLine($"HttpMethod: {httpMethod}, RequestUri: {baseUrl + endpoint}" + (string.IsNullOrEmpty(jsonBody) ? "" : $", RequestBody: {jsonBody}"));
                    throw new ArgumentException(errorMessage.ToString());
                }

                return responseMessage;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(CharlesSchwabApiClient)}.{nameof(SendRequestAsync)}: Unexpected error while sending request - {ex.Message}", ex);
            }
        }
    }

    private static async Task<string> GetErrorMessageByJsonContent(HttpResponseMessage responseMessage)
    {
        var jsonContent = await responseMessage.Content.ReadAsStringAsync();
        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(jsonContent);
        return $"{errorResponse.Error} - {string.Join('\n', errorResponse.ErrorDescription ?? new List<string> { "No error description available" })}.";
    }
}
