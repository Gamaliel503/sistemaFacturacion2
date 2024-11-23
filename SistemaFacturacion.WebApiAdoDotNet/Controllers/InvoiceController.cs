using Dapper;
using Microsoft.AspNetCore.Mvc;
using SistemaFacturacion.WebApi.DTO;
using SistemaFacturacion.WebApi.Model;
using System.Data;
using System.Data.Common;

namespace SistemaFacturacion.WebApi.Controllers
{

    [ApiController]
    [Route("api/[Controller]")]

    public class InvoiceController : ControllerBase
    {

        private readonly IDbConnection _dbConnection;

        public InvoiceController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
                
        }

        [HttpGet]

        public async Task<ActionResult> GetAll()
        { 
            

            var sql = @"

             SELECT i.Id, i.Date, i.Total, c.Name AS CustomerName

             FROM Invoices i

             JOIN Customers c ON i.CustomerId = c.Id

             ORDER BY i.Date DESC";

            var invoices = await _dbConnection.QueryAsync<InvoiceListItemDTO>(sql);
            return Ok(invoices);

        }

        [HttpPost]

        public async Task<ActionResult> Create(Invoice invoice)
        {
            using (var dbConnection = _dbConnection) 
                        
            {
                dbConnection.Open();

                using (var transaction = dbConnection.BeginTransaction())
                {
                    try
                    {
                        // Insertar factura

                        var invoiceSql = @"INSERT INTO Invoices (CustomerId, Date, Total) 

                        VALUES (@CustomerId, @Date, @Total);

                         SELECT CAST(SCOPE_IDENTITY() as int)";

                        int invoiceId = await dbConnection.QuerySingleAsync<int>(invoiceSql, invoice, transaction);


                        foreach (var detail in invoice.Details)
                        {

                            var detailSql = @"INSERT INTO InvoiceDetails (InvoiceId, ProductId, Quantity, UnitPrice) 
                                        VALUES (@InvoiceId, @ProductId, @Quantity, @UnitPrice);
                                        UPDATE Products 
                                        SET StockQuantity = StockQuantity - @Quantity 
                                        WHERE Id = @ProductId";

                        

                        await dbConnection.ExecuteAsync(detailSql, new
                        {
                            InvoiceId = invoiceId,
                            detail.ProductId,
                            detail.Quantity,
                            detail.UnitPrice
                        }, transaction);

                        }


                        transaction.Commit();

                        return Ok(new { Id = invoiceId });
                    }



                    catch (Exception)
                    {

                       transaction.Rollback();
                        return StatusCode(500, "Error al procesar la factura");
                    }
                
                }
            
            }
                 
          
        }



        [HttpDelete("{id}")]

        public async Task<ActionResult> Delete(int id)
        {
            using (var dbConnection = _dbConnection)

            {
                dbConnection.Open();

                

                using (var transaction = dbConnection.BeginTransaction())
                {
                    try
                    {
                        // Insertar factura

                        var invoiceSql = @"DELETE FROM InvoiceDetails WHERE InvoiceId = @Id";

                        int invoiceId = await dbConnection.ExecuteAsync(invoiceSql, new { Id = id }, transaction);


                        var detailSql = @"DELETE FROM Invoices WHERE Id = @Id";



                        await dbConnection.ExecuteAsync(detailSql, new { Id = id }, transaction);




                        transaction.Commit();

                        return Ok();
                    }
                    catch (Exception)
                    {

                        transaction.Rollback();
                        return StatusCode(500, "Error al procesar la factura");
                    }

                }

            }


        }





    }
}
