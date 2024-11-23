using Microsoft.AspNetCore.Mvc;
using SistemaFacturacion.WebApi.Model;
using System.Data;
using System.Data.SqlClient;

namespace SistemaFacturacion.WebApi.Controllers
{
	[ApiController]
	[Route("api/[Controller]")]
	public class ProductController : ControllerBase
	{

		private readonly string _connectionString;
		public ProductController(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("SqlConnection");
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			//Se inicializa una lista vacía de objetos Product que almacenará los productos recuperados de la base de datos.
			var products = new List<Product>();

			//conexion a la base de datos
			using (var db = new SqlConnection(_connectionString))
			{
				//abre la conexion a la base de datos
				await db.OpenAsync();
				var command = new SqlCommand("SELECT * FROM Products", db);
				using (var reader = await command.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						products.Add(new Product
						{
							Id = reader.GetInt32("Id"),
							Name = reader.GetString("Name"),
							Description = reader.GetString("Description"),
							Price = reader.GetDecimal("Price"),
							StockQuantity = reader.GetInt32("StockQuantity")
						});
					}
				}
			}

			return Ok(products);
		}


		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			Product? products = null;
			using (var db = new SqlConnection(_connectionString))
			{
				await db.OpenAsync();
				var command = new SqlCommand("SELECT * FROM Products WHERE Id = @Id", db);
				command.Parameters.Add(new SqlParameter("Id", SqlDbType.Int)).Value = id;

				using (var reader = await command.ExecuteReaderAsync())
				{
					if (reader.Read())
					{
						products = new Product()
						{
							Id = reader.GetInt32("Id"),
							Name = reader.GetString("Name"),
							Description = reader.GetString("Description"),
							Price = reader.GetDecimal("Price"),
							StockQuantity = reader.GetInt32("StockQuantity")
						};
					}
				}
			}

			if (products == null)
				return NotFound();

			return Ok(products);
		}

		[HttpPost]
		public async Task<IActionResult> Create(Product product)
		{
			int newProductId;
			var sql = @"INSERT INTO Products (Name, Description, Price, StockQuantity) 
                    VALUES (@Name, @Description, @Price, @StockQuantity);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

			using (var db = new SqlConnection(_connectionString))
			{
				await db.OpenAsync();
				var command = new SqlCommand(sql, db);
				command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar) { Value = product.Name });
				command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar) { Value = product.Description });
				command.Parameters.Add(new SqlParameter("@Price", SqlDbType.Decimal) { Value = product.Price });
				command.Parameters.Add(new SqlParameter("@StockQuantity", SqlDbType.Int) { Value = product.StockQuantity });

				newProductId = (int)(await command.ExecuteScalarAsync() ?? 0);
			}

			product.Id = newProductId;

			return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, Product product)
		{
			int affectedRows;
			var sql = @"UPDATE Products 
                        SET Name = @Name, Description = @Description, 
                        Price = @Price, StockQuantity = @StockQuantity 
                        WHERE Id = @Id";

			using (var connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();
				var command = new SqlCommand(sql, connection);

				command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
				command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar) { Value = product.Name });
				command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar) { Value = product.Description });
				command.Parameters.Add(new SqlParameter("@Price", SqlDbType.Decimal) { Value = product.Price });
				command.Parameters.Add(new SqlParameter("@StockQuantity", SqlDbType.Int) { Value = product.StockQuantity });

				affectedRows = await command.ExecuteNonQueryAsync();
			}

			if (affectedRows == 0)
				return NotFound();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			int affectedRows;
			var sql = @"DELETE FROM Products WHERE Id = @Id";

			using (var connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();
				var command = new SqlCommand(sql, connection);
				command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
				affectedRows = await command.ExecuteNonQueryAsync();
			}

			if (affectedRows == 0)
				return NotFound();

			return NoContent();
		}
	}
}

