using Dapper;
using Microsoft.Extensions.Configuration;
using ShipmentService.Models;
using Microsoft.Data.SqlClient;
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
    }
}
