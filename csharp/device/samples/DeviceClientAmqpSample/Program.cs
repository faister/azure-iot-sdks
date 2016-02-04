// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.Client.Samples
{
    class Program
    {

        private const string DeviceConnectionString = "HostName=northryde.azure-devices.net;DeviceId=34242880-43b5-49c7-ad65-fe2c17f922d1;SharedAccessKey=JpTwASYeinr2Hvr3xu7P9w==";
            //"HostName=ozmote.azure-devices.net;DeviceId=ThePondsGenerator;SharedAccessKey=RS4gIpUX06b2iZ6prIdxDw==";
            //"HostName=FaisterRemote.azure-devices.net;DeviceId=FaisterNodeMCU;SharedAccessKey=lYn3o0OUYjSmCz4z9pp3Nw==";
            //"HostName=makerdenhub.azure-devices.net;DeviceId=NodeMCUTaipei;SharedAccessKey=+SQqvFv0lSdSeb5JhohJ1Q==";
            //"HostName=makerdenhub.azure-devices.net;DeviceId=s3innovate;SharedAccessKey=WX+kfhtPHX/eKyjGK4ZdMg=="; //"HostName=makerdenhub.azure-devices.net;DeviceId=rpi2nodejs;SharedAccessKey=SIzS+QpsYfBCYso6ncYA9w==";
            //"HostName=FaisterRemote.azure-devices.net;DeviceId=IgniteBand;SharedAccessKey=RtBb9H28DX/TsgTcwUFpZg=="; //"HostName=FaisterRemote.azure-devices.net;DeviceId=FaisterBand;SharedAccessKey=LcMzzunxxjIgrVNBPfnlcw=="; //"HostName=FaisterRemote.azure-devices.net;DeviceId=IgniteTestDevice;SharedAccessKey=bMEtne4yZi88OD15jWguKA=="; //"HostName=FaisterRemote.azure-devices.net;DeviceId=FaisterNodeMCU;SharedAccessKey=lYn3o0OUYjSmCz4z9pp3Nw=="; //"HostName=FaisterRemote.azure-devices.net;DeviceId=AzureIoTRedCarpetDevice;SharedAccessKey=TycEITRqvlY39vg8oUIFzA=="; //"HostName=FaisterRemote.azure-devices.net;DeviceId=FaisterNodeMCU;SharedAccessKey=lYn3o0OUYjSmCz4z9pp3Nw==";
            //    "HostName=FaisterRemote.azure-devices.net;DeviceId=FaisterSensorTag;SharedAccessKey=rFoihjoARP7Ve+Oplflqmw==";
        private static int MESSAGE_COUNT = 5;

        // Fai
        private const string deviceID = "34242880-43b5-49c7-ad65-fe2c17f922d1"; //"ThePondsGenerator"; //"IgniteTestDevice"; //"FaisterNodeMCU"; // "FaisterNodeMCU";//"FaisterSensorTag";
        private const double latitude = 16.509264; //-33.53051;  //25.038182; //-27.998892; //-28.026678;  //-28.028164; //-33.857098; //-33.852242; //-33.699871;
        private const double longitude = -151.73616; // 151.212646; //121.568994; //153.431824; //153.435837; //153.42865; // 151.210739; //150.912292;

        static void Main(string[] args)
        {
            try
            {
                DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Amqp_WebSocket);

                if (deviceClient == null)
                {
                    Console.WriteLine("Failed to create DeviceClient!");
                }
                else
                {
                    UpdateDeviceInfo(deviceClient).Wait();
                    SendEvent(deviceClient).Wait();
                    ReceiveCommands(deviceClient).Wait();
                }

                Console.WriteLine("Exited!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in sample: {0}", ex.Message);
            }
        }

        static async Task SendEvent(DeviceClient deviceClient)
        {
            string dataBuffer;

            Console.WriteLine("Device sending {0} messages to IoTHub...\n", MESSAGE_COUNT);

            for (int count = 0; count < MESSAGE_COUNT; count++)
            {
                dataBuffer = Guid.NewGuid().ToString();
                Message eventMessage = new Message(Encoding.UTF8.GetBytes(dataBuffer));
                Console.WriteLine("\t{0}> Sending message: {1}, Data: [{2}]", DateTime.Now.ToLocalTime(), count, dataBuffer);

                await deviceClient.SendEventAsync(eventMessage);
            }
        }

        // Fai: to send DeviceInfo update to remote monitoring solution
        static async Task UpdateDeviceInfo(DeviceClient deviceClient)
        {
            string createdDateTime = DateTime.UtcNow.ToString("o");
            //string deviceInfo = "{\"DeviceProperties\":{\"DeviceID\":\"" + deviceID + "\",\"HubEnabledState\":true,\"CreatedTime\":\"" + createdDateTime + "\",\"DeviceState\":\"normal\",\"UpdatedTime\":null,\"Manufacturer\":\"Texas Instruments\",\"ModelNumber\":\"CC2541\",\"SerialNumber\":\"n/a\",\"FirmwareVersion\":\"Rev. B\",\"Platform\":\"Windows Phone 8.1\",\"Processor\":\"TPS62730\",\"InstalledRAM\":\"n/a\",\"Latitude\":" + latitude.ToString() + ",\"Longitude\":" + longitude.ToString() + "},\"Commands\":[{\"Name\":\"StartTelemetry\",\"Parameters\":null},{\"Name\":\"StopTelemetry\",\"Parameters\":null}]},{\"Name\":\"DiagnosticTelemetry\",\"Parameters\":[{\"Name\":\"Active\",\"Type\":\"boolean\"}]},{\"Name\":\"ChangeDeviceState\",\"Parameters\":[{\"Name\":\"DeviceState\",\"Type\":\"string\"}]}],\"CommandHistory\":[],\"IsSimulatedDevice\":true,\"Version\":\"1.0\",\"ObjectType\":\"DeviceInfo\"}";
            // FaisterSensorTag string deviceInfo = "{\"DeviceProperties\":{\"DeviceID\":\"" + deviceID + "\",\"HubEnabledState\":true,\"CreatedTime\":\"" + createdDateTime + "\",\"DeviceState\":\"normal\",\"UpdatedTime\":null,\"Manufacturer\":\"Texas Instruments\",\"ModelNumber\":\"CC2541\",\"SerialNumber\":\"n/a\",\"FirmwareVersion\":\"Rev. B\",\"Platform\":\"Windows Phone 8.1\",\"Processor\":\"n/a\",\"InstalledRAM\":\"n/a\",\"Latitude\":" + latitude.ToString() + ",\"Longitude\":" + longitude.ToString() + "},\"Commands\":[{\"Name\":\"PingDevice\",\"Parameters\":null},{\"Name\":\"StartTelemetry\",\"Parameters\":null},{\"Name\":\"StopTelemetry\",\"Parameters\":null},{\"Name\":\"ChangeSetPointTemp\",\"Parameters\":[{\"Name\":\"SetPointTemp\",\"Type\":\"double\"}]},{\"Name\":\"DiagnosticTelemetry\",\"Parameters\":[{\"Name\":\"Active\",\"Type\":\"boolean\"}]},{\"Name\":\"ChangeDeviceState\",\"Parameters\":[{\"Name\":\"DeviceState\",\"Type\":\"string\"}]}],\"CommandHistory\":[],\"IsSimulatedDevice\":true,\"Version\":\"1.0\",\"ObjectType\":\"DeviceInfo\"}";

            // Device Info for NodeMCU
            string deviceInfo = "{\"DeviceProperties\":{\"DeviceID\":\"" + deviceID + "\",\"HubEnabledState\":true,\"CreatedTime\":\"" + createdDateTime + "\",\"DeviceState\":\"normal\",\"UpdatedTime\":null,\"Manufacturer\":\"ESP8266 Opensource Community\",\"ModelNumber\":\"ESP8266\",\"SerialNumber\":\"n/a\",\"FirmwareVersion\":\"1.0 (ESP-12E Module)\",\"Platform\":\"XTOS\",\"Processor\":\"ESP8266(LX106)\",\"InstalledRAM\":\"1,044,464 bytes\",\"Latitude\":" + latitude.ToString() + ",\"Longitude\":" + longitude.ToString() + "},\"Commands\":[{\"Name\":\"PingDevice\",\"Parameters\":null},{\"Name\":\"StartTelemetry\",\"Parameters\":null},{\"Name\":\"StopTelemetry\",\"Parameters\":null},{\"Name\":\"ChangeSetPointTemp\",\"Parameters\":[{\"Name\":\"SetPointTemp\",\"Type\":\"double\"}]},{\"Name\":\"DiagnosticTelemetry\",\"Parameters\":[{\"Name\":\"Active\",\"Type\":\"boolean\"}]},{\"Name\":\"ChangeDeviceState\",\"Parameters\":[{\"Name\":\"DeviceState\",\"Type\":\"string\"}]}],\"CommandHistory\":[],\"IsSimulatedDevice\":false,\"Version\":\"1.0\",\"ObjectType\":\"DeviceInfo\"}";

            //string deviceInfo = "{\"DeviceProperties\":{\"DeviceID\":\"" + deviceID + "\",\"HubEnabledState\":true,\"CreatedTime\":\"" + createdDateTime + "\",\"DeviceState\":\"normal\",\"UpdatedTime\":null,\"Manufacturer\":\"Azure IoT RedCarpet\",\"ModelNumber\":\"RedCarpet\",\"SerialNumber\":\"n/a\",\"FirmwareVersion\":\"1.0\",\"Platform\":\"Windows\",\"Processor\":\"ARM Cortex-A53\",\"InstalledRAM\":\"1 GB\",\"Latitude\":" + latitude.ToString() + ",\"Longitude\":" + longitude.ToString() + "},\"Commands\":[{\"Name\":\"PingDevice\",\"Parameters\":null},{\"Name\":\"StartTelemetry\",\"Parameters\":null},{\"Name\":\"StopTelemetry\",\"Parameters\":null},{\"Name\":\"ChangeSetPointTemp\",\"Parameters\":[{\"Name\":\"SetPointTemp\",\"Type\":\"double\"}]},{\"Name\":\"DiagnosticTelemetry\",\"Parameters\":[{\"Name\":\"Active\",\"Type\":\"boolean\"}]},{\"Name\":\"ChangeDeviceState\",\"Parameters\":[{\"Name\":\"DeviceState\",\"Type\":\"string\"}]}],\"CommandHistory\":[],\"IsSimulatedDevice\":false,\"Version\":\"1.0\",\"ObjectType\":\"DeviceInfo\"}";

            // Device Info for Unit Test
            //string deviceInfo = "{\"DeviceProperties\":{\"DeviceID\":\"" + deviceID + "\",\"HubEnabledState\":true,\"CreatedTime\":\"" + createdDateTime + "\",\"DeviceState\":\"normal\",\"UpdatedTime\":null,\"Manufacturer\":\"Microsoft Corp\",\"ModelNumber\":\"Surface Pro 3\",\"SerialNumber\":\"n/a\",\"FirmwareVersion\":\"n/a\",\"Platform\":\"Windows 10 Enterprise\",\"Processor\":\"Intel Core i7-4650U CPU\",\"InstalledRAM\":\"8 GB\",\"Latitude\":" + latitude.ToString() + ",\"Longitude\":" + longitude.ToString() + "},\"Commands\":[{\"Name\":\"PingDevice\",\"Parameters\":null},{\"Name\":\"StartTelemetry\",\"Parameters\":null},{\"Name\":\"StopTelemetry\",\"Parameters\":null},{\"Name\":\"ChangeSetPointTemp\",\"Parameters\":[{\"Name\":\"SetPointTemp\",\"Type\":\"double\"}]},{\"Name\":\"DiagnosticTelemetry\",\"Parameters\":[{\"Name\":\"Active\",\"Type\":\"boolean\"}]},{\"Name\":\"ChangeDeviceState\",\"Parameters\":[{\"Name\":\"DeviceState\",\"Type\":\"string\"}]}],\"CommandHistory\":[],\"IsSimulatedDevice\":false,\"Version\":\"1.0\",\"ObjectType\":\"DeviceInfo\"}";

            // Device Info for MS Band
            //string deviceInfo = "{\"DeviceProperties\":{\"DeviceID\":\"" + deviceID + "\",\"HubEnabledState\":true,\"CreatedTime\":\"" + createdDateTime + "\",\"DeviceState\":\"normal\",\"UpdatedTime\":null,\"Manufacturer\":\"Microsoft Corp\",\"ModelNumber\":\"Band - Model 1619\",\"SerialNumber\":\"007139151249\",\"FirmwareVersion\":\"10.3.3304.0 09 R\",\"Platform\":\"Windows Phone 8.1\",\"Processor\":\"ARM\",\"InstalledRAM\":\"4GB\",\"Latitude\":" + latitude.ToString() + ",\"Longitude\":" + longitude.ToString() + "},\"Commands\":[{\"Name\":\"PingDevice\",\"Parameters\":null},{\"Name\":\"StartTelemetry\",\"Parameters\":null},{\"Name\":\"StopTelemetry\",\"Parameters\":null},{\"Name\":\"ChangeSetPointTemp\",\"Parameters\":[{\"Name\":\"SetPointTemp\",\"Type\":\"double\"}]},{\"Name\":\"DiagnosticTelemetry\",\"Parameters\":[{\"Name\":\"Active\",\"Type\":\"boolean\"}]},{\"Name\":\"ChangeDeviceState\",\"Parameters\":[{\"Name\":\"DeviceState\",\"Type\":\"string\"}]}],\"CommandHistory\":[],\"IsSimulatedDevice\":false,\"Version\":\"1.0\",\"ObjectType\":\"DeviceInfo\"}";

            // Device Info for RPi
            //string deviceInfo = "{\"DeviceProperties\":{\"DeviceID\":\"" + deviceID + "\",\"HubEnabledState\":true,\"CreatedTime\":\"" + createdDateTime + "\",\"DeviceState\":\"normal\",\"UpdatedTime\":null,\"Manufacturer\":\"Nexcom\",\"ModelNumber\":\"Nise 50\",\"SerialNumber\":\"n/a\",\"FirmwareVersion\":\"10.0.10556\",\"Platform\":\"Windows\",\"Processor\":\"Intel\",\"InstalledRAM\":\"1 GB\",\"Latitude\":" + latitude.ToString() + ",\"Longitude\":" + longitude.ToString() + "},\"Commands\":[{\"Name\":\"PingDevice\",\"Parameters\":null},{\"Name\":\"StartTelemetry\",\"Parameters\":null},{\"Name\":\"StopTelemetry\",\"Parameters\":null},{\"Name\":\"ChangeSetPointTemp\",\"Parameters\":[{\"Name\":\"SetPointTemp\",\"Type\":\"double\"}]},{\"Name\":\"DiagnosticTelemetry\",\"Parameters\":[{\"Name\":\"Active\",\"Type\":\"boolean\"}]},{\"Name\":\"ChangeDeviceState\",\"Parameters\":[{\"Name\":\"DeviceState\",\"Type\":\"string\"}]}],\"CommandHistory\":[],\"IsSimulatedDevice\":false,\"Version\":\"1.0\",\"ObjectType\":\"DeviceInfo\"}";



            Console.WriteLine("Updating device info {0} messages to IoTHub...\n", MESSAGE_COUNT);

            Message eventMessage = new Message(Encoding.UTF8.GetBytes(deviceInfo));
            Console.WriteLine("\t{0}> Sending Device Info: Data: [{1}]", DateTime.Now.ToLocalTime(), deviceInfo);

            await deviceClient.SendEventAsync(eventMessage);
            
        }


        static async Task ReceiveCommands(DeviceClient deviceClient)
        {
            Console.WriteLine("\nDevice waiting for commands from IoTHub...\n");
            Message receivedMessage;
            string messageData;

            while (true)
            {
                receivedMessage = await deviceClient.ReceiveAsync(TimeSpan.FromSeconds(1));
                
                if (receivedMessage != null)
                {
                    messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    Console.WriteLine("\t{0}> Received message: {1}", DateTime.Now.ToLocalTime(), messageData);

                    await deviceClient.CompleteAsync(receivedMessage);
                }
            }
        }
    }
}
