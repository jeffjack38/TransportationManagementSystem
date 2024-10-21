using SharedModels.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingService.Repositories
{
    public interface IBookingRepository
    {
        Task AddBookingAsync(Booking booking);
        Task<Booking> GetBookingByIdAsync(int bookingId);
        Task<IEnumerable<Booking>> GetBookingsAsync(int page, int pageSize);  
        Task<(IEnumerable<Booking>, int)> GetBookingsWithCountAsync(int page, int pageSize);  
        Task UpdateBookingAsync(Booking booking);
        Task DeleteBookingAsync(int bookingId);

    }
}
