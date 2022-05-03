// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.IoT.Framework.Authentication
{
    public class IoTHubAuthSymmetricKey : IoTHubAuthentication
    {
        private string iotHubConnectionString;

        public IoTHubAuthSymmetricKey(string connectionString, TransportType transportType, ClientOptions options) : base(transportType, options)
        {
            iotHubConnectionString = connectionString;
            var builder = IotHubConnectionStringBuilder.Create(connectionString);
            this.IoTHubUrl = builder.HostName;
            this.DeviceId=builder.DeviceId;
        }

        public override DeviceClient CreateDeviceClient()
        {
            return DeviceClient.CreateFromConnectionString(iotHubConnectionString, TransportType, Options);
        }

        public override ModuleClient CreateModuleClient()
        {
            throw new NotImplementedException();
        }
    }
}

