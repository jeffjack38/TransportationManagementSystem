using System;
using System.ComponentModel.DataAnnotations;

namespace SharedModels.Models
{
    public class Booking
    {
        [Key]  // Marks this as the primary key
        public int BookingId { get; set; }

        [Required]  // ShipmentId is required
        public int ShipmentId { get; set; }

        [Required]  // CustomerName is required
        [StringLength(100)]  // Limits the CustomerName to 100 characters
        public string CustomerName { get; set; }

        [Required]  // BookingDate is required
        public DateTime BookingDate { get; set; }

        [Required]  // Status is required
        [StringLength(50)]  // Limits the Status to 50 characters
        public string Status { get; set; }

        // Associate this booking with a user (customer)
        public string? UserId { get; set; }

    }
}


