using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleService.Models;

namespace VehicleService.Repositories
{
    public interface IDriverRepository
    {
        Task<IEnumerable<Driver>> GetDriversAsync();
        Task<Driver> GetDriverByIdAsync(int driverId);
        Task AddDriverAsync(Driver driver);
        Task UpdateDriverAsync(Driver driver);
        Task DeleteDriverAsync(int driverId);
    }
}


