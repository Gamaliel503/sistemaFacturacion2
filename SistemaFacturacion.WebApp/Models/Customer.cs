using System.ComponentModel.DataAnnotations;

namespace SistemaFacturacion.WebApp.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [MaxLength(15,ErrorMessage="No se permite mas de 15 caracteres") ]
        [MinLength(3, ErrorMessage = "No se permite menos de 3 caracteres")]
        public string Name { get; set; }

    }
}
