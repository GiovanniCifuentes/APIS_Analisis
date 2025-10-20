namespace CRMVentasAPI.Models
{
    public class AplicacionDescuentoDto
    {
        public string? ClienteId { get; set; }
        public int? OportunidadId { get; set; }
        public int? PropuestaId { get; set; }
        public decimal PrecioOriginal { get; set; }
        public double DescuentoSolicitado { get; set; }
        public string? UsuarioAplicacion { get; set; }
        public string? Comentarios { get; set; }
    }
}