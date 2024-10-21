using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SharedModels.Models
{
    public class Vehicle
    {
        public int VehicleId { get; set; }

        [Required]  // Will create a NOT NULL column in the database
        [StringLength(50)]  // The maximum length of the 'Make' field
        public string Make { get; set; }

        [Required]  // Will create a NOT NULL column in the database
        [StringLength(50)]  // The maximum length of the 'Model' field
        public string Model { get; set; }

        [Range(1886, 2100)]  // The valid range for the 'Year' field
        public int Year { get; set; }

        public int? DriverId { get; set; }  // Nullable in case a vehicle has no driver assigned yet

        [JsonIgnore]
        public Driver? Driver { get; set; }  // Navigation property
    }
}


