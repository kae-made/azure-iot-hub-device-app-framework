// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.Utility.Logging;
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.IoT.Framework.Authentication
{
    public abstract class IoTHubAuthentication
    {
        public string IoTHubUrl { get; set; }
        public string DeviceId { get; set; }
        public TransportType TransportType { get; set; }
        public ClientOptions Options { get; set; }
        public Logger Logger { get; set; }
        public IoTHubAuthentication(TransportType transportType, ClientOptions options)
        {
            this.TransportType = transportType;
            this.Options = options;
        }
        public abstract DeviceClient CreateDeviceClient();
        public abstract ModuleClient CreateModuleClient();

        protected void CheckLogger()
        {
            if (Logger == null)
            {
                Logger = ConsoleLogger.CreateLogger();
            }
        }
    }
}
