// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Kae.IoT.Framework.Authentication
{
    public class IoTHubAuthX509 : IoTHubAuthentication
    {
        private string rootCertPath;
        private string intermediate1CertPath;
        private string intermediate2CertPath;
        private string devicePfxPath;
        private string devicePfxPassword;

        public IoTHubAuthX509(string hostName, string deviceId, string rootCertPath, string inter1CertPath, string inter2CertPath, string pfxFileName, string password, TransportType transportType, ClientOptions options) : base(transportType, options)
        {
            this.IoTHubUrl = hostName;
            this.DeviceId = deviceId;
            this.rootCertPath = rootCertPath;
            this.intermediate1CertPath = inter1CertPath;
            this.intermediate2CertPath = inter2CertPath;
            devicePfxPath = pfxFileName;
            devicePfxPassword = password;
        }
        public override DeviceClient CreateDeviceClient()
        {
            var chainCerts = new X509Certificate2Collection();
            chainCerts.Add(new X509Certificate2(rootCertPath));
            chainCerts.Add(new X509Certificate2(intermediate1CertPath));
            chainCerts.Add(new X509Certificate2(intermediate2CertPath));
            var deviceCert = new X509Certificate2(devicePfxPath, devicePfxPassword);
            var auth = new DeviceAuthenticationWithX509Certificate(DeviceId, deviceCert, chainCerts);
            return DeviceClient.Create(IoTHubUrl, auth, TransportType);
        }

        public override ModuleClient CreateModuleClient()
        {
            throw new NotImplementedException();
        }
    }
}
