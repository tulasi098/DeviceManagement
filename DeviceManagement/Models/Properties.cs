using System.ComponentModel.DataAnnotations;

namespace DeviceManagement.Models
{
    public class Properties
    {
        [Required]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }   
    }
}
