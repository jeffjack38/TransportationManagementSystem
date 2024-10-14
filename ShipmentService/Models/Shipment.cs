namespace ShipmentService.Models
{
    public class Shipment
    {
        public int ShipmentId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime ShipDate { get; set; }
        public string Status { get; set; } // Pending, Shipped, Delivered, etc.
    }

}
