using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRMVentasAPI.Models
{
    public class Oportunidad
    {
        [Key]
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Etapa { get; set; } = string.Empty;
        public decimal ValorEstimado { get; set; }
        public DateTime FechaCierreProbable { get; set; }
        public string Estado { get; set; } = "Abierta";

        // 🔹 CAMBIO: Ahora es string para coincidir con la API externa
        public string ClienteId { get; set; } = string.Empty;

        // 🔹 NUEVO: Datos del cliente para mostrar (no se guardan en DB)
        [NotMapped]
        public string ClienteNombre { get; set; } = string.Empty;
        [NotMapped]
        public string ClienteEmail { get; set; } = string.Empty;
        [NotMapped]
        public string ClienteTelefono { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaCierre { get; set; }
        public string Vendedor { get; set; } = string.Empty;

        // Propiedades de navegación
        public ICollection<Tarea>? Tareas { get; set; }
        public ICollection<Propuesta>? Propuestas { get; set; }
    }
}
