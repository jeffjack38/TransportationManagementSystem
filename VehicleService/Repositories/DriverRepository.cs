using Microsoft.EntityFrameworkCore;
using VehicleService.Data;
using SharedModels.Models;
using VehicleService.Repositories;

public class DriverRepository : IDriverRepository
{
    private readonly VehicleDbContext _context;

    public DriverRepository(VehicleDbContext context)
    {
        _context = context;
    }

    
    public async Task<IEnumerable<Driver>> GetDriversAsync()
    {
        return await _context.Drivers
                             .Include(d => d.Vehicles)
                             .AsNoTracking()
                             .ToListAsync();
    }

    
    public async Task<IEnumerable<Driver>> GetDriversByUserIdAsync(string userId)
    {
        return await _context.Drivers
            .Where(d => d.UserId == userId)
            .ToListAsync();
    }

    
    public async Task<Driver> GetDriversByIdAsync(int driverId)
    {
        return await _context.Drivers
            .Include(d => d.Vehicles)  
            .FirstOrDefaultAsync(d => d.DriverId == driverId);
    }

    
    public async Task AddDriverAsync(Driver driver)
    {
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();
    }

    
    public async Task UpdateDriverAsync(Driver driver)
    {
        var existingDriver = await _context.Drivers
            .Include(d => d.Vehicles)
            .FirstOrDefaultAsync(d => d.DriverId == driver.DriverId);

        if (existingDriver != null)
        {
            _context.Entry(existingDriver).State = EntityState.Detached;
            foreach (var vehicle in existingDriver.Vehicles)
            {
                _context.Entry(vehicle).State = EntityState.Detached;
            }
        }

        _context.Drivers.Update(driver);
        await _context.SaveChangesAsync();
    }

    
    public async Task DeleteDriverAsync(int driverId)
    {
        var driver = await _context.Drivers.Include(d => d.Vehicles).FirstOrDefaultAsync(d => d.DriverId == driverId);
        if (driver != null)
        {
            foreach (var vehicle in driver.Vehicles)
            {
                vehicle.DriverId = null;
            }

            await _context.SaveChangesAsync();
            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();
        }
    }
}
