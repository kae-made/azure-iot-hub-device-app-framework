// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.IoT.Framework.Authentication;
using Kae.Utility.Logging;
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kae.IoT.Framework
{
    internal class IoTClientDeviceImpl : IoTClientBase, IoTClient
    {
        private Microsoft.Azure.Devices.Client.DeviceClient deviceClinet;

        public string DeviceId { get { return appConnector.AppConfig.DeviceId; } }

        internal IoTClientDeviceImpl(IoTAppConnector connector, IoTHubAuthentication auth, Logger logger) : base(connector, logger)
        {
            client = this;
            deviceClinet = auth.CreateDeviceClient();
            connectionStatus = ConnectionStatus.Disconnected;
            deviceClinet.SetConnectionStatusChangesHandler(ConnectionStatusChanged);
        }

        public async Task OpenAsync()
        {
            try
            {
                logger.LogInfo("Setting handlers and try to open...");
                await deviceClinet.SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdated, this);
                await deviceClinet.SetMethodDefaultHandlerAsync(MethodInvocationHander, this);
                await deviceClinet.SetReceiveMessageHandlerAsync(C2DMessageReceivedHandler, this);
                await deviceClinet.OpenAsync();
                logger.LogInfo("IoT Hub has been opened.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        private async Task C2DMessageReceivedHandler(Message message, object userContext)
        {
            logger.LogInfo("received c2d message...");
            var content = System.Text.Encoding.UTF8.GetString(message.GetBytes());
            logger.LogInfo($"  content - {content}");
            await appConnector.NotifyC2DMessageAsync(message);
            await deviceClinet.CompleteAsync(message);
        }

        public async Task CloseAsync()
        {
            logger.LogInfo("IoT Hub connection closing...");
            await deviceClinet.CloseAsync();
            deviceClinet.Dispose();
            lock (this)
            {
                connectionStatus = ConnectionStatus.Disconnected;
            }
            logger.LogInfo("IoT Hub connection has been closed.");
        }

        public async Task<IoTData> GetDeviceTwinsDesiredPropertiesAsync(IoTData dtProps)
        {
            try
            {
                logger.LogInfo("getting twins...");
                var dp = await deviceClinet.GetTwinAsync();
                var dpJson = dp.Properties.Desired.ToJson();
                dtProps.Deserialize(dpJson);
                logger.LogInfo("got twins.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
            return dtProps;
        }

        protected override async Task ResolveDesiredProperties(Microsoft.Azure.Devices.Shared.TwinCollection dpTwin)
        {
            // Do nothing
        }

        public async Task SendD2CMessageAsync(IoTDataWithProperties data, string outputPort)
        {
            logger.LogInfo("sending d2c message...");
            try
            {
                var json = data.Serialize();
                logger.LogInfo($"  content - {json}");
                var msg = new Microsoft.Azure.Devices.Client.Message(System.Text.Encoding.UTF8.GetBytes(json));
                foreach(var prop in data.Properties)
                {
                    msg.Properties.Add(prop.Key, prop.Value);
                }
                await deviceClinet.SendEventAsync(msg);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        public async Task StartSendD2CMessageAsync(TimeSpan interval, CancellationTokenSource cancelTokenSource, string outputPort)
        {
            d2cSendInterval = interval;
            await StartSendD2CMessagePeriodicallyAsync(cancelTokenSource);
        }

        public void StopSendD2CMessage()
        {
            StopSendD2CMessagePeriodically();
        }

        public async Task UpdateD2CDataAsync(IoTDataWithProperties data)
        {
            lock (d2cData)
            {
                d2cData = data;
            }
        }

        public async Task UpdateDeviceTwinsReportedPropertiesAsync(IoTData data)
        {
            logger.LogInfo("updating reported properties...");
            try
            {
                var rp = appConnector.ResolveDeviceTwinsReportedProperties(data);
                await deviceClinet.UpdateReportedPropertiesAsync(rp);
                logger.LogInfo("updated reported properties.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        public async Task UploadLargeDataAsync(string blobName, Stream data)
        {
            logger.LogInfo("updating large data...");
            try
            {
                var sasUriRequest = new Microsoft.Azure.Devices.Client.Transport.FileUploadSasUriRequest
                {
                    BlobName = blobName
                };
                var uriResponse = await deviceClinet.GetFileUploadSasUriAsync(sasUriRequest);
                var uploadUri = uriResponse.GetBlobUri();
                var blobClient = new Azure.Storage.Blobs.Specialized.BlockBlobClient(uploadUri);
                await blobClient.UploadAsync(data, new Azure.Storage.Blobs.Models.BlobUploadOptions());
                var uploadCompletionNotification = new Microsoft.Azure.Devices.Client.Transport.FileUploadCompletionNotification
                {
                    CorrelationId = uriResponse.CorrelationId,
                    IsSuccess = true,
                    StatusCode = 200,
                    StatusDescription = "Success"
                };
                await deviceClinet.CompleteFileUploadAsync(uploadCompletionNotification);
                logger.LogInfo("updated large data.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }
    }
}
