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

        // GET /api/driver
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DriverDTO>>> GetDrivers()
        {
            var drivers = await _driverRepository.GetDriversAsync();

            
            var driverDTOs = drivers.Select(driver => new DriverDTO
            {
                DriverId = driver.DriverId,
                Name = driver.Name,
                LicenseNumber = driver.LicenseNumber,
                VehicleId = driver.Vehicles.FirstOrDefault()?.VehicleId ?? 0 
            }).ToList();

            return Ok(driverDTOs);
        }

        // GET /api/driver/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Driver>> GetDriver(int id)
        {
            var driver = await _driverRepository.GetDriversByIdAsync(id); 
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



        // PUT /api/driver/{id} 
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDriver(int id, [FromBody] DriverDTO driverDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            if (driverDTO.Vehicles != null && driverDTO.Vehicles.Count == 0)
            {
                return BadRequest("At least one vehicle must be provided.");
            }

            
            var driver = await _driverRepository.GetDriversByIdAsync(id);
            if (driver == null)
            {
                return NotFound();
            }

            driver.Name = driverDTO.Name;
            driver.LicenseNumber = driverDTO.LicenseNumber;

            
            if (driverDTO.Vehicles != null)
            {
               
            }

            await _driverRepository.UpdateDriverAsync(driver);
            return Ok("Driver updated successfully.");
        }


        // DELETE /api/driver/{id} 
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
