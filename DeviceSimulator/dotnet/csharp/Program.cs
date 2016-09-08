﻿using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

using Azure.IoTHub.Examples.CSharp.Core;

namespace DeviceSimulator
{
    class Program
    {
        static DeviceClient deviceClient;

        private static async void SendDeviceToCloudMessagesAsync(string deviceId)
        {
            double avgWindSpeed = 10; // m/s
            Random rand = new Random();

            while (true)
            {
                double currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;

                var telemetryDataPoint = new
                {
                    deviceId = deviceId,
                    windSpeed = currentWindSpeed
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                Task.Delay(1000).Wait();
            }
        }

        static void Main(string[] args)
        {
            var config = @"config.yaml".GetIoTConfiguration().AzureIoTHubConfig;

            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(config.IoTHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(config.DeviceId, config.DeviceKey));

            SendDeviceToCloudMessagesAsync(config.DeviceId);
            Console.ReadLine();
        }
    }
}
