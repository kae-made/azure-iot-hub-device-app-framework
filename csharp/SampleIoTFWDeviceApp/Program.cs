using System;

namespace SampleIoTFWDeviceApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var iotApp = new IoTApp();

            iotApp.UserPreInitializeAsync().Wait();

            iotApp.InitializeAsync("iot-app-config.yaml").Wait();

            iotApp.UserPostInitializeAsync().Wait();

            iotApp.DoWorkAsync().Wait();

            iotApp.UserPreTerminateAsync().Wait();

            iotApp.TerminateAsync().Wait();

            iotApp.UserPostTerminateAsync().Wait();

            
        }
    }
}
