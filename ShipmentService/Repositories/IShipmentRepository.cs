using System.Threading.Tasks;
using ShipmentService.Models;

namespace ShipmentService.Repositories
{
    public interface IShipmentRepository
    {
        Task AddShipmentAsync(Shipment shipment);          // Add a new shipment
        Task UpdateShipmentAsync(Shipment shipment);       // Update an existing shipment
        Task UpdateShipmentStatusAsync(int shipmentId, string status);  // Update the status of a shipment
        Task<Shipment> GetShipmentByIdAsync(int shipmentId);   // Retrieve a shipment by its ID
    }
}

