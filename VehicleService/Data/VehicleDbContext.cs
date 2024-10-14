using System.Collections.Generic;
using System.Reflection.Emit;
using VehicleService.Models;

using Microsoft.EntityFrameworkCore;
using VehicleService.Models;

namespace VehicleService.Data
{
    public class VehicleDbContext : DbContext
    {
        public VehicleDbContext(DbContextOptions<VehicleDbContext> options) : base(options) { }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Driver> Drivers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure one-to-many relationship between Vehicle and Driver
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Driver)
                .WithMany(d => d.Vehicles)
                .HasForeignKey(v => v.DriverId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Set field constraints
            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Make)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Model)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Driver>()
                .HasIndex(d => d.LicenseNumber)
                .IsUnique();

            // Add indexes
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.VehicleId)
                .HasDatabaseName("IDX_VehicleId");

            modelBuilder.Entity<Driver>()
                .HasIndex(d => d.DriverId)
                .HasDatabaseName("IDX_DriverId");

            // Seed initial data 
            modelBuilder.Entity<Driver>().HasData(
                new Driver { DriverId = 1, Name = "John Doe", LicenseNumber = "A1234567" }
            );

            modelBuilder.Entity<Vehicle>().HasData(
                new Vehicle { VehicleId = 1, Make = "Ford", Model = "F-150", Year = 2022, DriverId = 1 }
            );

            base.OnModelCreating(modelBuilder);
        }

    }
}
