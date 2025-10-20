using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRMVentasAPI.Models
{
    public class DescuentoAplicado
    {
        public int Id { get; set; }

        public string? ClienteId { get; set; } // Cambiado a string para coincidir con API externa
        public int? OportunidadId { get; set; }
        public int? PropuestaId { get; set; }
        public int? CondicionComercialId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioOriginal { get; set; }

        public double DescuentoPorcentual { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioFinal { get; set; }

        public string UsuarioAplicacion { get; set; } = string.Empty;
        public DateTime FechaAplicacion { get; set; } = DateTime.Now;

        public string Comentarios { get; set; } = string.Empty;
    }
}
