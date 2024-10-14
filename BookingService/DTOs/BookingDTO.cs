using System;
using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class BookingDTO
    {
        [Required(ErrorMessage = "ShipmentId is required.")]
        public int ShipmentId { get; set; }

        [Required(ErrorMessage = "CustomerName is required.")]
        [StringLength(100, ErrorMessage = "CustomerName cannot exceed 100 characters.")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "BookingDate is required.")]
        public DateTime BookingDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; }
    }
}

