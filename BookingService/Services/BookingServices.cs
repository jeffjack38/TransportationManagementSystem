using BookingService.DTOs;
using SharedModels.Models;
using BookingService.Repositories;

namespace BookingService.Services
{
    public class BookingServices
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IBookingRepository _bookingRepository;  
        private readonly string _shipmentServiceUrl;

        public BookingServices(IHttpClientFactory httpClientFactory, IBookingRepository bookingRepository, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _bookingRepository = bookingRepository;
            _shipmentServiceUrl = configuration["ShipmentServiceUrl"];
        }

        public async Task<bool> IsShipmentAvailableAsync(int shipmentId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_shipmentServiceUrl}/api/Shipment/{shipmentId}");

            if (response.IsSuccessStatusCode)
            {
                var shipment = await response.Content.ReadFromJsonAsync<ShipmentDTO>();
                return shipment != null && shipment.Status == "Available";
            }

            Console.WriteLine($"Failed to retrieve shipment. StatusCode: {response.StatusCode}");
            return false;
        }

        
        public async Task<bool> CreateBookingAsync(BookingDTO bookingDTO)
        {
            Console.WriteLine($"Attempting to create booking for ShipmentId: {bookingDTO.ShipmentId}");

           
            bool isShipmentAvailable = await IsShipmentAvailableAsync(bookingDTO.ShipmentId);

            if (!isShipmentAvailable)
            {
                Console.WriteLine("Shipment not available or does not exist.");
                return false;
            }

            await _bookingRepository.AddBookingAsync(new Booking
            {
                ShipmentId = bookingDTO.ShipmentId,
                CustomerName = bookingDTO.CustomerName,
                BookingDate = bookingDTO.BookingDate,
                Status = bookingDTO.Status,
                UserId = bookingDTO.UserId
            });

            return true;
        }
    }
}
