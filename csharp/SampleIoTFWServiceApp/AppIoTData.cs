using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleIoTFWServiceApp
{
    class D2CData : Kae.IoT.Framework.IoTDataWithProperties
    {
        public static readonly string Key_Temperature = "temperature";
        public static readonly string Key_Humidity = "humidity";
        public static readonly string Key_Pressure = "pressure";
        public static readonly string Key_Timestamp = "timestamp";
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
        public DateTime Timestamp { get; set; }

        public override D2CData Deserialize(string json)
        {
            return (D2CData)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(D2CData));
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

        public override AppDTDesiredProperties Deserialize(string json)
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
        public override AppDTReporetedProperties Deserialize(string json)
        {
            return (AppDTReporetedProperties)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(AppDTReporetedProperties));
        }

        public override string Serialize()
        {
            string content = "{" + $"\"{Key_Status}\":\"{Status}\"" + "}";
            return content;
        }
    }

    class C2DData : Kae.IoT.Framework.IoTData
    {
        public static readonly string Key_Command = "command";
        public static readonly string Key_Operation = "operation";

        public string Command { get; set; }
        public string Operation { get; set; }
        public override C2DData Deserialize(string json)
        {
            return (C2DData)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(C2DData));
        }

        public override string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
