using Dapper;
using Microsoft.Extensions.Configuration;
using ShipmentService.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShipmentService.Repositories
{
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly string _connectionString;

        public ShipmentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ShipmentDbContext");
        }

        public async Task AddShipmentAsync(Shipment shipment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "INSERT INTO Shipments (Origin, Destination, ShipDate, Status) " +
                            "VALUES (@Origin, @Destination, @ShipDate, @Status)";
                await connection.ExecuteAsync(query, shipment);
            }
        }

        public async Task UpdateShipmentAsync(Shipment shipment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "UPDATE Shipments SET Origin = @Origin, Destination = @Destination, ShipDate = @ShipDate, Status = @Status " +
                            "WHERE ShipmentId = @ShipmentId";
                await connection.ExecuteAsync(query, shipment);
            }
        }

        public async Task UpdateShipmentStatusAsync(int shipmentId, string status)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "UPDATE Shipments SET Status = @Status WHERE ShipmentId = @ShipmentId";
                await connection.ExecuteAsync(query, new { ShipmentId = shipmentId, Status = status });
            }
        }

        public async Task<Shipment> GetShipmentByIdAsync(int shipmentId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Shipments WHERE ShipmentId = @ShipmentId";
                return await connection.QueryFirstOrDefaultAsync<Shipment>(query, new { ShipmentId = shipmentId });
            }
        }

        // Method to retrieve paginated shipments
        public async Task<IEnumerable<Shipment>> GetShipmentsPaginatedAsync(int page, int pageSize)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"SELECT * FROM Shipments
                              ORDER BY ShipmentId
                              OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                return await connection.QueryAsync<Shipment>(query, new
                {
                    Offset = (page - 1) * pageSize,  // Calculate how many records to skip
                    PageSize = pageSize  // Number of records to fetch
                });
            }
        }

        // Method to get total count of shipments (for pagination)
        public async Task<int> GetTotalShipmentsCountAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT COUNT(*) FROM Shipments";
                return await connection.ExecuteScalarAsync<int>(query);
            }
        }

        public async Task<IEnumerable<Shipment>> GetShipmentsAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Shipments";
                return await connection.QueryAsync<Shipment>(query);
            }
        }
    }
}

