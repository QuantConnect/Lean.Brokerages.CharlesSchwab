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
using NUnit.Framework;
using QuantConnect.Util;
using QuantConnect.Packets;
using QuantConnect.Interfaces;

namespace QuantConnect.Brokerages.CharlesSchwab.Tests;

[TestFixture]
public class CharlesSchwabBrokerageFactoryTests
{
    [TestCase("", "http://localhost", "123", false, "error\":\"invalid_request\"}\".")]
    [TestCase("123", "", "", false, "invalid_client - refresh token invalid.")]
    [TestCase("123", "http://localhost", "123", false, "invalid_client - refresh token invalid.")]
    [TestCase("", "http://localhost", "", true, "")]
    [TestCase("", "", "123", true, "")]
    [TestCase("", "", "", true, "")]
    public void InitializesFactoryFromComposer(string refreshToken, string redirectUrl, string authorizationCode, bool shouldThrowArgumentException, string exceptionMessage)
    {
        using var factory = Composer.Instance.Single<IBrokerageFactory>(instance => instance.BrokerageType == typeof(CharlesSchwabBrokerage));

        var newBrokerageData = factory.BrokerageData;

        newBrokerageData["charles-schwab-refresh-token"] = refreshToken;
        newBrokerageData["charles-schwab-redirect-url"] = redirectUrl;
        newBrokerageData["charles-schwab-authorization-code-from-url"] = authorizationCode;
        newBrokerageData["charles-schwab-account-number"] = string.Empty;

        var liveNodePacket = new LiveNodePacket() { BrokerageData = newBrokerageData };

        if (shouldThrowArgumentException)
        {
            Assert.Throws<ArgumentException>(() => factory.CreateBrokerage(liveNodePacket, new Mock<IAlgorithm>().Object));
        }
        else
        {
            var exception = Assert.Throws<Exception>(() => factory.CreateBrokerage(liveNodePacket, new Mock<IAlgorithm>().Object));
            Assert.True(exception.Message.EndsWith(exceptionMessage));
        }
    }
}