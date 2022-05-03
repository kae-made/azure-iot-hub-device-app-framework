using Kae.IoT.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModule
{
    class D2CData : Kae.IoT.Framework.D2CMessage
    {
        public static readonly string Key_Temperature = "temperature";
        public static readonly string Key_Humidity = "humidity";
        public static readonly string Key_Pressure = "pressure";
        public static readonly string Key_Timestamp = "timestamp";
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
        public DateTime Timestamp { get; set; }

        public override IoTData Deserialize(string json)
        {
            return (IoTData)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(D2CData));
        }

        public override string Serialize()
        {
            var content = "{" + $"\"{Key_Temperature}\":{Temperature},\"{Key_Humidity}\":{Humidity},\"{Key_Pressure}\":{Pressure},\"{Key_Timestamp}\":\"{Timestamp.ToString("yyyy/MM/ddTHH:mm:ss.fff")}\"" + "}";
            return content;
        }
    }

    class AppDTDesiredProperties : Kae.IoT.Framework.IoTData
    {
        public static readonly string Key_IntervalMSec = "intervalMsec";
        public int IntervalMSec
        {
            get; set;

        }

        public override IoTData Deserialize(string json)
        {
            return (AppDTDesiredProperties)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(AppDTDesiredProperties));
        }

        public override string Serialize()
        {
            string content = "{" + $"\"{Key_IntervalMSec}\":{IntervalMSec}" + "}";
            return content;
        }
    }

    class AppDTReporetedProperties : Kae.IoT.Framework.IoTData
    {
        public static readonly string Key_Status = "status";

        public string Status { get; set; }
        public override IoTData Deserialize(string json)
        {
            return (AppDTReporetedProperties)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(AppDTReporetedProperties));
        }

        public override string Serialize()
        {
            string content = "{" + $"\"{Key_Status}\":\"{Status}\"" + "}";
            return content;
        }
    }


}