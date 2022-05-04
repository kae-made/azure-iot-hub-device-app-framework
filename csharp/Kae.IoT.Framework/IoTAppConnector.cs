// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.IoT.Framework.Authentication;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.IoT.Framework
{
    public abstract class IoTAppConnector
    {
        public IoTAppConfig AppConfig { get; set; }
        protected IoTAppConnector(IoTAppConfig appConfig)
        {
            this.AppConfig = appConfig;
        }

        public abstract IoTDataWithProperties GetAppD2CData();
        public abstract TwinCollection ResolveDeviceTwinsReportedProperties(IoTData rp);

        public abstract Task NotifyC2DMessageAsync(Message msg);
        public abstract Task NotifyE2DMessageAsync(Message msg, string inputPort);
        public abstract Task NotifyDeviceTwinsDesiredPropertiesAsync(TwinCollection dp);


        // called by framework
        public abstract Task<(byte[] result, int statusCode)> InvokeDirectMethodAsync(MethodRequest methodRequest);
    }
}
