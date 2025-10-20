using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRMVentasAPI.Models
{
    public class Tarea
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public DateTime? FechaVencimiento { get; set; }

        public bool Completada { get; set; } = false;

        // 🔹 OportunidadId ahora nullable para evitar error si no hay FK
        public int? OportunidadId { get; set; }

        [ForeignKey("OportunidadId")]
        public Oportunidad? Oportunidad { get; set; }
    }
}


