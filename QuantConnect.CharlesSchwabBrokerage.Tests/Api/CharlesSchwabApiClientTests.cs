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

using NUnit.Framework;
using System.Threading.Tasks;
using QuantConnect.Configuration;
using QuantConnect.Brokerages.CharlesSchwab.Api;
using Moq.Protected;
using Moq;
using System.Net.Http;
using System.Threading;
using System;
using System.Net;
using System.Linq;

namespace QuantConnect.Brokerages.CharlesSchwab.Tests.Api;

[TestFixture]
public class CharlesSchwabApiClientTests
{
    private string _baseUrl = Config.Get("charles-schwab-api-url");
    private string _appKey = Config.Get("charles-schwab-app-key");
    private string _secret = Config.Get("charles-schwab-secret");
    private string _redirectUrl = Config.Get("charles-schwab-redirect-url");
    private string _accountNumber = Config.Get("charles-schwab-account-number");

    [Test]
    public async Task GetAccounts()
    {
        var authorizationCode = Config.Get("charles-schwab-authorization-code-from-url");

        var mockHandler = GetMockHttpMessageHandler("123");

        var apiClient = new CharlesSchwabApiClient(_baseUrl, _appKey, _secret, _accountNumber, _redirectUrl, authorizationCode, string.Empty, mockHandler.Object);

        var result = await apiClient.GetAccountByNumber("123");

        Assert.IsNotNull(result);
    }

    private Mock<HttpClientHandler> GetMockHttpMessageHandler(string accessToken)
    {
        var mockHandler = new Mock<HttpClientHandler>(MockBehavior.Strict);

        mockHandler.Protected().Setup("Dispose", ItExpr.IsAny<bool>());

        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken cancellationToken) =>
            {
                if (request.RequestUri.AbsolutePath.StartsWith("/trader/v1/accounts/"))
                {
                    var accountNumber = request.RequestUri.AbsolutePath.Split('/').Last();
                    var jsonResponse = GetAccountJsonResponse(accountNumber);
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(jsonResponse)
                    };
                }

                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            });

        return mockHandler;
    }

    private string GetAccountJsonResponse(string accountNumber)
    {
        return $@"
            {{
            ""securitiesAccount"": {{
            ""type"": ""CASH"",
            ""accountNumber"": ""{accountNumber}"",
            ""roundTrips"": 0,
            ""isDayTrader"": false,
            ""isClosingOnlyRestricted"": false,
            ""pfcbFlag"": false,
            ""positions"": [
            {{
              ""shortQuantity"": 1,
              ""averagePrice"": 300.2,
              ""currentDayProfitLoss"": 12.2,
              ""currentDayProfitLossPercentage"": 35,
              ""longQuantity"": 1,
              ""settledLongQuantity"": 1,
              ""settledShortQuantity"": 1,
              ""agedQuantity"": 1,
              ""instrument"":
              {{
                ""cusip"": ""777"",
                ""symbol"": ""ABC"",
                ""description"": ""Alphabet"",
                ""instrumentId"": 1,
                ""netChange"": 12.2,
                ""type"": ""SWEEP_VEHICLE""
              }},
              ""marketValue"": 100,
              ""maintenanceRequirement"": 1,
              ""averageLongPrice"": 300.33,
              ""averageShortPrice"": 222.22,
              ""taxLotAverageLongPrice"": 11,
              ""taxLotAverageShortPrice"": 23,
              ""longOpenProfitLoss"": 22,
              ""shortOpenProfitLoss"": 32,
              ""previousSessionLongQuantity"": 2,
              ""previousSessionShortQuantity"": 5,
              ""currentDayCost"": 777.77
            }}]}}}}";
    }
}
