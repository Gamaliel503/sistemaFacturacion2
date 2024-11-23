using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SistemaFacturacion.WebApi.Model;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace SistemaFacturacion.WebApi.Controllers
{

    [ApiController]
    [Route("api/[Controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly string _connectionString;

        public CustomerController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SqlConnection");

        }

        [HttpGet]

        public async Task<IActionResult> GetAll()
        {
            var customer = new List<Customer>();

            using (var conex = new SqlConnection(_connectionString))
            {
                await conex.OpenAsync();
                var command = new SqlCommand("SELECT * FROM Customers", conex);
                using (var reader = await command.ExecuteReaderAsync())
                {

                    while (reader.Read())
                    {

                        customer.Add(new Customer

                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),

                        });

                    }

                }

            }

            return Ok(customer);
        }


        [HttpGet("{id}")]

        public async Task<IActionResult> GetbyId(int id)
        {
            Customer? customer = null;
            using (var conex = new SqlConnection(_connectionString))
            {
                await conex.OpenAsync();
                var command = new SqlCommand("SELECT * FROM Customers WHERE Id = @Id", conex);
                command.Parameters.Add(new SqlParameter("id", SqlDbType.Int)).Value = id;

                using (var reader = await command.ExecuteReaderAsync())

                {
                    while (reader.Read())

                    {
                        customer = new Customer()

                        { Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),

                        };

                    }

                }

            }

            if (customer == null)
                return NotFound();

            return Ok(customer);

        }

        [HttpPost]

        public async Task<IActionResult> Create(Customer customer)
        {
            int newCustomerId;
            var sql = @"INSERT INTO Customers (Name)
                      VALUES (@Name);
                      SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var conex = new SqlConnection(_connectionString))
            {
                await conex.OpenAsync();
                var command = new SqlCommand(sql, conex);
                command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar) { Value = customer.Name });

                newCustomerId = (int)(await command.ExecuteScalarAsync() ?? 0);

            }
            customer.Id = newCustomerId;
            return CreatedAtAction(nameof(GetbyId), new { id = customer.Id }, customer);

        }

        [HttpPut("{id}")]

        public async Task<IActionResult> Update(int id, Customer customer)
        {
            int affectedRows;
            var sql = @"UPDATE Customers
                        SET Name= @Name
                        WHERE Id = @Id";
            using (var conex = new SqlConnection(_connectionString))
            {
                await conex.OpenAsync();
                var command = new SqlCommand(sql, conex);

                command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar) { Value = customer.Name });

                affectedRows = await command.ExecuteNonQueryAsync();
            }

            if (affectedRows == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete]

        public async Task<IActionResult> Delete(int id)
        {
            int affectedRows;
            var sql = @"DELETE FROM Customers WHERE Id = @Id";

            using (var conex = new SqlConnection(_connectionString))
            {
                await conex.OpenAsync();
                var command = new SqlCommand(sql, conex);
                command.Parameters.Add(new SqlParameter(@"id", SqlDbType.Int) { Value = id });
                affectedRows = await command.ExecuteNonQueryAsync();

            }
            if (affectedRows == 0)
                return NotFound();

            return NoContent();


        }

    }
}
