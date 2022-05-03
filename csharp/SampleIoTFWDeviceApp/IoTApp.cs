using Kae.IoT.Framework;
using Kae.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleIoTFWDeviceApp
{
    partial class IoTApp
    {
        private SampleloTAppConnector appConnector;
        private IoTClient iotClient;
        private D2CData sensingData;

        private Logger logger;

        public IoTApp(Logger logger = null)
        {
            this.logger = logger;
        }

        public D2CData GetD2CData()
        {
            return sensingData;
        }

        public async Task InitializeAsync(string configYamlFile)
        {
            DesiredProperties = new AppDTDesiredProperties();
            ReportedProperties = new AppDTReporetedProperties();

            if (logger == null)
            {
                logger = ConsoleLogger.CreateLogger();
            }

            var iotAppConfig = IoTAppConfigResolver.ResolveConfig(configYamlFile);
            appConnector = new SampleloTAppConnector(iotAppConfig, this);
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
