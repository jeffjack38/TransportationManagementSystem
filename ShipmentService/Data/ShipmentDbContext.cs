using Microsoft.EntityFrameworkCore;
using ShipmentService.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ShipmentService.Data
{
    public class ShipmentDbContext : DbContext
    {
        public ShipmentDbContext(DbContextOptions<ShipmentDbContext> options) : base(options)
        {
        }
        public DbSet<Shipment> Shipments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Shipment>()
                .HasIndex(s => s.ShipmentId)
                .HasDatabaseName("IDX_ShipmentId");  // Use HasDatabaseName

            modelBuilder.Entity<Shipment>()
                .HasIndex(s => s.Status)
                .HasDatabaseName("IDX_Status");  // Use HasDatabaseName
        }
    }
}

