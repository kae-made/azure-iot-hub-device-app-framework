// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.IoT.Framework.Authentication
{
    public abstract class IoTHubAuthDPS :IoTHubAuthentication
    {
        public string GlobalEndpoint { get; set; }
        public string IDScope { get; set; }
        public string RegisrationId { get; set; }
        public Microsoft.Azure.Devices.Provisioning.Client.Transport.ProvisioningTransportHandler ProvisioningTransport { get; set; }
        protected abstract SecurityProvider CreateSecurityProvider();
        
        protected IoTHubAuthDPS(string globalEndpoint, string idScope, TransportType dpsTransportType, TransportType iothubTransportType, ClientOptions clientOptions):base (iothubTransportType, clientOptions)
        {
            this.GlobalEndpoint = globalEndpoint;
            this.IDScope = idScope;
            this.ProvisioningTransport = GetTransporthandler(dpsTransportType);
        }

        protected async Task<Microsoft.Azure.Devices.Provisioning.Client.DeviceRegistrationResult> RegisterDeviceAsync()
        {
            var security = CreateSecurityProvider();
            var provClient = Microsoft.Azure.Devices.Provisioning.Client.ProvisioningDeviceClient.Create(GlobalEndpoint, IDScope, security, ProvisioningTransport);
            Logger.LogInfo($"Registering ${RegisrationId} device to {GlobalEndpoint} by IDScope:{IDScope}");
            var dpsResult = await provClient.RegisterAsync();
            
            if (dpsResult.Status != Microsoft.Azure.Devices.Provisioning.Client.ProvisioningRegistrationStatusType.Assigned)
            {
                Logger.LogError($"Registration Failed - ${dpsResult.Status} - ${dpsResult.JsonPayload}");
                return null;
            }
            Logger.LogInfo($"{dpsResult.DeviceId} has been registered to {dpsResult.AssignedHub}");

            return dpsResult;
        }

        protected Microsoft.Azure.Devices.Provisioning.Client.Transport.ProvisioningTransportHandler GetTransporthandler(TransportType transportType)
        {
            return transportType switch
            {
                TransportType.Amqp => new Microsoft.Azure.Devices.Provisioning.Client.Transport.ProvisioningTransportHandlerAmqp(),
                TransportType.Amqp_Tcp_Only => new Microsoft.Azure.Devices.Provisioning.Client.Transport.ProvisioningTransportHandlerAmqp(TransportFallbackType.TcpOnly),
                TransportType.Amqp_WebSocket_Only => new Microsoft.Azure.Devices.Provisioning.Client.Transport.ProvisioningTransportHandlerAmqp(TransportFallbackType.WebSocketOnly),
                TransportType.Http1 => new Microsoft.Azure.Devices.Provisioning.Client.Transport.ProvisioningTransportHandlerHttp(),
                TransportType.Mqtt => new Microsoft.Azure.Devices.Provisioning.Client.Transport.ProvisioningTransportHandlerMqtt(),
                TransportType.Mqtt_Tcp_Only => new Microsoft.Azure.Devices.Provisioning.Client.Transport.ProvisioningTransportHandlerMqtt(TransportFallbackType.TcpOnly),
                TransportType.Mqtt_WebSocket_Only => new Microsoft.Azure.Devices.Provisioning.Client.Transport.ProvisioningTransportHandlerMqtt(TransportFallbackType.WebSocketOnly),
                _ => throw new NotSupportedException($"Unsupported transport type {transportType}"),
            };
        }
    }
}
