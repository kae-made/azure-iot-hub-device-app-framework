// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
using Microsoft.Azure.Devices.Client;
using Kae.IoT.Framework.Authentication;

namespace Kae.IoT.Framework
{
    public class IoTAppConfig
    {
        public IoTAppConfig()
        {
            IsSASAuth = false;
            IsX509Auth = false;
            IsEdgeAuth = false;
            IsDPS = false;

            IoTHubTransportType = TransportType.Amqp;
            DPSTransportType = TransportType.Amqp;

            EdgeInputPorts = new List<string>();
        }

        public bool IsSASAuth { get; set; }
        public bool IsX509Auth { get; set; }
        public bool IsEdgeAuth { get; set; }
        public bool IsDPS { get; set; }

        public string ConnectionString { get; set; }
        public TransportType IoTHubTransportType { get; set; }
        public TransportType DPSTransportType { get; set; }
        public ClientOptions Options { get; set; }

        public string RootCertPath { get; set; }
        public string Intermediate1CertPath { get; set; }
        public string Intermediate2CertPath { get; set; }
        public string DevicePfxPath { get; set; }
        public string DevicePfxPassword { get; set; }
        public string IotHubUrl { get; set; }
        public string DeviceId { get; set; }
        public string DPSGlobalEndpoint { get; set; }
        public string DPSRegistrationId { get; set; }
        public string DPSSASKey { get; set; }
        public string DPSIdScope { get; set; }
        public string DPSCertPath { get; set; }
        public string DPSCertPassword { get; set; }
        public IList<string> EdgeInputPorts { get; set; }
        public string BlobOnEdgeModuleNameKey { get; set; }
        public string BlobOnEdgeAccountNameKey { get; set; }
        public string BlobOnEdgeAccountKeyKey { get; set; }

    }
    public class IoTAppConfigResolver
    {
        public static IoTAppConfig ResolveConfig(string configYamlFileName)
        {
            IoTAppConfig iotAppConfig = new IoTAppConfig();


            using (var reader = new StreamReader(configYamlFileName))
            {
                var yaml = new YamlStream();
                yaml.Load(reader);
                var config = (YamlMappingNode)yaml.Documents[0].RootNode;
                foreach (var configItem in config.Children)
                {
                    string itemKey = ((YamlScalarNode)configItem.Key).Value;
                    if (itemKey == "iot-hub")
                    {
                        if (configItem.Value.NodeType == YamlNodeType.Mapping)
                        {
                            foreach (var iotHubConfigItem in ((YamlMappingNode)configItem.Value).Children)
                            {
                                string iotHubConfigKey = ((YamlScalarNode)iotHubConfigItem.Key).Value;
                                string iotHubConfigValue = ((YamlScalarNode)iotHubConfigItem.Value).Value;
                                if (iotHubConfigKey == "connection-string")
                                {
                                    iotAppConfig.ConnectionString = iotHubConfigValue;
                                    iotAppConfig.IsSASAuth = true;
                                }
                                else if (iotHubConfigKey == "transport-type")
                                {
                                    var ttStr = iotHubConfigValue;
                                    iotAppConfig.IoTHubTransportType = ResolveTransportType(ttStr);
                                }
                                else if (iotHubConfigKey == "model-id")
                                {
                                    var miStr = iotHubConfigValue;
                                    if (!string.IsNullOrEmpty(miStr))
                                    {
                                        iotAppConfig.Options = new ClientOptions() { ModelId = miStr };
                                    }
                                }
                                else if (iotHubConfigKey == "root-cert-path")
                                {
                                    iotAppConfig.RootCertPath = iotHubConfigValue;
                                    iotAppConfig.IsX509Auth = true;
                                }
                                else if (iotHubConfigKey == "intermediate1-path")
                                {
                                    iotAppConfig.Intermediate1CertPath = iotHubConfigValue;
                                }
                                else if (iotHubConfigKey == "intermediate2-path")
                                {
                                    iotAppConfig.Intermediate2CertPath = iotHubConfigValue;
                                }
                                else if (iotHubConfigKey == "device-pfx-path")
                                {
                                    iotAppConfig.DevicePfxPath = iotHubConfigValue;
                                }
                                else if (iotHubConfigKey == "device-pfx-password")
                                {
                                    iotAppConfig.DevicePfxPassword = iotHubConfigValue;
                                }
                                else if (iotHubConfigKey == "hostname")
                                {
                                    iotAppConfig.IotHubUrl = iotHubConfigValue;
                                }
                                else if (iotHubConfigKey == "device-id")
                                {
                                    iotAppConfig.DeviceId = iotHubConfigValue;
                                }
                            }
                        }
                    }
                    else if (itemKey == "dps")
                    {
                        if (configItem.Value.NodeType == YamlNodeType.Mapping)
                        {
                            foreach (var dpsConfigItem in ((YamlMappingNode)configItem.Value).Children)
                            {
                                string dpsConfigKey = ((YamlScalarNode)dpsConfigItem.Key).Value;
                                string dpsConfigValue = ((YamlScalarNode)dpsConfigItem.Value).Value;
                                if (dpsConfigKey == "global-endpoint")
                                {
                                    iotAppConfig.DPSGlobalEndpoint = dpsConfigValue;
                                    iotAppConfig.IsDPS = true;
                                }
                                else if (dpsConfigKey == "id-scope")
                                {
                                    iotAppConfig.DPSIdScope = dpsConfigValue;
                                }
                                else if (dpsConfigKey == "registration-id")
                                {
                                    iotAppConfig.DPSRegistrationId = dpsConfigValue;
                                }
                                else if (dpsConfigKey == "sas-key")
                                {
                                    iotAppConfig.DPSSASKey = dpsConfigValue;
                                }
                                else if (dpsConfigKey == "cert-path")
                                {
                                    iotAppConfig.DPSCertPath = dpsConfigValue;
                                }
                                else if (dpsConfigKey == "cert-password")
                                {
                                    iotAppConfig.DPSCertPassword = dpsConfigValue;
                                }
                                else if (dpsConfigKey == "transport-type")
                                {
                                    iotAppConfig.DPSTransportType = ResolveTransportType(dpsConfigValue);
                                }
                            }
                        }
                    }
                    else if (itemKey == "iot-edge")
                    {
                        if (configItem.Value.NodeType == YamlNodeType.Mapping)
                        {
                            iotAppConfig.IsEdgeAuth = true;
                            foreach (var edgeConfigItem in ((YamlMappingNode)configItem.Value).Children)
                            {
                                string edgeConfigKey = ((YamlScalarNode)edgeConfigItem.Key).Value;
                                if (edgeConfigKey == "input-ports")
                                {
                                    var inputPortsConfig = (YamlSequenceNode)edgeConfigItem.Value;
                                    foreach (var inputPortConfig in inputPortsConfig)
                                    {
                                        iotAppConfig.EdgeInputPorts.Add(((YamlScalarNode)inputPortConfig).Value);
                                    }
                                }
                                else if (edgeConfigKey == "model-id")
                                {
                                    iotAppConfig.Options = new ClientOptions() { ModelId = ((YamlScalarNode)edgeConfigItem.Value).Value };
                                }
                                else if (edgeConfigKey == "blob-on-edge-module-name-key")
                                {
                                    iotAppConfig.BlobOnEdgeModuleNameKey = ((YamlScalarNode)edgeConfigItem.Value).Value;
                                }
                                else if (edgeConfigKey == "blob-on-edge-account-name-key")
                                {
                                    iotAppConfig.BlobOnEdgeAccountNameKey = ((YamlScalarNode)edgeConfigItem.Value).Value;
                                }
                                else if (edgeConfigKey == "blob-on-edge-account-key-key")
                                {
                                    iotAppConfig.BlobOnEdgeAccountKeyKey = ((YamlScalarNode)edgeConfigItem.Value).Value;
                                }
                            }
                        }
                    }
                }
            }

            return iotAppConfig;
#if false
                if (IsSASAuth)
                {
                    authentication = new IoTHubAuthSymmetricKey(ConnectionString, IoTHubTransportType, Options);
                }
                if (IsX509Auth)
                {
                    authentication = new IoTHubAuthX509(IotHubUrl, DeviceId, RootCertPath, Intermediate1CertPath, Intermediate2CertPath, DevicePfxPath, DevicePfxPassword, IoTHubTransportType, Options);
                }
                if (IsSASAuth == false && IsX509Auth == false)
                {
                    authentication = new IoTHubAuthEnv(IoTHubTransportType, Options);
                }

                if (isDPS)
                {
                    if ((!string.IsNullOrEmpty(DPSGlobalEndpoint)) && (!string.IsNullOrEmpty(DPSIdScope)))
                    {
                        if ((!string.IsNullOrEmpty(DPSCertPath)) && (!string.IsNullOrEmpty(DPSCertPassword)))
                        {
                            authentication = new IoTHubAuthDPSX509(DPSGlobalEndpoint, DPSIdScope, DPSCertPath, DPSCertPassword, DPSTransportType, IoTHubTransportType, Options);
                        }
                        if (!string.IsNullOrEmpty(DPSRegistrationId))
                        {
                            if (!string.IsNullOrEmpty(DPSSASKey))
                            {
                                authentication = new IoTHubAuthDPSTpm(DPSGlobalEndpoint, DPSIdScope, DPSRegistrationId, DPSTransportType, IoTHubTransportType, Options);
                            }
                            else
                            {
                                authentication = new IoTHubAuthDPSSymmetricKey(DPSGlobalEndpoint, DPSIdScope, DPSRegistrationId, DPSSASKey, null, DPSTransportType, IoTHubTransportType, Options);
                            }
                        }
                    }
                }
                if (IsEdgeAuth)
                {
                    authentication = new IoTHubAuthEnv(IoTHubTransportType, Options);
                }
            }
            return authentication;
#endif
        }

        private static TransportType ResolveTransportType(string ttStr)
        {
            TransportType transportType = TransportType.Amqp;
            switch (ttStr.ToLower())
            {
                case "amqp":
                    transportType = TransportType.Amqp;
                    break;
                case "amqp_tcp_only":
                    transportType = TransportType.Amqp_Tcp_Only;
                    break;
                case "http":
                    transportType = TransportType.Http1;
                    break;
                case "mqtt":
                    transportType = TransportType.Mqtt;
                    break;
                case "mqtt_tcp_only":
                    transportType = TransportType.Mqtt_Tcp_Only;
                    break;
            }

            return transportType;
        }
    }
}