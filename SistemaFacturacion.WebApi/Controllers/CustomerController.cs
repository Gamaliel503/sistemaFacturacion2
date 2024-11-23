using Dapper;
using Microsoft.AspNetCore.Mvc;
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

        private readonly IDbConnection _connection;
        public CustomerController(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("SqlConnection"));
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var customer = _connection.Query<Customer>("SELECT * FROM Customers");
            return Ok(customer);
        }

        [HttpGet("GetAllAsync")]
        public async Task<IActionResult> GetAllAsync()
        {
            var customer = await _connection.QueryAsync<Customer>("SELECT * FROM Customers");
            return Ok(customer);
        }

        [HttpGet("{id}", Name = "GetById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            //var parametros = new
            //{
            //    Id = id
            //};

           
            var customer = await _connection.QueryFirstOrDefaultAsync<Customer>("SELECT * FROM Customers WHERE Id = @Id", new { Id = id});
            return Ok(customer);
        }

        
        

        [HttpGet("GetByNameAsync/{nombre}")]
        public async Task<IActionResult> GetByNameAsync(string nombre)
        {
            var customer = await _connection.QueryFirstOrDefaultAsync<Customer>("SELECT * FROM Customers WHERE Name = @NombreCompleto55", new { NombreCompleto55 = nombre });
            return Ok(customer);
        }

        [HttpGet("SearhClientByNameAsync/{nombre}")]
        public async Task<IActionResult> SearhClientByNameAsync(string nombre)
        {
            var customer = await _connection.QueryAsync<Customer>("SELECT * FROM Customers WHERE Name LIKE @Nombre", new { Nombre = $"%{nombre}%" });
            return Ok(customer);
        }


        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            var sql = @"INSERT INTO Customers (Name)
                    VALUES (@Name);
                    SELECT CAST(SCOPE_IDENTITY() as int)";            

            var id = await _connection.QuerySingleAsync<int>(sql, customer);
            customer.Id = id;

            return CreatedAtAction("GetById", new { id = customer.Id }, customer);
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> Update(int id, Customer customer)
        {
            customer.Id = id;
            var sql = @"UPDATE Customers
                        SET Name= @Name
                        WHERE Id = @Id";

            var affected = await _connection.ExecuteAsync(sql, customer);

            if (affected == 0)
                return NotFound();

            return Ok(customer);


        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var affected = await _connection.ExecuteAsync("DELETE FROM Customers WHERE Id = @Id", new { Id = id });

            if (affected == 0)
                return NotFound();

            return NoContent();

        
        
        }


    }
}
