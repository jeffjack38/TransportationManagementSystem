﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using System.Threading.Tasks;
using BookingService.Services;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly BookingServices _bookingServices;

        public BookingController(IBookingRepository bookingRepository, BookingServices bookingServices)
        {
            _bookingRepository = bookingRepository;
            _bookingServices = bookingServices;
        }


        // GET /api/Booking?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetBookings([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (bookings, totalCount) = await _bookingRepository.GetBookingsWithCountAsync(page, pageSize);
            var bookingDTOs = bookings.Select(b => new BookingDTO
            {
                ShipmentId = b.ShipmentId,
                CustomerName = b.CustomerName,
                BookingDate = b.BookingDate,
                Status = b.Status
            });

            return Ok(new { Data = bookingDTOs, TotalCount = totalCount });
        }

        // POST /api/Booking
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDTO bookingDTO)
        {
            // Validate the incoming model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Use BookingServices to create a booking after checking shipment availability
            var result = await _bookingServices.CreateBookingAsync(bookingDTO);

            if (!result)
            {
                return BadRequest("Shipment not found or unavailable.");
            }

            // Create the booking entity to be stored in the database
            var booking = new Booking
            {
                ShipmentId = bookingDTO.ShipmentId,
                CustomerName = bookingDTO.CustomerName,
                BookingDate = bookingDTO.BookingDate,
                Status = bookingDTO.Status
            };

            // Add booking to the repository
            await _bookingRepository.AddBookingAsync(booking);

            // Return success and the created booking's details
            return CreatedAtAction(nameof(GetBookingById), new { id = booking.BookingId }, bookingDTO);
        }


        // GET /api/Bookings/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDTO>> GetBookingById(int id)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            var bookingDTO = new BookingDTO
            {
                ShipmentId = booking.ShipmentId,
                CustomerName = booking.CustomerName,
                BookingDate = booking.BookingDate,
                Status = booking.Status
            };

            return Ok(bookingDTO);
        }

        // PUT /api/Booking/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingDTO bookingDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingBooking = await _bookingRepository.GetBookingByIdAsync(id);
            if (existingBooking == null)
            {
                return NotFound("Booking not found.");
            }

            // Update the booking details
            existingBooking.ShipmentId = bookingDTO.ShipmentId;
            existingBooking.CustomerName = bookingDTO.CustomerName;
            existingBooking.BookingDate = bookingDTO.BookingDate;
            existingBooking.Status = bookingDTO.Status;

            await _bookingRepository.UpdateBookingAsync(existingBooking);

            return Ok("Booking updated successfully.");
        }



    }
}
