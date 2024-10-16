﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ShipmentService.Models
{
    public class Shipment
    {
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
    }
}

