using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRMVentasAPI.Models
{
    public class Propuesta
    {
        [Key]
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        public string Estado { get; set; } = "Borrador";
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public int OportunidadId { get; set; }

        [ForeignKey("OportunidadId")]
        public Oportunidad? Oportunidad { get; set; }
    }
}





