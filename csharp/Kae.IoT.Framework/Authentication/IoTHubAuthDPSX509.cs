// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Kae.IoT.Framework.Authentication
{
    public class IoTHubAuthDPSX509 : IoTHubAuthDPS
    {
        public IoTHubAuthDPSX509(string globalEndpoint, string idScope,  string certificatePath, string certificatePathword, TransportType dpsTransportType,
            TransportType iothubTransportType, ClientOptions clientOptions) : base(globalEndpoint, idScope, dpsTransportType, iothubTransportType, clientOptions)
        {
            this.CertifiatePath = certificatePath;
            this.CertificatePassword = certificatePathword;
        }
        public string CertificatePassword { get; set; }
        public string CertifiatePath { get; set; }
        protected X509Certificate2 Certificate { get; set; }
        public override DeviceClient CreateDeviceClient()
        {
            CheckLogger();
            Logger.LogInfo("Initializing the device provisioning clinet for Symmetric Key...");
            var result = this.RegisterDeviceAsync();
            result.Wait();
            var dpsResult = result.Result;
            if (dpsResult.Status == ProvisioningRegistrationStatusType.Assigned)
            {
                var auth = new DeviceAuthenticationWithX509Certificate(dpsResult.DeviceId, Certificate);
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
            var certCollection = new X509Certificate2Collection();
            certCollection.Import(
                CertifiatePath,
                CertificatePassword,
                 X509KeyStorageFlags.UserKeySet);
            foreach(var elem in certCollection)
            {
                if (elem.HasPrivateKey)
                {
                    Certificate = elem;
                }
                else
                {
                    elem.Dispose();
                }
            }
            if (Certificate == null)
            {
                Logger.LogError($"{CertifiatePath} is not found.");
                throw new FileNotFoundException($"{CertifiatePath} didn't contain any certificate with a private key.");
            }
            var security = new SecurityProviderX509Certificate(Certificate);
            return security;
        }
    }
}
