using System.Threading.Tasks;
using SharedModels.Models;

namespace ShipmentService.Repositories
{
    public interface IShipmentRepository
    {
        Task AddShipmentAsync(Shipment shipment);          
        Task UpdateShipmentAsync(Shipment shipment);       
        Task UpdateShipmentStatusAsync(int shipmentId, string status);  
        Task<Shipment> GetShipmentByIdAsync(int shipmentId);   
        Task<IEnumerable<Shipment>> GetShipmentsAsync();   
        Task<IEnumerable<Shipment>> GetShipmentsPaginatedAsync(int page, int pageSize);  
        Task<int> GetTotalShipmentsCountAsync(); 
        Task DeleteShipmentAsync(int shipmentId);


    }
}

