using System.ComponentModel.DataAnnotations;

namespace VehicleService.DTOs
{
    public class VehicleDTO
    {
        public int VehicleId { get; set; }

        [Required(ErrorMessage = "Make is required.")]
        [StringLength(50, ErrorMessage = "Make cannot exceed 50 characters.")]
        public string Make { get; set; }

        [Required(ErrorMessage = "Model is required.")]
        [StringLength(50, ErrorMessage = "Model cannot exceed 50 characters.")]
        public string Model { get; set; }

        [Range(1886, 2100, ErrorMessage = "Year must be between 1886 and 2100.")]
        public int Year { get; set; }

        public int? DriverId { get; set; }
    }
}

