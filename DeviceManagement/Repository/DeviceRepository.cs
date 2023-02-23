using DeviceManagement.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Rest;
using Newtonsoft.Json;
using System.Text;

namespace DeviceManagement.Repository
{
    public class DeviceRepository:IDeviceRepository
    {
        private readonly string _ioTHubConnectionString;
        private readonly string _deviceConnectionString;
        public DeviceRepository(IConfiguration configuration)
        {
            _ioTHubConnectionString = configuration.GetValue<string>("IoTHubConnectionString");
            _deviceConnectionString = configuration.GetValue<string>("DeviceConnectionString");
        }
        public async Task<string> AddDeviceAsync(string deviceId)
        {
            RegistryManager _registryManager = RegistryManager.CreateFromConnectionString(_ioTHubConnectionString);

            var d = new Device(deviceId);

            var device = await _registryManager.AddDeviceAsync(d);

            return device.Authentication.SymmetricKey.PrimaryKey;

        }

        public async Task<bool> DeleteDeviceAsync(string deviceId)
        {
            RegistryManager _registryManager = RegistryManager.CreateFromConnectionString(_ioTHubConnectionString);

            Device device = new Device();

            device = await _registryManager.GetDeviceAsync(deviceId);
            if (device != null)
            {
                await _registryManager.RemoveDeviceAsync(device);
                return true;
            }
            else
                return false;
        }

        [Obsolete]
        public async Task<List<DeviceModel>> GetDevicesAsync()
        {
            RegistryManager _registryManager = RegistryManager.CreateFromConnectionString(_ioTHubConnectionString);

            var devicemodel = new List<DeviceModel>();

            var devices = await _registryManager.GetDevicesAsync(Int32.MaxValue);

            foreach (var device in devices)
            {
                devicemodel.Add(new DeviceModel()
                {
                    Id = device.Id,
                    Status = device.Status,
                    StatusReason = device.StatusReason
                });
            }
            return devicemodel;
        }

        public async Task<DeviceModel> UpdateDeviceAsync(DeviceModel deviceModel)
        {
            RegistryManager _registryManager = RegistryManager.CreateFromConnectionString(_ioTHubConnectionString);
            var deviceupdate = new Device();
            var device = await _registryManager.GetDeviceAsync(deviceModel.Id);

            if (device != null)
            {
                device.Status = deviceModel.Status;
                device.StatusReason = deviceModel.StatusReason;

                deviceupdate = await _registryManager.UpdateDeviceAsync(device);
            }
            return new DeviceModel()
            {
                Id = deviceupdate.Id,
                Status = deviceupdate.Status,
                StatusReason = deviceupdate.StatusReason
            };
        }

        public async Task<Object> UpdateDesiredProperties(string deviceId, Properties properties)
        {
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString(_ioTHubConnectionString);

            var twin = await registryManager.GetTwinAsync(deviceId);
            if (twin != null)
            {
                twin.Properties.Desired[properties.Key] = properties.Value;

                var response = await registryManager.UpdateTwinAsync(deviceId, twin, twin.ETag);

                return response.ToJson();
            }
            else
                return null;
            
        }

        public async Task<object> UpdateReportedProperties(Properties properties)
        {
            DeviceClient client = DeviceClient.CreateFromConnectionString(_deviceConnectionString, Microsoft.Azure.Devices.Client.TransportType.Mqtt);

            TwinCollection reportedProperties;

            reportedProperties = new TwinCollection();

            reportedProperties[properties.Key] = properties.Value;

            await client.UpdateReportedPropertiesAsync(reportedProperties);
            return reportedProperties;
        }

        public async Task<bool> SendTelemetryMessages()
        {
            DeviceClient client = DeviceClient.CreateFromConnectionString(_deviceConnectionString, Microsoft.Azure.Devices.Client.TransportType.Mqtt);
            double minTemperature = 20;
            double minHumidity = 60;
            Random rand = new Random();

            while (true)
            {
                double currentTemperature = minTemperature + rand.NextDouble() * 15;
                double currentHumidity = minHumidity + rand.NextDouble() * 20;

                var telemetryDataPoint = new
                {

                    temperature = currentTemperature,
                    humidity = currentHumidity
                };

                string messageString = "";

                messageString = JsonConvert.SerializeObject(telemetryDataPoint);

                var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(messageString));

                // Add a custom application property to the message.  
                // An IoT hub can filter on these properties without access to the message body.  
                //message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");  

                // Send the telemetry message  
                await client.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
                await Task.Delay(1000);
            }
        }
    }
}
