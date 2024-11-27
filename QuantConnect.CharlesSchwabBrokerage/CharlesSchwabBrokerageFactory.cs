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
using System.Linq;
using QuantConnect.Util;
using QuantConnect.Packets;
using QuantConnect.Interfaces;
using QuantConnect.Securities;
using System.Collections.Generic;
using QuantConnect.Configuration;

namespace QuantConnect.Brokerages.CharlesSchwab
{
    /// <summary>
    /// Provides a CharlesSchwab implementation of BrokerageFactory
    /// </summary>
    public class CharlesSchwabBrokerageFactory : BrokerageFactory
    {
        /// <summary>
        /// Gets the brokerage data required to run the brokerage from configuration/disk
        /// </summary>
        /// <remarks>
        /// The implementation of this property will create the brokerage data dictionary required for
        /// running live jobs. See <see cref="IJobQueueHandler.NextJob"/>
        /// </remarks>
        public override Dictionary<string, string> BrokerageData => new()
        {
            // The URL to connect to brokerage environment:
            { "charles-schwab-api-url", Config.Get("charles-schwab-api-url", "https://api.schwabapi.com")},
            { "charles-schwab-app-key", Config.Get("charles-schwab-app-key") },
            { "charles-schwab-secret", Config.Get("charles-schwab-secret") },
            // Users can have multiple different accounts
            { "charles-schwab-account-number", Config.Get("charles-schwab-account-number") },

            // USE CASE 1 (normal): lean CLI & live clous wizard
            {  "charles-schwab-refresh-token", Config.Get("charles-schwab-refresh-token") },

            // USE CASE 2 (developing): Only if refresh token is not provided
            { "charles-schwab-authorization-code-from-url", Config.Get("charles-schwab-authorization-code-from-url") },
            { "charles-schwab-redirect-url", Config.Get("charles-schwab-redirect-url") },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="CharlesSchwabBrokerageFactory"/> class
        /// </summary>
        public CharlesSchwabBrokerageFactory() : base(typeof(CharlesSchwabBrokerage))
        {
        }

        /// <summary>
        /// Gets a brokerage model that can be used to model this brokerage's unique behaviors
        /// </summary>
        /// <param name="orderProvider">The order provider</param>
        public override IBrokerageModel GetBrokerageModel(IOrderProvider orderProvider) => new CharlesSchwabBrokerageModel();

        /// <summary>
        /// Creates a new IBrokerage instance
        /// </summary>
        /// <param name="job">The job packet to create the brokerage for</param>
        /// <param name="algorithm">The algorithm instance</param>
        /// <returns>A new brokerage instance</returns>
        public override IBrokerage CreateBrokerage(LiveNodePacket job, IAlgorithm algorithm)
        {
            var errors = new List<string>();

            var baseUrl = Read<string>(job.BrokerageData, "charles-schwab-api-url", errors);
            var appKey = Read<string>(job.BrokerageData, "charles-schwab-app-key", errors);
            var secret = Read<string>(job.BrokerageData, "charles-schwab-secret", errors);
            var accountNumber = Read<string>(job.BrokerageData, "charles-schwab-account-number", errors);

            if (errors.Count != 0)
            {
                // if we had errors then we can't create the instance
                throw new ArgumentException(string.Join(Environment.NewLine, errors));
            }

            var cs = default(CharlesSchwabBrokerage);

            var refreshToken = Read<string>(job.BrokerageData, "charles-schwab-refresh-token", errors);
            if (string.IsNullOrEmpty(refreshToken))
            {
                var redirectUrl = Read<string>(job.BrokerageData, "charles-schwab-redirect-url", errors);
                var authorizationCode = Read<string>(job.BrokerageData, "charles-schwab-authorization-code-from-url", errors);

                if (new string[] { redirectUrl, authorizationCode }.Any(string.IsNullOrEmpty))
                {
                    throw new ArgumentException("RedirectUrl or AuthorizationCode cannot be empty or null. Please ensure these values are correctly set in the configuration file.");
                }

                // Case 1: authentication with using redirectUrl, authorizationCode
                cs = new CharlesSchwabBrokerage(baseUrl, appKey, secret, accountNumber, redirectUrl, authorizationCode, refreshToken: string.Empty, algorithm);
            }
            else
            {
                cs = new CharlesSchwabBrokerage(baseUrl, appKey, secret, accountNumber, redirectUrl: string.Empty, authorizationCodeFromUrl: string.Empty, refreshToken, algorithm);
            }

            Composer.Instance.AddPart<IDataQueueHandler>(cs);

            return cs;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            // Not Needed
        }
    }
}