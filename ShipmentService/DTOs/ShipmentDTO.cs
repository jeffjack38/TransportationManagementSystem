using System;
using System.ComponentModel.DataAnnotations;

namespace ShipmentService.DTOs
{
    public class ShipmentDTO
    {
        public int ShipmentId { get; set; }

        [Required(ErrorMessage = "Origin is required.")]
        [StringLength(100, ErrorMessage = "Origin cannot exceed 100 characters.")]
        public string Origin { get; set; }

        [Required(ErrorMessage = "Destination is required.")]
        [StringLength(100, ErrorMessage = "Destination cannot exceed 100 characters.")]
        public string Destination { get; set; }

        [Required(ErrorMessage = "ShipDate is required.")]
        public DateTime ShipDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
        public string Status { get; set; }
    }
}

