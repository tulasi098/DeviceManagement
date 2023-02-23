using Microsoft.Azure.Devices;

namespace DeviceManagement.Models
{
    public class DeviceModel
    {
        public string? Id { get; set; }
        public DeviceStatus Status { get; set; }
        public string? StatusReason { get; set; }
    }
}
