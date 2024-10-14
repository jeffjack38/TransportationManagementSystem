using Dapper;
using Microsoft.Extensions.Configuration;
using BookingService.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingService.DTOs;

namespace BookingService.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly string _connectionString;

        public BookingRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BookingDbContext");
        }

        public async Task AddBookingAsync(Booking booking)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "INSERT INTO Bookings (ShipmentId, CustomerName, BookingDate, Status) " +
                            "VALUES (@ShipmentId, @CustomerName, @BookingDate, @Status)";
                await connection.ExecuteAsync(query, booking);
            }
        }

        public async Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Bookings WHERE BookingId = @BookingId";
                return await connection.QueryFirstOrDefaultAsync<Booking>(query, new { BookingId = bookingId });
            }
        }

        public async Task<IEnumerable<Booking>> GetBookingsAsync(int page, int pageSize)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Bookings ORDER BY BookingDate " +
                            "OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                return await connection.QueryAsync<Booking>(query, new { Offset = (page - 1) * pageSize, PageSize = pageSize });
            }
        }

        public async Task<(IEnumerable<Booking>, int)> GetBookingsWithCountAsync(int page, int pageSize)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Bookings ORDER BY BookingDate " +
                            "OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var countQuery = "SELECT COUNT(*) FROM Bookings";

                var bookings = await connection.QueryAsync<Booking>(query, new { Offset = (page - 1) * pageSize, PageSize = pageSize });
                var totalCount = await connection.ExecuteScalarAsync<int>(countQuery);

                return (bookings, totalCount);
            }
        }

        public async Task<IEnumerable<BookingShipmentDTO>> GetBookingsWithShipmentsAsync(int page, int pageSize)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
            SELECT b.BookingId, b.CustomerName, b.BookingDate, b.Status, s.ShipmentId, s.Origin, s.Destination, s.ShipDate, s.Status AS ShipmentStatus
            FROM Bookings b
            JOIN Shipments s ON b.ShipmentId = s.ShipmentId
            ORDER BY b.BookingDate
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                return await connection.QueryAsync<BookingShipmentDTO>(query, new { Offset = (page - 1) * pageSize, PageSize = pageSize });
            }
        }
    }
}

