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
    internal class InputMessageHandler
    {
        private IoTAppConnector appConnector;
        private string inputPort;
        public string InputPort { get { return InputPort; } }
        public InputMessageHandler(IoTAppConnector connector, string inputPort)
        {
            appConnector = connector;
            this.inputPort = inputPort;
        }

        public async Task<MessageResponse> ReseiveInputMessageHandler(Message message, object userContext)
        {
            await appConnector.NotifyE2DMessageAsync(message, inputPort);
            return new MessageResponse();            
        }
    }
}
