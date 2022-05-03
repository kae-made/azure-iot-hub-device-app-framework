// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.IoT.Framework.Authentication
{
    public class IoTHubAuthEnv : IoTHubAuthentication
    {
        public IoTHubAuthEnv(ClientOptions options) : base(TransportType.Mqtt_Tcp_Only, options)
        {

        }

        public override DeviceClient CreateDeviceClient()
        {
            throw new NotImplementedException();
        }

        public override ModuleClient CreateModuleClient()
        {
            var mqttSetting = new MqttTransportSettings(TransportType);
            ITransportSettings[] settings = { mqttSetting };
            var t = ModuleClient.CreateFromEnvironmentAsync(settings, Options);
            t.Wait();
            return t.Result;
        }
    }
}
