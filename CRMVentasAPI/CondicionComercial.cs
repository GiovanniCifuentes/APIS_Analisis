using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRMVentasAPI.Models
{
    public class CondicionComercial
    {
        public int Id { get; set; }

        // Identificador si la condición pertenece a un cliente o a una oportunidad
        public string? ClienteId { get; set; } // Cambiado a string para coincidir con API externa
        public int? OportunidadId { get; set; }

        // Precio especial (opcional) y/o descuento en porcentaje
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PrecioEspecial { get; set; }

        // Descuento por defecto (en porcentaje, 0-100)
        public double DescuentoPorcentual { get; set; } = 0;

        // Restricciones y vigencia
        public DateTime FechaInicio { get; set; } = DateTime.Now;
        public DateTime? FechaFin { get; set; }
        public bool Activa { get; set; } = true;

        // Metadatos / auditoría
        public string UsuarioCreacion { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public string UsuarioUltimaActualizacion { get; set; } = string.Empty;
        public DateTime UltimaActualizacion { get; set; } = DateTime.Now;

        // Comentarios o política específica
        public string Notas { get; set; } = string.Empty;
    }
}