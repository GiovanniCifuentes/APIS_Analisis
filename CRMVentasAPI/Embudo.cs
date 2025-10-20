using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRMVentasAPI.Models
{
    public class Embudo
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Etapas como lista
        public List<string> Etapas { get; set; } = new List<string>();

        public double ValorEstimado { get; set; }
        public string Estado { get; set; } = "Activo";
        public DateTime FechaCierreProbable { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public string Vendedor { get; set; } = string.Empty;

        // Probabilidad de cierre (0-100%)
        public int ProbabilidadCierre { get; set; } = 50;

        // Valor previsto calculado
        [NotMapped]
        public double ValorPrevisto => ValorEstimado * (ProbabilidadCierre / 100.0);
    }
}