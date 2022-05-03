using Kae.IoT.Framework;
using Kae.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModule
{
    partial class IoTApp
    {
        private SampleloTAppConnector appConnector;
        private IoTClient iotClient;
        private D2CData sensingData;

        private Logger logger;

        public IoTApp()
        {
        }

        public D2CData GetD2CData()
        {
            return sensingData;
        }

        public async Task InitializeAsync(string configYamlFile)
        {
            DesiredProperties = new AppDTDesiredProperties();
            ReportedProperties = new AppDTReporetedProperties();

            logger = ConsoleLogger.CreateLogger();

            var appConfig = IoTAppConfigResolver.ResolveConfig(configYamlFile);
            appConnector = new SampleloTAppConnector(appConfig, this);
            iotClient = IoTClientFactory.CreateIoTClientForDevice(appConnector, logger);

            await iotClient.OpenAsync();


            DesiredProperties = (AppDTDesiredProperties)await iotClient.GetDeviceTwinsDesiredPropertiesAsync(DesiredProperties);
        }


        public async Task TerminateAsync()
        {
            await iotClient.CloseAsync();
        }
    }
}
