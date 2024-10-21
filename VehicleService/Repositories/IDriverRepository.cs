using System.Collections.Generic;
using System.Threading.Tasks;
using SharedModels.Models;

namespace VehicleService.Repositories
{
    public interface IDriverRepository
    {
        Task<IEnumerable<Driver>> GetDriversAsync();
        Task<IEnumerable<Driver>> GetDriversByUserIdAsync(string userId);
        Task<Driver> GetDriversByIdAsync(int driverId);  
        Task AddDriverAsync(Driver driver);
        Task UpdateDriverAsync(Driver driver);
        Task DeleteDriverAsync(int driverId);
    }

}


