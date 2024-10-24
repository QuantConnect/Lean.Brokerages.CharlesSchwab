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
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using QuantConnect.Brokerages.CharlesSchwab.Models;

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
        _accountNumber = accountNumber;

        var httpClient = httpClientHandler ?? new HttpClientHandler();
        var tokenRefreshHandler = new CharlesSchwabTokenRefreshHandler(httpClient, baseUrl, appKey, secret, redirectUri, authorizationCodeFromUrl, refreshToken);
        _httpClient = new(tokenRefreshHandler);
    }

    /// <summary>
    /// Retrieves the balance and positions of the securities account.
    /// </summary>
    /// <returns>A <see cref="SecuritiesAccountMetaData"/> object that represents the account balance and positions.</returns>
    public async Task<SecuritiesAccountMetaData> GetAccountBalanceAndPosition()
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
        return await RequestAsync<CharlesSchwabSecuritiesAccount>(HttpMethod.Get, _traderBaseUrl, $"/accounts/{accountNumber}?fields=positions");
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
                    var errorResponse = JsonConvert.DeserializeObject<CharlesSchwabErrorResponse>(jsonContent);
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
