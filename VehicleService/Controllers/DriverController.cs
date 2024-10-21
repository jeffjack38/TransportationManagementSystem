using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedModels.Models;
using VehicleService.DTOs;
using VehicleService.Repositories;

namespace VehicleService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriverController : ControllerBase
    {
        private readonly IDriverRepository _driverRepository;
        private readonly IVehicleRepository _vehicleRepository;



        public DriverController(IDriverRepository driverRepository, IVehicleRepository vehicleRepository)
        {
            _driverRepository = driverRepository;
            _vehicleRepository = vehicleRepository;
        }

        // GET /api/drivers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DriverDTO>>> GetDrivers()
        {
            var drivers = await _driverRepository.GetDriversAsync();

            // Map Driver to DriverDTO
            var driverDTOs = drivers.Select(driver => new DriverDTO
            {
                DriverId = driver.DriverId,
                Name = driver.Name,
                LicenseNumber = driver.LicenseNumber,
                VehicleId = driver.Vehicles.FirstOrDefault()?.VehicleId ?? 0 // Assuming a driver can have multiple vehicles
            }).ToList();

            return Ok(driverDTOs);
        }

        // GET /api/drivers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Driver>> GetDriver(int id)
        {
            var driver = await _driverRepository.GetDriversByIdAsync(id); // Consistent method name
            if (driver == null)
            {
                return NotFound();
            }

            return Ok(driver);
        }

        // POST /api/driver
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CreateDriver([FromBody] DriverDTO driverDTO)
        {
            var driver = new Driver
            {
                Name = driverDTO.Name,
                LicenseNumber = driverDTO.LicenseNumber,
                Vehicles = new List<Vehicle>()
            };

            // Check if a vehicleId is provided and valid
            if (driverDTO.VehicleId.HasValue)
            {
                var vehicle = await _vehicleRepository.GetVehicleByIdAsync(driverDTO.VehicleId.Value);
                if (vehicle != null)
                {
                    driver.Vehicles.Add(vehicle);
                }
                else
                {
                    return BadRequest($"Vehicle with ID {driverDTO.VehicleId.Value} not found.");
                }
            }

            await _driverRepository.AddDriverAsync(driver);

            return CreatedAtAction(nameof(CreateDriver), new { id = driver.DriverId }, driverDTO);
        }



        // PUT /api/drivers/{id} (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDriver(int id, [FromBody] Driver driver)
        {
            if (id != driver.DriverId || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingDriver = await _driverRepository.GetDriversByIdAsync(id);
            if (existingDriver == null)
            {
                return NotFound();
            }

            await _driverRepository.UpdateDriverAsync(driver);
            return Ok("Driver updated successfully.");
        }

        // DELETE /api/drivers/{id} (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            var driver = await _driverRepository.GetDriversByIdAsync(id);
            if (driver == null)
            {
                return NotFound();
            }

            await _driverRepository.DeleteDriverAsync(id);
            return NoContent();
        }
    }
}
