using Kae.IoT.Framework.Authentication;
using Kae.Utility.Logging;
// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.IoT.Framework
{
    public class IoTClientFactory
    {
        public static IoTClient CreateIoTClientForDevice(IoTAppConnector appConnector, Logger logger)
        {
            var auth = CreateIoTHubAuth(appConnector.AppConfig);
            var iotClient = new IoTClientDeviceImpl(appConnector, auth, logger);
            return iotClient;
        }
        public static IoTClient CreateIoTClientForEdge(IoTAppConnector appConnector, Logger logger)
        {
            var auth = CreateIoTHubAuth(appConnector.AppConfig);
            var iotClient = new IoTClientModuleImple(appConnector, auth, appConnector.AppConfig.EdgeInputPorts, logger);
            return iotClient;
        }

        public static IoTHubAuthentication CreateIoTHubAuth(IoTAppConfig config)
        {
            IoTHubAuthentication authentication = null;
            if (config.IsSASAuth)
            {
                authentication = new IoTHubAuthSymmetricKey(config.ConnectionString, config.IoTHubTransportType, config.Options);
            }
            if (config.IsX509Auth)
            {
                authentication = new IoTHubAuthX509(config.IotHubUrl, config.DeviceId, config.RootCertPath, config.Intermediate1CertPath, config.Intermediate2CertPath, config.DevicePfxPath, config.DevicePfxPassword, config.IoTHubTransportType, config.Options);
            }
            if (config.IsSASAuth == false && config.IsX509Auth == false && config.IsDPS == false && config.IsEdgeAuth)
            {
                authentication = new IoTHubAuthEnv(config.Options);
            }
            if (config.IsDPS)
            {
                if ((!string.IsNullOrEmpty(config.DPSGlobalEndpoint)) && (!string.IsNullOrEmpty(config.DPSIdScope)))
                {
                    if ((!string.IsNullOrEmpty(config.DPSCertPath)) && (!string.IsNullOrEmpty(config.DPSCertPassword)))
                    {
                        authentication = new IoTHubAuthDPSX509(config.DPSGlobalEndpoint, config.DPSIdScope, config.DPSCertPath, config.DPSCertPassword, config.DPSTransportType, config.IoTHubTransportType, config.Options);
                    }
                    if (!string.IsNullOrEmpty(config.DPSRegistrationId))
                    {
                        if (!string.IsNullOrEmpty(config.DPSSASKey))
                        {
                            authentication = new IoTHubAuthDPSTpm(config.DPSGlobalEndpoint, config.DPSIdScope, config.DPSRegistrationId, config.DPSTransportType, config.IoTHubTransportType, config.Options);
                        }
                        else
                        {
                            authentication = new IoTHubAuthDPSSymmetricKey(config.DPSGlobalEndpoint, config.DPSIdScope, config.DPSRegistrationId, config.DPSSASKey, null, config.DPSTransportType, config.IoTHubTransportType, config.Options);
                        }
                    }
                }
            }
            return authentication;
        }
    }
}
