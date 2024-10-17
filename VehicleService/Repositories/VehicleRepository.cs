using Microsoft.EntityFrameworkCore;
using VehicleService.Data;
using VehicleService.Models;
using VehicleService.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly VehicleDbContext _context;

    public VehicleRepository(VehicleDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Vehicle>> GetVehiclesAsync()
    {
        return await _context.Vehicles.AsNoTracking().Include(v => v.Driver).ToListAsync();
    }

    public async Task<IEnumerable<Vehicle>> GetVehiclesPaginatedAsync(int page, int pageSize)
    {
        return await _context.Vehicles
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(v => v.Driver)
            .ToListAsync();
    }

    public async Task<int> GetTotalVehiclesCountAsync()
    {
        return await _context.Vehicles.CountAsync();
    }

    public async Task<Vehicle> GetVehicleByIdAsync(int id)
    {
        return await _context.Vehicles.AsNoTracking().Include(v => v.Driver).FirstOrDefaultAsync(v => v.VehicleId == id);
    }

    public async Task AddVehicleAsync(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateVehicleAsync(Vehicle vehicle)
    {
        var existingVehicle = await _context.Vehicles.Include(v => v.Driver).FirstOrDefaultAsync(v => v.VehicleId == vehicle.VehicleId);

        if (existingVehicle != null)
        {
            _context.Entry(existingVehicle).State = EntityState.Detached;  // Detach the tracked entity
        }

        _context.Vehicles.Update(vehicle);  // Attach and update the incoming entity
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
