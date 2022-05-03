using Kae.IoT.Framework.Authentication;
using Kae.Utility.Logging;
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.IoT.Framework
{
    internal class IoTClientModuleImple : IoTClientBase, IoTClient
    {
        private ModuleClient moduleClient;
        private IEnumerable<string> inputPorts;
        private IEnumerable<string> InputPorts { get { return inputPorts; } }

        public string DeviceId
        {
            get { return appConnector.AppConfig.DeviceId; }
        }

        internal IoTClientModuleImple(IoTAppConnector connector, IoTHubAuthentication auth, IEnumerable<string> inputPorts, Logger logger) : base(connector, logger)
        {
            this.client = this;
            this.inputPorts = inputPorts;
            moduleClient = auth.CreateModuleClient();
            connectionStatus = ConnectionStatus.Disconnected;
            moduleClient.SetConnectionStatusChangesHandler(ConnectionStatusChanged);
        }
        public async Task OpenAsync()
        {
            try
            {
                // TODO: How to apply input points
                foreach (var inputPort in inputPorts)
                {
                    var messageHandler = new InputMessageHandler(appConnector, inputPort);
                    await moduleClient.SetInputMessageHandlerAsync(inputPort, messageHandler.ReseiveInputMessageHandler, this);
                }
                await moduleClient.SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdated, this);
                await moduleClient.SetMethodDefaultHandlerAsync(MethodInvocationHander, this);
                await moduleClient.OpenAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        
        public async Task CloseAsync()
        {
            await moduleClient.CloseAsync();
        }

        public async Task<IoTData> GetDeviceTwinsDesiredPropertiesAsync(IoTData dtProps)
        {
            try
            {
                logger.LogInfo("getting twins...");
                var dp = await moduleClient.GetTwinAsync();
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

        public async Task SendD2CMessageAsync(D2CMessage data)
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
                await moduleClient.SendEventAsync(msg);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        public async Task SendD2CMessageAsync(D2CMessage data, string outputPort)
        {
            logger.LogInfo("sending d2c message...");
            try
            {
                var json = data.Serialize();
                logger.LogInfo($"  content - {json}");
                var msg = new Microsoft.Azure.Devices.Client.Message(System.Text.Encoding.UTF8.GetBytes(json));
                await moduleClient.SendEventAsync(outputPort, msg);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        public async Task StartSendD2CMessageAsync(TimeSpan interval, string outputPort)
        {
            d2cSendInterval = interval;
            await StartSendD2CMessagePeriodicallyAsync(outputPort);
        }

        public void StopSendD2CMessage()
        {
            StopSendD2CMessagePeriodically();
        }

        public async Task UpdateD2CDataAsync(D2CMessage data)
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
                await moduleClient.UpdateReportedPropertiesAsync(rp);
                logger.LogInfo("updated reported properties.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        public Task UploadLargeDataAsync(string blobName, Stream data)
        {
            throw new NotImplementedException();
        }
    }
}

