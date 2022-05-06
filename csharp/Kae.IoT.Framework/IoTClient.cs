// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.IoT.Framework
{
    public interface IoTClient
    {
        string DeviceId { get; }

        Task OpenAsync();
        Task CloseAsync();

        Task UpdateD2CDataAsync(IoTDataWithProperties data);
        Task UpdateDeviceTwinsReportedPropertiesAsync(IoTData data);
        Task<IoTData> GetDeviceTwinsDesiredPropertiesAsync(IoTData dtProps);

        Task SendD2CMessageAsync(IoTDataWithProperties data, string outputPort = null);
        Task StartSendD2CMessageAsync(TimeSpan interval, string outputPort = null);
        void StopSendD2CMessage();


        Task UploadLargeDataAsync(string blobName, Stream data);
    }
}
