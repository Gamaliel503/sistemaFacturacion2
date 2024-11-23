namespace SistemaFacturacion.WebApi.Model
{
    public class InvoiceDetail
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public  decimal UnitPrice { get; set; }

        

    }
}
