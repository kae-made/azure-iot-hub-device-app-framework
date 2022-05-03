using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleIoTFWDeviceApp
{
    partial class IoTApp : IIoTApp
    {
        public AppDTDesiredProperties DesiredProperties { get; set; }
        public AppDTReporetedProperties ReportedProperties { get; set; }


        public string Hello(string payload)
        {
            Console.WriteLine($"Invoked Hello with '{payload}'");
            return $"How are you? {payload}";
        }
        public string Test(string payload)
        {
            Console.WriteLine($"Invoked Test with '{payload}'");
            dynamic pl = Newtonsoft.Json.JsonConvert.DeserializeObject(payload);
            double x = pl["x"];
            double y = pl["y"];
            var result = new
            {
                z = x + y
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(result);
        }


        public async Task ReceivedC2DDataAsync(Message message)
        {
            Console.WriteLine($"Received C2D - {System.Text.Encoding.UTF8.GetString(message.GetBytes())}");
            if (message.Properties.Count > 0)
            {
                Console.WriteLine("  properties...");
                foreach (var prop in message.Properties)
                {
                    Console.WriteLine($"  {prop.Key}:{prop.Value}");
                }
            }
        }

        public async Task UpdatedDTDesiredPropertiesAsync(AppDTDesiredProperties dp)
        {
            Console.WriteLine($"Updated DT Desired Props - {DesiredProperties.Serialize()}");
        }

        public async Task UserPreInitializeAsync()
        {
            sensingData = new D2CData()
            {
                Temperature = 20.0f,
                Humidity = 50.0f,
                Pressure = 1013.0f,
                Timestamp = DateTime.Now
            };


            // TODO: impliment
        }

        public async Task UserPostTerminateAsync()
        {
            // TODO: implement
        }

        public async Task DoWorkAsync()
        {
            ReportedProperties.Status = "running";
            await iotClient.UpdateDeviceTwinsReportedPropertiesAsync(ReportedProperties);
            var task = new Task(async () =>
            {
                var rand = new Random(DateTime.Now.Millisecond);
                var sensingInterval = TimeSpan.FromMilliseconds(1000);
                while (true)
                {
                    // 予めフレームワーク側に変数を教えているので、更新通知はいらない。-> いや、やっぱり更新を明示的にやった方がええだろ。
                    await iotClient.UpdateD2CDataAsync(sensingData);
                    // 都度都度送信したい場合は、
                    // await iotClient.SendD2CMessageAsync(sensingData);
                    await Task.Delay(sensingInterval);
                    lock (sensingData)
                    {
                        sensingData.Temperature += 0.1 * (0.5 - rand.NextDouble());
                        sensingData.Humidity += 0.1 * (0.5 - rand.NextDouble());
                        sensingData.Pressure += 0.05 * (0.5 - rand.NextDouble());
                        sensingData.Timestamp = DateTime.Now;
                    }
                }
            });
            task.Start();
            iotClient.StartSendD2CMessageAsync(TimeSpan.FromSeconds(10));

            var key = Console.ReadKey();
            iotClient.StopSendD2CMessage();

        }


        public async Task UserPreTerminateAsync()
        {
            // TODO: implement
        }

        public async Task UserPostInitializeAsync()
        {
            // TODO: implement
        }

    }
}
