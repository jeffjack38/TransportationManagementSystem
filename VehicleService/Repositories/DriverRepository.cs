using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleService.Data;
using VehicleService.Models;

namespace VehicleService.Repositories
{
    public class DriverRepository : IDriverRepository
    {
        private readonly VehicleDbContext _context;

        public DriverRepository(VehicleDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Driver>> GetDriversAsync()
        {
            return await _context.Drivers.ToListAsync();
        }

        public async Task<Driver> GetDriverByIdAsync(int driverId)
        {
            return await _context.Drivers.FindAsync(driverId);
        }

        public async Task AddDriverAsync(Driver driver)
        {
            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDriverAsync(Driver driver)
        {
            _context.Drivers.Update(driver);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDriverAsync(int driverId)
        {
            var driver = await _context.Drivers.FindAsync(driverId);
            if (driver != null)
            {
                _context.Drivers.Remove(driver);
                await _context.SaveChangesAsync();
            }
        }
    }
}
