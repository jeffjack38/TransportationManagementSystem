using Microsoft.AspNetCore.Mvc;
using ShipmentService.Models;
using ShipmentService.Repositories;

namespace ShipmentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentController : ControllerBase
    {
        private readonly IShipmentRepository _shipmentRepository;

        public ShipmentController(IShipmentRepository shipmentRepository)
        {
            _shipmentRepository = shipmentRepository;
        }

        // POST /api/shipments
        [HttpPost]
        public async Task<IActionResult> CreateShipment([FromBody] Shipment shipment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _shipmentRepository.AddShipmentAsync(shipment);
            return CreatedAtAction(nameof(GetShipmentById), new { id = shipment.ShipmentId }, shipment);
        }

        // PUT /api/shipments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShipment(int id, [FromBody] Shipment shipment)
        {
            if (id != shipment.ShipmentId || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingShipment = await _shipmentRepository.GetShipmentByIdAsync(id);
            if (existingShipment == null)
            {
                return NotFound();
            }

            await _shipmentRepository.UpdateShipmentAsync(shipment);
            return NoContent();
        }

        // PUT /api/shipments/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateShipmentStatus(int id, [FromBody] string status)
        {
            var shipment = await _shipmentRepository.GetShipmentByIdAsync(id);
            if (shipment == null)
            {
                return NotFound();
            }

            await _shipmentRepository.UpdateShipmentStatusAsync(id, status);  // Pass both id and status
            return NoContent();
        }

        // GET /api/shipments/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetShipmentById(int id)
        {
            var shipment = await _shipmentRepository.GetShipmentByIdAsync(id);
            if (shipment == null)
            {
                return NotFound();
            }

            return Ok(shipment);
        }
    }

}
