using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using SharedModels.Models;

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
                .HasDatabaseName("IDX_ShipmentId");  

            modelBuilder.Entity<Shipment>()
                .HasIndex(s => s.Status)
                .HasDatabaseName("IDX_Status");  
        }
    }
}

