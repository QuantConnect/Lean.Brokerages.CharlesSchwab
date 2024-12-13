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

using Newtonsoft.Json;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using QuantConnect.Brokerages.CharlesSchwab.Models;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums;
using QuantConnect.Brokerages.CharlesSchwab.Models.Stream;
using QuantConnect.Brokerages.CharlesSchwab.Models.Requests;
using QuantConnect.Brokerages.CharlesSchwab.Models.Enums.Stream;

namespace QuantConnect.Brokerages.CharlesSchwab.Tests.Api;

[TestFixture]
public class CharlesSchwabApiResponseJsonConverterTests
{

    [Test]
    public void DeserializeGetPriceHistoryResponse()
    {
        var jsonResponse = @"
{
""candles"": [
    {
      ""open"": 228.4,
      ""high"": 228.49,
      ""low"": 228.4,
      ""close"": 228.48,
      ""volume"": 3957,
      ""datetime"": 1729854000000
    },
    {
      ""open"": 228.46,
      ""high"": 228.46,
      ""low"": 228.13,
      ""close"": 228.13,
      ""volume"": 6723,
      ""datetime"": 1729854060000
    },
],
""symbol"": ""AAPL"",
""empty"": false
}";

        var candles = JsonConvert.DeserializeObject<CandleResponse>(jsonResponse);

        Assert.IsNotNull(candles);
    }

    [Test]
    public void DeserializeAccountNumbersResponse()
    {
        var jsonResponse = @"
[
  {
    ""accountNumber"": ""123"",
    ""hashValue"": ""zxV23""
  },
  {
    ""accountNumber"": ""456"",
    ""hashValue"": ""maspod2""
  },
]";
        var accountNumbers = JsonConvert.DeserializeObject<IReadOnlyCollection<AccountNumberResponse>>(jsonResponse);

        Assert.IsNotNull(accountNumbers);
        Assert.Greater(accountNumbers.Count, 0);
    }

    [TestCase(OrderType.Market, "AAPL", Instruction.Buy)]
    [TestCase(OrderType.Limit, "AAPL", Instruction.Sell)]
    [TestCase(OrderType.Stop, "AAPL", Instruction.Sell)]
    public void SerializePlaceOrderRequest(OrderType orderType, string symbol, Instruction instruction)
    {
        var orderRequest = default(OrderBaseRequest);
        switch (orderType)
        {
            case OrderType.Market:
                orderRequest = new MarketOrderRequest(new List<OrderLegRequest> { new(instruction, 1m, new InstrumentRequest(symbol, AssetType.Equity)) });
                break;
            case OrderType.Limit:
                orderRequest = new LimitOrderRequest(SessionType.Normal, Duration.GoodTillCancel,
                    new List<OrderLegRequest> { new(instruction, 1m, new InstrumentRequest(symbol, AssetType.Equity)) }, 22.222m);
                break;
            case OrderType.Stop:
                orderRequest = new StopMarketOrderRequest(Duration.GoodTillCancel, new List<OrderLegRequest> { new(instruction, 1m, new InstrumentRequest(symbol, AssetType.Equity)) }, 22.222m);
                break;
        }

        var json = JsonConvert.SerializeObject(orderRequest);

        Assert.IsNotNull(json);
        Assert.IsNotEmpty(json);
    }

    [TestCase(1, "Someone", "2be0b7e7-5b8b-4fd3-9bed-7f49106cfe1", "PN", "IO", "Tradeticket")]
    public void LoginRequestShouldEqualAsDocs(int requestId, string schwabClientCustomerId, string schwabClientCorrelId, string accessToken, string schwabClientChannel, string schwabClientFunctionId)
    {
        var adminLoginRequest = new AdminStreamRequest(
            requestId,
            Command.Login,
            schwabClientCustomerId,
            schwabClientCorrelId,
            accessToken,
            schwabClientChannel,
            schwabClientFunctionId);

        var userJsonMessage = JsonConvert.SerializeObject(adminLoginRequest);

        var docsAdmingLoginExample = @"{
    ""service"": ""ADMIN"",
    ""requestid"": ""1"",
    ""command"": ""LOGIN"",
    ""SchwabClientCustomerId"": ""Someone"",
    ""SchwabClientCorrelId"": ""2be0b7e7-5b8b-4fd3-9bed-7f49106cfe1"",
    ""parameters"": {
     ""Authorization"": ""PN"",
     ""SchwabClientChannel"": ""IO"",
     ""SchwabClientFunctionId"": ""Tradeticket""
    }
   }";

        var expected = JToken.Parse(docsAdmingLoginExample);
        var actual = JToken.Parse(userJsonMessage);

        Assert.AreEqual(expected, actual);
    }
}
