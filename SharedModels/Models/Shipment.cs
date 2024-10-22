using System;
using System.ComponentModel.DataAnnotations;

namespace SharedModels.Models
{
    public class Shipment
    {
        [Key]  // Primary Key
        public int ShipmentId { get; set; }  // Primary Key

        [Required]
        [StringLength(100)]  // Database constraint
        public string Origin { get; set; }  // The place the shipment is coming from

        [Required]
        [StringLength(100)]  // Database constraint
        public string Destination { get; set; }  // The place the shipment is going to

        [Required]
        public DateTime ShipDate { get; set; }  // Date when the shipment is scheduled

        [Required]
        [StringLength(20)]  // Database constraint
        public string Status { get; set; }  // Status of the shipment, e.g., Pending, Shipped, Delivered

        // Navigation property for bookings associated with this shipment
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}

