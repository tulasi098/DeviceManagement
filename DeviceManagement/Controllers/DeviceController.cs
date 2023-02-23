using DeviceManagement.Models;
using DeviceManagement.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceRepository _deviceRepository;
        public DeviceController(IDeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        [HttpGet("GetDevice", Name = "GetDevices")]
        public async Task<IActionResult> GetDevices()
        {
            try
            {
                var response = await _deviceRepository.GetDevicesAsync();
                if (response.Count == 0)
                {
                    return NotFound();
                }

                return Ok(response);               
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddDevice")]
        public async Task<IActionResult> AddDevice(string deviceId)
        {
            try
            {
                var response = await _deviceRepository.AddDeviceAsync(deviceId);

                if (response != null)
                {
                    return CreatedAtAction("GetDevices", response);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateDevice")]
        public async Task<IActionResult> UpdateDevice(DeviceModel deviceModel)
        {
            try
            {
                var response = await _deviceRepository.UpdateDeviceAsync(deviceModel);
                if (response != null)
                {
                    return Ok(response);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteDevice")]
        public async Task<IActionResult> DeleteDeviceAsync(string deviceId)
        {
            try
            {
                if (await _deviceRepository.DeleteDeviceAsync(deviceId))
                {
                    return NoContent();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateReportedProperties")]
        public async Task<IActionResult> ReportedProperties(Properties properties)
        {
            try
            {
                var response = await _deviceRepository.UpdateReportedProperties(properties);
                if (response == null)
                    return BadRequest();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("UpdateDesiredProperties")]
        public async Task<IActionResult> DesiredProperties(string deviceId,Properties properties)
        {
            try
            {
                var response = await _deviceRepository.UpdateDesiredProperties(deviceId, properties);
                if (response == null)
                    return BadRequest();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Telemetry")]
        public async Task SendTelemetryData()
        {
            await _deviceRepository.SendTelemetryMessages();
        }
    }
}
