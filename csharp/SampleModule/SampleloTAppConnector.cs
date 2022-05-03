﻿using Kae.IoT.Framework;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModule
{
    class SampleloTAppConnector : Kae.IoT.Framework.IoTAppConnector
    {
        private IIoTApp iotApp;

        public SampleloTAppConnector(IoTAppConfig appConfig, IIoTApp app) : base(appConfig)
        {
            iotApp = app;
        }
        public override async Task<(byte[] result, int statusCode)> InvokeDirectMethodAsync(MethodRequest methodRequest)
        {
            byte[] result = null;
            int statusCode = (int)System.Net.HttpStatusCode.OK;
            try
            {
                switch (methodRequest.Name)
                {
                    case "Hello":
                        result = System.Text.Encoding.UTF8.GetBytes(iotApp.Hello(methodRequest.DataAsJson));
                        break;
                    case "Test":
                        result = System.Text.Encoding.UTF8.GetBytes(iotApp.Test(methodRequest.DataAsJson));
                        break;
                    default:
                        statusCode = (int)System.Net.HttpStatusCode.BadRequest;
                        break;
                }
            }
            catch (Exception ex)
            {
                result = System.Text.Encoding.UTF8.GetBytes(ex.Message);
                statusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }
            return (result, statusCode);

        }

        public override Task NotifyC2DMessageAsync(Message msg)
        {
            throw new NotImplementedException();
        }

        public override async Task NotifyDeviceTwinsDesiredPropertiesAsync(TwinCollection dp)
        {
            var newProps = iotApp.DesiredProperties.Deserialize(dp.ToJson());
            lock (iotApp.DesiredProperties)
            {
                iotApp.DesiredProperties = (AppDTDesiredProperties)newProps;
            }
            await iotApp.UpdatedDTDesiredPropertiesAsync(iotApp.DesiredProperties);
        }

        public override TwinCollection ResolveDeviceTwinsReportedProperties(IoTData appRP)
        {
            TwinCollection rp = new TwinCollection(appRP.Serialize());
            return rp;
        }

        public override D2CMessage GetAppD2CData()
        {
            return iotApp.GetD2CData();
        }

        public override async Task NotifyE2DMessageAsync(Message msg, string inputPort)
        {
            await iotApp.ReceivedE2DDataAsync(msg, inputPort);
        }

    }
}
