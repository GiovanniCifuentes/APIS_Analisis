using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRMVentasAPI.Models
{
    public class Tarea
    {
        [Key]
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public bool Completada { get; set; } = false;
        public int OportunidadId { get; set; }

        [ForeignKey("OportunidadId")]
        public Oportunidad? Oportunidad { get; set; }
    }
}



