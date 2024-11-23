namespace SistemaFacturacion.WebApp.Models
{
    
        public class Detail
        {
            public int Id { get; set; }
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
        }

        public class Invoice
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }
            public DateTime Date { get; set; }
            public decimal Total { get; set; }
        public List<Detail> Details { get; set; } = new List<Detail>();
        }


    
}
