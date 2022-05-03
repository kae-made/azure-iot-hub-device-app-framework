using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleIoTFWServiceApp
{
    interface IIoTApp
    {
        AppDTDesiredProperties DesiredProperties
        {
            get;
            set;
        }

        Task DoWorkAsync();

        string Hello(string payload);
        string Test(string payload);

        D2CData GetD2CData();
        Task ReceivedC2DDataAsync(Message data);
        Task UpdatedDTDesiredPropertiesAsync(AppDTDesiredProperties dp);

        Task UserPreInitializeAsync();
        Task UserPostInitializeAsync();
        Task UserPreTerminateAsync();
        Task UserPostTerminateAsync();
    }
}
