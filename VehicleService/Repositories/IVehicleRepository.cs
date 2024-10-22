using SharedModels.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VehicleService.Repositories
{
    public interface IVehicleRepository
    {
        Task<IEnumerable<Vehicle>> GetVehiclesAsync();
        Task<Vehicle> GetVehicleByIdAsync(int id);
        Task AddVehicleAsync(Vehicle vehicle);
        Task UpdateVehicleAsync(Vehicle vehicle);
        Task AssignDriverToVehicleAsync(int vehicleId, int driverId);
        Task<int> GetTotalVehiclesCountAsync();  
        Task<IEnumerable<Vehicle>> GetVehiclesPaginatedAsync(int page, int pageSize);  
    }
}

