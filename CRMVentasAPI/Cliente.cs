using System.ComponentModel.DataAnnotations;

namespace CRMVentasAPI.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefono
        {
            get; set; // Opcional
        }
    }
}

