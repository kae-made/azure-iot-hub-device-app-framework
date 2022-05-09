// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.IoT.Framework
{
    public abstract class IoTData
    {
        public abstract string Serialize();
        public abstract IoTData Deserialize(string json);
    }

    public abstract class IoTDataWithProperties : IoTData
    {
        [Newtonsoft.Json.JsonIgnore]
        public IDictionary<string, string> Properties { get; set; }

        public IoTDataWithProperties()
        {
            Properties = new Dictionary<string, string>();
        }
    }
}
