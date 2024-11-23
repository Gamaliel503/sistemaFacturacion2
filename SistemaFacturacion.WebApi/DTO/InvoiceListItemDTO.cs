using SistemaFacturacion.WebApi.Model;

namespace SistemaFacturacion.WebApi.DTO
{
    public class InvoiceListItemDTO
    {
        public int Id { get; set; }
      
        public DateTime Date { get; set; }

        public decimal Total { get; set; }

        public string CustomerName { get; set; }


    }
}
