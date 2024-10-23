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

using Moq;
using System;
using System.Net;
using Moq.Protected;
using NUnit.Framework;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using QuantConnect.Configuration;
using QuantConnect.Brokerages.CharlesSchwab.Api;

namespace QuantConnect.Brokerages.CharlesSchwab.Tests.Api;

[TestFixture]
public class CharlesSchwabTokenRefreshHandlerTests
{
    private string _baseUrl = Config.Get("charles-schwab-api-url");
    private string _redirectUrl = Config.Get("charles-schwab-redirect-url");

    [Test]
    public void GetAuthorizationUrl()
    {
        var clientId = Config.Get("charles-schwab-client-id");

        var tokenRefreshHandler = new CharlesSchwabTokenRefreshHandler(new HttpClientHandler(), _baseUrl, clientId, string.Empty, _redirectUrl, string.Empty, string.Empty);

        var authorizationUrl = tokenRefreshHandler.GetAuthorizationUrl();

        Assert.IsNotNull(authorizationUrl);
        Assert.IsNotEmpty(authorizationUrl);

        Assert.Pass($"Charles Schwab, Authorization URL: {authorizationUrl}");
    }

    [TestCase("NOT_EXPIRED_ACCESS_TOKEN_HERE")]
    public async Task GetAccessToken(string accessToken)
    {
        await SendAuthenticationRequestAndAssertSuccess(accessToken, string.Empty);
    }

    [TestCase("EXPIRED_ACCESS_TOKEN_HERE", "NOT_EXPIRED_ACCESS_TOKEN_HERE")]
    public async Task RefreshAccessToken(string accessToken, string newAccessToken)
    {
        await SendAuthenticationRequestAndAssertSuccess(accessToken, newAccessToken);
    }

    private async Task SendAuthenticationRequestAndAssertSuccess(string accessToken, string newAccessToken)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var mockHandler = GetMockHttpMessageHandler(accessToken, newAccessToken);

        var tokenRefreshHandler = new CharlesSchwabTokenRefreshHandler(
            mockHandler.Object,
            _baseUrl,
            "123",
            "confidential key",
            _redirectUrl,
            "secret-code-from-url",
            string.Empty
        );

        using var httpClient = new HttpClient(tokenRefreshHandler);

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/accounts");
        using var response = await httpClient.SendAsync(requestMessage, cancellationTokenSource.Token);

        Assert.IsTrue(response.IsSuccessStatusCode, "Request failed with status code: " + response.StatusCode);
    }

    private static Mock<HttpMessageHandler> GetMockHttpMessageHandler(string accessToken, string newAccessToken)
    {
        var mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        mockHandler.Protected().Setup("Dispose", ItExpr.IsAny<bool>());

        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken cancellationToken) =>
            {
                if (request.RequestUri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries)[0] == "accounts")
                {
                    switch (request.Headers.Authorization, accessToken)
                    {
                        case (null, _):
                            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        case (not null, "EXPIRED_ACCESS_TOKEN_HERE"):
                            accessToken = newAccessToken;
                            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        case (not null, "NOT_EXPIRED_ACCESS_TOKEN_HERE"):
                            return new HttpResponseMessage(HttpStatusCode.OK);
                        default:
                            throw new NotImplementedException($"{nameof(CharlesSchwabTokenRefreshHandlerTests)}.{nameof(GetAccessToken)}:" +
                                $"Unexpected combination of Authorization header: '{request.Headers.Authorization}' and access token: '{accessToken}'.");
                    }
                }

                var content = request.Content as FormUrlEncodedContent;

                var requestBody = content.ReadAsStringAsync().Result;

                string jsonResponse = $@"
                    {{
                    ""expires_in"": 1800,
                    ""token_type"": ""Bearer"",
                    ""scope"": ""api"",
                    ""refresh_token"": ""REFRESH_TOKEN_HERE"",
                    ""access_token"": ""{accessToken}"",
                    ""id_token"": ""JWT_HERE""
                    }}";

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(jsonResponse)
                };
            });

        return mockHandler;
    }
}