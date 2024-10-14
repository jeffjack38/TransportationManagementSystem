using System;
using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class BookingShipmentDTO
    {
        [Required(ErrorMessage = "BookingId is required.")]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "CustomerName is required.")]
        [StringLength(100, ErrorMessage = "CustomerName cannot exceed 100 characters.")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "BookingDate is required.")]
        public DateTime BookingDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; }

        // Shipment information
        [Required(ErrorMessage = "ShipmentId is required.")]
        public int ShipmentId { get; set; }

        [Required(ErrorMessage = "Origin is required.")]
        [StringLength(100, ErrorMessage = "Origin cannot exceed 100 characters.")]
        public string Origin { get; set; }

        [Required(ErrorMessage = "Destination is required.")]
        [StringLength(100, ErrorMessage = "Destination cannot exceed 100 characters.")]
        public string Destination { get; set; }

        [Required(ErrorMessage = "ShipDate is required.")]
        public DateTime ShipDate { get; set; }

        [Required(ErrorMessage = "ShipmentStatus is required.")]
        [StringLength(50, ErrorMessage = "ShipmentStatus cannot exceed 50 characters.")]
        public string ShipmentStatus { get; set; } // Shipment status (Available, Shipped, Delivered, etc.)
    }


}
