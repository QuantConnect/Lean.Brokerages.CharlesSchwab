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
using System.Net;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
using QuantConnect.Brokerages.CharlesSchwab.Models;

namespace QuantConnect.Brokerages.CharlesSchwab.Api;

public class CharlesSchwabTokenRefreshHandler : DelegatingHandler
{
    /// <summary>
    /// Represents the number of retry attempts made for an authenticated request.
    /// </summary>
    private int _retryCount = 0;

    /// <summary>
    /// Represents the maximum number of retry attempts for an authenticated request.
    /// </summary>
    private int _maxRetryCount = 3;

    /// <summary>
    /// Represents the time interval between retry attempts for an authenticated request.
    /// </summary>
    private TimeSpan _retryInterval = TimeSpan.FromSeconds(2);

    /// <summary>
    /// The base URL used for constructing API endpoints.
    /// </summary>
    private readonly string _baseOauthUrl;

    /// <summary>
    /// Represents the authorization code obtained from the URL during OAuth authentication.
    /// </summary>
    private readonly string _authorizationCodeFromUrl;

    /// <summary>
    /// Represents the URI to which the user will be redirected after authentication.
    /// </summary>
    private readonly string _redirectUri;

    /// <summary>
    /// Represents the refresh token used to obtain a new access token when the current one expires.
    /// </summary>
    private string _refreshToken;

    /// <summary>
    /// The client ID for the OAuth authorization.
    /// </summary>
    private readonly string _clientId;

    /// <summary>
    /// Encoded client credentials for authentication, combining the client ID and client secret in a base64 format.
    /// </summary>
    private readonly string _encodedClientCredentials;

    /// <summary>
    /// Represents an object storing AccessToken and information for Charles Schwab authentication.
    /// </summary>
    private CharlesSchwabAccessToken _charlesSchwabAccessToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharlesSchwabTokenRefreshHandler"/> class.
    /// This handler manages Charles Schwab OAuth token acquisition and refresh processes.
    /// </summary>
    /// <param name="innerHandler">The inner HTTP message handler responsible for sending HTTP requests.</param>
    /// <param name="baseUrl">The base URL for the Charles Schwab API.</param>
    /// <param name="clientId">The client ID for the OAuth authorization.</param>
    /// <param name="clientSecret">The client secret for the OAuth authorization.</param>
    /// <param name="redirectUri">The redirect URI that matches the one registered with the Charles Schwab API.</param>
    /// <param name="authorizationCodeFromUrl">The authorization code obtained from the URL during the OAuth flow.</param>
    /// <param name="refreshToken">The refresh token used to obtain a new access token when the current one expires.</param>
    public CharlesSchwabTokenRefreshHandler(
        HttpMessageHandler innerHandler,
        string baseUrl,
        string clientId,
        string clientSecret,
        string redirectUri,
        string authorizationCodeFromUrl,
        string refreshToken) : base(innerHandler)
    {
        _clientId = clientId;
        _redirectUri = redirectUri;
        _refreshToken = refreshToken;
        _baseOauthUrl = baseUrl + "/v1/oauth";
        _authorizationCodeFromUrl = authorizationCodeFromUrl;
        _encodedClientCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
    }

    /// <summary>
    /// Sends an HTTP request with automatic retries and token refresh on authorization failure.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The HTTP response message.</returns>
    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = null;

        for (_retryCount = 0; _retryCount < _maxRetryCount; _retryCount++)
        {
            if (_charlesSchwabAccessToken != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(_charlesSchwabAccessToken.TokenType, _charlesSchwabAccessToken.AccessToken);
            }

            response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                break;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (_charlesSchwabAccessToken == null && string.IsNullOrEmpty(_refreshToken))
                {
                    _charlesSchwabAccessToken = await GetAccessToken(cancellationToken);
                    _refreshToken = _charlesSchwabAccessToken.RefreshToken;
                }
                else
                {
                    _charlesSchwabAccessToken = await RefreshAccessToken(_refreshToken, cancellationToken);
                }
            }
            else
            {
                break;
            }

            // Wait for retry interval or cancellation request
            if (cancellationToken.WaitHandle.WaitOne(_retryInterval))
            {
                break;
            }
        }

        return response;
    }

    /// <summary>
    /// Generates the authorization URL for initiating the OAuth 2.0 authorization process.
    /// </summary>
    /// <returns>A string representing the full authorization URL to be used for OAuth 2.0 authorization.</returns>
    public string GetAuthorizationUrl()
    {
        return _baseOauthUrl + $"/authorize?client_id={_clientId}&redirect_uri={_redirectUri}";
    }

    /// <summary>
    /// Refreshes the access token using an expired refresh token.
    /// </summary>
    /// <param name="expiredRefreshToken">The expired refresh token to be exchanged for a new one.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The task result contains the refreshed access token.</returns>
    private async Task<CharlesSchwabAccessToken> RefreshAccessToken(string expiredRefreshToken, CancellationToken cancellationToken)
    {
        var payload = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", expiredRefreshToken }
        };

        return await SendSignInAsync<CharlesSchwabAccessToken>(payload, cancellationToken);
    }

    /// <summary>
    /// Obtains a new access token using the authorization code.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The task result contains the access token.</returns>
    private async Task<CharlesSchwabAccessToken> GetAccessToken(CancellationToken cancellationToken)
    {
        var payload = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", _authorizationCodeFromUrl },
            { "redirect_uri", _redirectUri }
        };

        return await SendSignInAsync<CharlesSchwabAccessToken>(payload, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP POST request to the Charles Schwab OAuth token endpoint with the specified payload.
    /// </summary>
    /// <typeparam name="T">The type representing the expected deserialized JSON response.</typeparam>
    /// <param name="payload">A dictionary containing the parameters required for the OAuth token request.</param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the deserialized JSON response of type <typeparamref name="T"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when an error occurs during the OAuth request, providing details about the error.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if any other exception occurs while processing the response.
    /// </exception>
    private async Task<T> SendSignInAsync<T>(Dictionary<string, string> payload, CancellationToken cancellationToken) where T : class
    {
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, _baseOauthUrl + "/token") { Content = new FormUrlEncodedContent(payload) })
        {
            requestMessage.Headers.Add("Authorization", $"Basic {_encodedClientCredentials}");
            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var content = new FormUrlEncodedContent(payload);

            var response = await base.SendAsync(requestMessage, cancellationToken);

            var jsonContent = await response.Content.ReadAsStringAsync();

            try
            {
                return JsonConvert.DeserializeObject<T>(jsonContent);
            }
            catch (JsonSerializationException)
            {
                var errorResponse = JsonConvert.DeserializeObject<CharlesSchwabErrorResponse>(jsonContent);
                throw new ArgumentException($"{nameof(CharlesSchwabTokenRefreshHandler)}.{nameof(SendSignInAsync)}: {errorResponse?.Error ?? "Unknown error"} - " +
                               $"{errorResponse?.ErrorDescription ?? "No description"}.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"{nameof(CharlesSchwabTokenRefreshHandler)}.{nameof(SendSignInAsync)}: {ex.Message}");
            }
        }
    }
}
