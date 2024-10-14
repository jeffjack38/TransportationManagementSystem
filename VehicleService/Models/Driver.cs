using System.ComponentModel.DataAnnotations;

namespace VehicleService.Models
{
    public class Driver
    {
        public int DriverId { get; set; }

        [Required]  // Will create a NOT NULL column in the database
        [StringLength(100)]  // The maximum length of the 'Name' field
        public string Name { get; set; }

        [Required]  // Will create a NOT NULL column in the database
        [StringLength(15)]  // The maximum length of the 'LicenseNumber' field
        public string LicenseNumber { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; }  // A driver can have multiple vehicles
    }
}

