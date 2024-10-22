using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SharedModels.Models
{
    public class Driver
    {
        public int DriverId { get; set; }

        [Required]  
        [StringLength(100)]  
        public string Name { get; set; }

        [Required]  
        [StringLength(15)]  
        public string LicenseNumber { get; set; }
    
        public string? UserId { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; }  
    }
}

