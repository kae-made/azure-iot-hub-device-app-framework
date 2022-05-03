// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.IoT.Framework.Authentication;
using Kae.Utility.Logging;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kae.IoT.Framework
{
    internal abstract class IoTClientBase
    {
        protected IoTAppConnector appConnector;
        protected IoTHubAuthentication authentication;
        protected Logger logger;

        protected ConnectionStatus connectionStatus;

        public ConnectionStatus ConnectionStatus
        {
            get { return connectionStatus; }
        }

        protected ConnectionStatusChangeReason connectionStatusReason;
        public ConnectionStatusChangeReason ConnectionStatusReason
        {
            get { return connectionStatusReason; }
        }

        protected D2CMessage d2cData;
       
        protected TimeSpan d2cSendInterval;
        protected CancellationTokenSource d2cCancellationTokeSource;

        protected IoTClient client;


        internal IoTClientBase(IoTAppConnector connector, Logger logger)
        {
            appConnector = connector;
            if (logger == null)
            {
                this.logger = ConsoleLogger.CreateLogger();
            }
            else
            {
                this.logger = logger;
            }
            d2cData = connector.GetAppD2CData();
         }

        public void ConnectionStatusChanged(ConnectionStatus status, ConnectionStatusChangeReason reason)
        {
            lock (this)
            {
                connectionStatus = status;
                connectionStatusReason = reason;
            }
        }

        protected async Task StartSendD2CMessagePeriodicallyAsync(string outputPort = null)
        {
            d2cCancellationTokeSource = new CancellationTokenSource();
            var task = Task.Run(async () =>
            {
                d2cCancellationTokeSource.Token.ThrowIfCancellationRequested();
                logger.LogInfo("start periodic sending...");

                while (true)
                {
                    D2CMessage current = null;
                    lock (d2cData)
                    {
                        current = d2cData;
                    }
                    await client.SendD2CMessageAsync(current, outputPort);

                    if (d2cCancellationTokeSource.IsCancellationRequested)
                    {
                        break;
                    }
                    await Task.Delay(d2cSendInterval);
                }
            });
            try
            {
                await task;
                logger.LogInfo("stop periodic sending...");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
            finally
            {
                d2cCancellationTokeSource.Dispose();
            }
        }

        protected void StopSendD2CMessagePeriodically()
        {
            logger.LogInfo("stopping periodic sending...");
            d2cCancellationTokeSource.Cancel();
        }

        protected async Task DesiredPropertyUpdated(TwinCollection dp, object userContext)
        {
            await appConnector.NotifyDeviceTwinsDesiredPropertiesAsync(dp);
        }

        protected async Task<MethodResponse> MethodInvocationHander(MethodRequest methodRequest, object userContext)
        {
            var response = await appConnector.InvokeDirectMethodAsync(methodRequest);
            return new MethodResponse(response.result, response.statusCode);
        }
    }
}
