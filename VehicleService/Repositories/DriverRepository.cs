using Microsoft.EntityFrameworkCore;
using VehicleService.Data;
using VehicleService.Models;
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
        // AsNoTracking is used here because we don't need tracking for read operations
        return await _context.Drivers.AsNoTracking().ToListAsync();
    }

    public async Task<Driver> GetDriverByIdAsync(int driverId)
    {
        // AsNoTracking is useful here to avoid tracking issues when fetching the driver for reading purposes
        return await _context.Drivers.AsNoTracking().Include(d => d.Vehicles).FirstOrDefaultAsync(d => d.DriverId == driverId);
    }

    public async Task AddDriverAsync(Driver driver)
    {
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateDriverAsync(Driver driver)
    {
        // Get the existing driver including the vehicles
        var existingDriver = await _context.Drivers
            .Include(d => d.Vehicles)  // Include related vehicles to handle them as well
            .FirstOrDefaultAsync(d => d.DriverId == driver.DriverId);

        if (existingDriver != null)
        {
            // Detach the existing driver and its related entities (like vehicles)
            _context.Entry(existingDriver).State = EntityState.Detached;

            foreach (var vehicle in existingDriver.Vehicles)
            {
                _context.Entry(vehicle).State = EntityState.Detached;
            }
        }

        // Now update the incoming driver and related vehicles
        _context.Drivers.Update(driver);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteDriverAsync(int driverId)
    {
        var driver = await _context.Drivers.Include(d => d.Vehicles).FirstOrDefaultAsync(d => d.DriverId == driverId);
        if (driver != null)
        {
            // Remove the association between vehicles and the driver
            foreach (var vehicle in driver.Vehicles)
            {
                vehicle.DriverId = null;  // or assign a different driver if needed
            }

            await _context.SaveChangesAsync();

            // Now delete the driver
            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();
        }
    }
}
