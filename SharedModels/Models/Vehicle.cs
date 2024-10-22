using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SharedModels.Models
{
    public class Vehicle
    {
        public int VehicleId { get; set; }

        [Required] 
        [StringLength(50)]  
        public string Make { get; set; }

        [Required]  
        [StringLength(50)]  
        public string Model { get; set; }

        [Range(1886, 2100)]  
        public int Year { get; set; }

        public int? DriverId { get; set; } 

        [JsonIgnore]
        public Driver? Driver { get; set; }  
    }
}


