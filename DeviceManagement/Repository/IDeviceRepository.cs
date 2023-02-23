using DeviceManagement.Models;

namespace DeviceManagement.Repository
{
    public interface IDeviceRepository
    {
        Task<string> AddDeviceAsync(string deviceId);
        Task<List<DeviceModel>> GetDevicesAsync();
        Task<DeviceModel> UpdateDeviceAsync(DeviceModel deviceModel);
        Task<bool> DeleteDeviceAsync(string deviceId);
        Task<Object> UpdateDesiredProperties(string deviceId, Properties properties);
        Task<Object> UpdateReportedProperties(Properties properties);
        Task<bool> SendTelemetryMessages();
    }
}
