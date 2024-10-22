using Microsoft.AspNetCore.Mvc;
using VehicleService.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedModels.Models;
using VehicleService.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace VehicleService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehicleController(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        // GET /api/vehicle
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehicleDTO>>> GetVehicles([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var vehicles = await _vehicleRepository.GetVehiclesPaginatedAsync(page, pageSize);

            var vehicleDTOs = vehicles.Select(v => new VehicleDTO
            {
                VehicleId = v.VehicleId,
                Make = v.Make,
                Model = v.Model,
                Year = v.Year,
                DriverId = v.DriverId
            });

            
            var totalCount = await _vehicleRepository.GetTotalVehiclesCountAsync();

            return Ok(new { totalCount, vehicleDTOs });
        }

        // GET /api/vehicle/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleDTO>> GetVehicle(int id)
        {
            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(id);

            if (vehicle == null)
            {
                return NotFound();
            }

            var vehicleDTO = new VehicleDTO
            {
                VehicleId = vehicle.VehicleId,
                Make = vehicle.Make,
                Model = vehicle.Model,
                Year = vehicle.Year,
                DriverId = vehicle.DriverId
            };

            return Ok(vehicleDTO);
        }

        // POST /api/vehicle
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CreateVehicle([FromBody] VehicleDTO vehicleDTO)
        {
            var vehicle = new Vehicle
            {
                Make = vehicleDTO.Make,
                Model = vehicleDTO.Model,
                Year = vehicleDTO.Year,
                DriverId = vehicleDTO.DriverId
            };

            await _vehicleRepository.AddVehicleAsync(vehicle);
            return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.VehicleId }, vehicleDTO);
        }

        // PUT /api/vehicle/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] VehicleDTO vehicleDTO)
        {
            if (id != vehicleDTO.VehicleId)
            {
                return BadRequest("Vehicle ID mismatch");
            }

            var existingVehicle = await _vehicleRepository.GetVehicleByIdAsync(id);
            if (existingVehicle == null)
            {
                return NotFound("Vehicle not found");
            }

            var vehicle = new Vehicle
            {
                VehicleId = vehicleDTO.VehicleId,
                Make = vehicleDTO.Make,
                Model = vehicleDTO.Model,
                Year = vehicleDTO.Year,
                DriverId = vehicleDTO.DriverId
            };

            await _vehicleRepository.UpdateVehicleAsync(vehicle);
            return Ok(new { message = "Vehicle updated successfully!", vehicleDTO });
        }

        // PUT /api/vehicle/{id}/assign
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/assign")]
        public async Task<IActionResult> AssignDriver(int id, [FromBody] int driverId)
        {
            await _vehicleRepository.AssignDriverToVehicleAsync(id, driverId);
            return Ok(new {message = "Assigned driver success!"});
        }
    }
}
