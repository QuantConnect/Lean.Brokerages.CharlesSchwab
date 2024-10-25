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
using System.Net;
using Moq.Protected;
using System.Net.Http;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using QuantConnect.Configuration;
using QuantConnect.Brokerages.CharlesSchwab.Api;

namespace QuantConnect.Brokerages.CharlesSchwab.Tests.Api;

[TestFixture]
public class CharlesSchwabApiClientTests
{
  private string _accountNumber = Config.Get("charles-schwab-account-number", "123");
  private CharlesSchwabApiClient _apiClient;

  [OneTimeSetUp]
  public void OneTimeSetUp()
  {
    var baseUrl = Config.Get("charles-schwab-api-url");
    var appKey = Config.Get("charles-schwab-app-key");
    var secret = Config.Get("charles-schwab-secret");
    var redirectUrl = Config.Get("charles-schwab-redirect-url");

    var authorizationCode = Config.Get("charles-schwab-authorization-code-from-url");

    var mockHandler = GetMockHttpMessageHandler();

    _apiClient = new CharlesSchwabApiClient(baseUrl, appKey, secret, _accountNumber, redirectUrl, authorizationCode, string.Empty, mockHandler.Object);
  }

  [Test]
  public async Task GetAccounts()
  {
    var result = await _apiClient.GetAccountBalanceAndPosition();
    Assert.IsNotNull(result);
  }

  [Test]
  public async Task GetOpenOrders()
  {
    var result = await _apiClient.GetOpenOrders();
    Assert.IsNotNull(result);
  }

  private Mock<HttpClientHandler> GetMockHttpMessageHandler()
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
          var requestUriAbsolutePath = request.RequestUri.AbsolutePath;
          if (requestUriAbsolutePath.StartsWith($"/trader/v1/accounts/{_accountNumber}?fields=positions"))
          {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
              Content = new StringContent(GetAccountJsonResponse())
            };
          }
          else if (requestUriAbsolutePath.StartsWith($"/trader/v1/accounts/{_accountNumber}/orders"))
          {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
              Content = new StringContent(GetOpenOrderJsonResponse())
            };
          }
          return new HttpResponseMessage(HttpStatusCode.BadRequest);
        });

    return mockHandler;
  }

  private string GetAccountJsonResponse()
  {
    return $@"
            {{
            ""securitiesAccount"": {{
            ""type"": ""CASH"",
            ""accountNumber"": ""{_accountNumber}"",
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

    private string GetOpenOrderJsonResponse()
    {
        return $@"
    [ {{
        ""session"": ""NORMAL"",
        ""duration"": ""DAY"",
        ""orderType"": ""MARKET"",
        ""cancelTime"": ""2024-10-24T16:41:34.564Z"",
        ""complexOrderStrategyType"": ""NONE"",
        ""quantity"": 1,
        ""filledQuantity"": 2,
        ""remainingQuantity"": 3,
        ""requestedDestination"": ""INET"",
        ""destinationLinkName"": ""somewhere"",
        ""releaseTime"": ""2024-10-24T16:41:34.564Z"",
        ""stopPrice"": 223.65,
        ""stopPriceLinkBasis"": ""MANUAL"",
        ""stopPriceLinkType"": ""VALUE"",
        ""stopPriceOffset"": 0.22,
        ""stopType"": ""STANDARD"",
        ""priceLinkBasis"": ""MANUAL"",
        ""priceLinkType"": ""VALUE"",
        ""price"": 220.11,
        ""taxLotMethod"": ""FIFO"",
        ""orderLegCollection"": [ {{
            ""orderLegType"": ""EQUITY"",
            ""legId"": 1,
            ""instrument"": {{
                ""cusip"": ""777"",
                ""symbol"": ""ABC"",
                ""description"": ""Alphabet"",
                ""instrumentId"": 1,
                ""netChange"": 43.2,
                ""type"": ""SWEEP_VEHICLE""
            }},
            ""instruction"": ""BUY"",
            ""positionEffect"": ""OPENING"",
            ""quantity"": 1,
            ""quantityType"": ""ALL_SHARES"",
            ""divCapGains"": ""REINVEST"",
            ""toSymbol"": ""ABC""
        }}],
        ""activationPrice"": 200,
        ""specialInstruction"": ""ALL_OR_NONE"",
        ""orderStrategyType"": ""SINGLE"",
        ""orderId"": 123,
        ""cancelable"": true,
        ""editable"": true,
        ""status"": ""WORKING"",
        ""enteredTime"": ""2024-10-24T16:41:34.564Z"",
        ""closeTime"": ""2024-10-24T16:41:34.564Z"",
        ""tag"": ""MockOrder"",
        ""accountNumber"": ""{_accountNumber}"",
        ""orderActivityCollection"": [ {{
            ""activityType"": ""EXECUTION"",
            ""executionType"": ""FILL"",
            ""quantity"": 3,
            ""orderRemainingQuantity"": 2,
            ""executionLegs"": [ {{
                ""legId"": 1,
                ""price"": 22.22,
                ""quantity"": 1,
                ""mismarkedQuantity"": 3,
                ""instrumentId"": 777,
                ""time"": ""2024-10-24T16:41:34.564Z""
            }} ]
        }} ],
        ""replacingOrderCollection"": [ ""string"" ],
        ""childOrderStrategies"": [ ""string"" ],
        ""statusDescription"": ""string""
    }}]";
    }
}
