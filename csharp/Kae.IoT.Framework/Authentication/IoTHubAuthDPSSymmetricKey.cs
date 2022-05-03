// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.Utility;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.IoT.Framework.Authentication
{
    public class IoTHubAuthDPSSymmetricKey : IoTHubAuthDPS
    {
        public string DPSSASPrimaryKey { get; set; }
        public string DPSSASSecondaryKey { get; set; }
        protected string IoTHubSASKey { get; set; }
        public IoTHubAuthDPSSymmetricKey(string globalEndpoint, string idScope, string registrationId, string sasPrimaryKey, string sasSecondaryKey,  TransportType dpsTransportType,
            TransportType iothubTransportType, ClientOptions clientOptions) : base(globalEndpoint, idScope, dpsTransportType, iothubTransportType, clientOptions)
        {
            this.RegisrationId = registrationId;
            this.DPSSASPrimaryKey = sasPrimaryKey;
            this.DPSSASSecondaryKey = sasSecondaryKey;
        }
        public override DeviceClient CreateDeviceClient()
        {
            CheckLogger();

            Logger.LogInfo("Initializing the device provisioning clinet for Symmetric Key...");
            var result = this.RegisterDeviceAsync();
            result.Wait();
            var dpsResult = result.Result;
            if (dpsResult.Status == ProvisioningRegistrationStatusType.Assigned)
            {
                var auth = new DeviceAuthenticationWithRegistrySymmetricKey(dpsResult.DeviceId, IoTHubSASKey);
                return DeviceClient.Create(dpsResult.AssignedHub, auth, TransportType, this.Options);
            }
            return null;
        }

        public override ModuleClient CreateModuleClient()
        {
            throw new NotImplementedException();
        }

        protected override SecurityProvider CreateSecurityProvider()
        {
            var security = new SecurityProviderSymmetricKey(RegisrationId, DPSSASPrimaryKey, DPSSASSecondaryKey);
            IoTHubSASKey = security.GetPrimaryKey();
            return security;
        }
    }
}
