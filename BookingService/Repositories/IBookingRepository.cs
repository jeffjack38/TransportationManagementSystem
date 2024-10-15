using BookingService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingService.Repositories
{
    public interface IBookingRepository
    {
        Task AddBookingAsync(Booking booking);
        Task<Booking> GetBookingByIdAsync(int bookingId);
        Task<IEnumerable<Booking>> GetBookingsAsync(int page, int pageSize);  // Pagination support
        Task<(IEnumerable<Booking>, int)> GetBookingsWithCountAsync(int page, int pageSize);  // Add this method for total count
        Task UpdateBookingAsync(Booking booking);


    }
}
