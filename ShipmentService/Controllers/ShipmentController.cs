using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipmentService.DTOs;
using ShipmentService.Models;
using ShipmentService.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // GET /api/shipments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipmentDTO>>> GetShipments([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var shipments = await _shipmentRepository.GetShipmentsPaginatedAsync(page, pageSize);

            var shipmentDTOs = shipments.Select(s => new ShipmentDTO
            {
                ShipmentId = s.ShipmentId,
                Origin = s.Origin,
                Destination = s.Destination,
                ShipDate = s.ShipDate,
                Status = s.Status
            });

            // Optionally include total count
            var totalCount = await _shipmentRepository.GetTotalShipmentsCountAsync();

            return Ok(new { totalCount, shipmentDTOs });
        }


        // POST /api/shipments
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateShipment([FromBody] ShipmentDTO shipmentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shipment = new Shipment
            {
                Origin = shipmentDTO.Origin,
                Destination = shipmentDTO.Destination,
                ShipDate = shipmentDTO.ShipDate,
                Status = shipmentDTO.Status
            };

            await _shipmentRepository.AddShipmentAsync(shipment);
            return CreatedAtAction(nameof(GetShipmentById), new { id = shipment.ShipmentId }, shipmentDTO);
        }

        // GET /api/shipments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ShipmentDTO>> GetShipmentById(int id)
        {
            var shipment = await _shipmentRepository.GetShipmentByIdAsync(id);

            if (shipment == null)
            {
                return NotFound();
            }

            var shipmentDTO = new ShipmentDTO
            {
                ShipmentId = shipment.ShipmentId,
                Origin = shipment.Origin,
                Destination = shipment.Destination,
                ShipDate = shipment.ShipDate,
                Status = shipment.Status
            };

            return Ok(shipmentDTO);
        }

        // PUT /api/shipments/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShipment(int id, [FromBody] ShipmentDTO shipmentDTO)
        {
            if (id != shipmentDTO.ShipmentId || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shipment = new Shipment
            {
                ShipmentId = shipmentDTO.ShipmentId,
                Origin = shipmentDTO.Origin,
                Destination = shipmentDTO.Destination,
                ShipDate = shipmentDTO.ShipDate,
                Status = shipmentDTO.Status
            };

            await _shipmentRepository.UpdateShipmentAsync(shipment);
            return NoContent();
        }
    }
}

