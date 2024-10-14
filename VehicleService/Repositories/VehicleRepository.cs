using VehicleService.Data;
using VehicleService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VehicleService.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly VehicleDbContext _context;

        public VehicleRepository(VehicleDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesAsync()
        {
            return await _context.Vehicles.Include(v => v.Driver).ToListAsync();
        }

        //method to get paginated vehicles
        public async Task<IEnumerable<Vehicle>> GetVehiclesPaginatedAsync(int page, int pageSize)
        {
            return await _context.Vehicles
                .Skip((page - 1) * pageSize)  // Skip the previous pages
                .Take(pageSize)  // Take only the page size number of records
                .ToListAsync();
        }

        //method to get the total count of vehicles
        public async Task<int> GetTotalVehiclesCountAsync()
        {
            return await _context.Vehicles.CountAsync();
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            return await _context.Vehicles.Include(v => v.Driver).FirstOrDefaultAsync(v => v.VehicleId == id);
        }

        public async Task AddVehicleAsync(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateVehicleAsync(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task AssignDriverToVehicleAsync(int vehicleId, int driverId)
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle != null)
            {
                vehicle.DriverId = driverId;
                await _context.SaveChangesAsync();
            }
        }
    }
}

