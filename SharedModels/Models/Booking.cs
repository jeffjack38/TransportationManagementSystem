using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedModels.Models
{
    public class Booking
    {
        [Key]  
        public int BookingId { get; set; }

        [Required]  
        public int ShipmentId { get; set; }

        [ForeignKey("ShipmentId")]  
        public Shipment Shipment { get; set; }  

        [Required]  
        [StringLength(100)] 
        public string CustomerName { get; set; }

        [Required] 
        public DateTime BookingDate { get; set; }

        [Required] 
        [StringLength(50)]  
        public string Status { get; set; }

        public string? UserId { get; set; }

    }
}


