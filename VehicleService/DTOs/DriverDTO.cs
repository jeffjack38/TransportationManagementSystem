using System.ComponentModel.DataAnnotations;

namespace VehicleService.DTOs
{
    public class DriverDTO
    {
        public int DriverId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "License Number is required.")]
        [StringLength(15, ErrorMessage = "License Number cannot exceed 15 characters.")]
        public string LicenseNumber { get; set; }
        public int? VehicleId { get; set; }
    }
}

