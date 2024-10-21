using BookingService.DTOs;
using SharedModels.Models;
using BookingService.Repositories;

namespace BookingService.Services
{
    public class BookingServices
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IBookingRepository _bookingRepository;  // Inject BookingRepository
        private readonly string _shipmentServiceUrl;


        public BookingServices(IHttpClientFactory httpClientFactory, IBookingRepository bookingRepository, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _bookingRepository = bookingRepository;
            _shipmentServiceUrl = configuration["ShipmentServiceUrl"];
        }

        // Check if shipment is available
        public async Task<bool> IsShipmentAvailableAsync(int shipmentId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_shipmentServiceUrl}/api/Shipment/{shipmentId}");

            if (response.IsSuccessStatusCode)
            {
                // Use ShipmentDTO to deserialize the response
                var shipment = await response.Content.ReadFromJsonAsync<ShipmentDTO>();
                return shipment.Status == "Available";
            }

            Console.WriteLine($"Failed to retrieve shipment. StatusCode: {response.StatusCode}");
            return false;
        }

        // Create a booking if shipment is available
        public async Task<bool> CreateBookingAsync(BookingDTO bookingDTO)
        {
            bool isShipmentAvailable = await IsShipmentAvailableAsync(bookingDTO.ShipmentId);

            if (!isShipmentAvailable)
            {
                return false;
            }

            // Use repository to add booking to the database
            await _bookingRepository.AddBookingAsync(new Booking
            {
                ShipmentId = bookingDTO.ShipmentId,
                CustomerName = bookingDTO.CustomerName,
                BookingDate = bookingDTO.BookingDate,
                Status = bookingDTO.Status
            });

            return true;
        }
    }

}
