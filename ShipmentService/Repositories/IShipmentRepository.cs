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
        Task<IEnumerable<Shipment>> GetShipmentsAsync();   // Retrieve all shipments
        Task<IEnumerable<Shipment>> GetShipmentsPaginatedAsync(int page, int pageSize);  // method for pagination
        Task<int> GetTotalShipmentsCountAsync();  // New method to get total count of shipments



    }
}

