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

public class CharlesSchwabApiClient
{
    /// <summary>
    /// HttpClient is used for making HTTP requests and handling HTTP responses from web resources identified by a Uri.
    /// </summary>
    private readonly HttpClient _httpClient;

    private readonly string _accountNumber;

    private readonly string _traderBaseUrl;

    public CharlesSchwabApiClient(string baseUrl, string appKey, string secret, string accountNumber, string redirectUri, string authorizationCodeFromUrl, string refreshToken,
       HttpClientHandler httpClientHandler = null)
    {
        _traderBaseUrl = baseUrl + "/trader/v1";
        _accountNumber = accountNumber;

        var httpClient = httpClientHandler ?? new HttpClientHandler();
        var tokenRefreshHandler = new CharlesSchwabTokenRefreshHandler(httpClient, baseUrl, appKey, secret, redirectUri, authorizationCodeFromUrl, refreshToken);
        _httpClient = new(tokenRefreshHandler);
    }

    public async Task<CharlesSchwabSecuritiesAccount> GetAccountByNumber(string accountNumber)
    {
        return await RequestAsync<CharlesSchwabSecuritiesAccount>(HttpMethod.Get, _traderBaseUrl, $"/accounts/{accountNumber}?fields=positions");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="baseUrl"></param>
    /// <param name="endpoint">This name encapsulates both the path and query parameters</param>
    /// <returns></returns>
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
