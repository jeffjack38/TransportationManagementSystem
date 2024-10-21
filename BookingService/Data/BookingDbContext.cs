using Microsoft.EntityFrameworkCore;
using SharedModels.Models;
namespace BookingService.Data
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add an index on BookingDate
            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.BookingDate)
                .HasDatabaseName("IDX_BookingDate");

            // Add an index on ShipmentId
            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.ShipmentId)
                .HasDatabaseName("IDX_ShipmentId");
        }
    }
}
