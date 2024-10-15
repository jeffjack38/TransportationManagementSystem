using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleService.Models;
using VehicleService.Repositories;

namespace VehicleService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriverController : ControllerBase
    {
        private readonly IDriverRepository _driverRepository;

        public DriverController(IDriverRepository driverRepository)
        {
            _driverRepository = driverRepository;
        }

        // GET /api/drivers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Driver>>> GetDrivers()
        {
            var drivers = await _driverRepository.GetDriversAsync();
            return Ok(drivers);
        }

        // GET /api/drivers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Driver>> GetDriver(int id)
        {
            var driver = await _driverRepository.GetDriverByIdAsync(id);
            if (driver == null)
            {
                return NotFound();
            }

            return Ok(driver);
        }

        // POST /api/drivers (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CreateDriver([FromBody] Driver driver)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _driverRepository.AddDriverAsync(driver);
            return CreatedAtAction(nameof(GetDriver), new { id = driver.DriverId }, driver);
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

            var existingDriver = await _driverRepository.GetDriverByIdAsync(id);
            if (existingDriver == null)
            {
                return NotFound();
            }

            await _driverRepository.UpdateDriverAsync(driver);
            return NoContent();
        }

        // DELETE /api/drivers/{id} (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            var driver = await _driverRepository.GetDriverByIdAsync(id);
            if (driver == null)
            {
                return NotFound();
            }

            await _driverRepository.DeleteDriverAsync(id);
            return NoContent();
        }
    }
}
