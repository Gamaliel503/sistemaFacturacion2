namespace SistemaFacturacion.WebApi.Model
{
    public class Invoice
    {

        public int Id { get; set; }

        public int CustomerId { get; set; }

        public DateTime Date { get; set; }

        public decimal Total { get; set; }

        public List<InvoiceDetail> Details { get; set; }




    }
}
