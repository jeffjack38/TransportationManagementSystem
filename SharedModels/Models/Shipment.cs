using System;
using System.ComponentModel.DataAnnotations;

namespace SharedModels.Models
{
    public class Shipment
    {
        [Key]  
        public int ShipmentId { get; set; } 

        [Required]
        [StringLength(100)]  
        public string Origin { get; set; }  

        [Required]
        [StringLength(100)]  
        public string Destination { get; set; }  

        [Required]
        public DateTime ShipDate { get; set; }  

        [Required]
        [StringLength(20)]  
        public string Status { get; set; }  

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}

