// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Security;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.IoT.Framework.Authentication
{
    public class IoTHubAuthDPSTpm : IoTHubAuthDPS
    {
        public string RegistrationId { get; set; }
        protected SecurityProviderTpm CurrentSecurityProvider { get; set; }
        public IoTHubAuthDPSTpm(string globalEndpoint, string idScope, string registrationId, TransportType dpsTransportType,
            TransportType iothubTransportType, ClientOptions clientOptions) : base(globalEndpoint, idScope, dpsTransportType, iothubTransportType, clientOptions)
        {
            this.RegisrationId = registrationId;
        }
        public override DeviceClient CreateDeviceClient()
        {
            CheckLogger();
            Logger.LogInfo("Initializing the device provisioning clinet for Symmetric Key...");

            var result = RegisterDeviceAsync();
            result.Wait();
            var dpsResult = result.Result;
            if (dpsResult.Status ==  Microsoft.Azure.Devices.Provisioning.Client.ProvisioningRegistrationStatusType.Assigned)
            {
                var auth = new DeviceAuthenticationWithTpm(dpsResult.DeviceId, CurrentSecurityProvider);
                return DeviceClient.Create(dpsResult.AssignedHub, auth, this.TransportType, this.Options);
            }
            else
            {
                return null;
            }
        }

        public override ModuleClient CreateModuleClient()
        {
            throw new NotImplementedException();
        }

        protected override SecurityProvider CreateSecurityProvider()
        {
            var security = new SecurityProviderTpmHsm(RegistrationId);
            this.CurrentSecurityProvider = security;
            return security;
        }
    }
}
