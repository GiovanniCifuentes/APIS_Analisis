namespace CRMVentasAPI.Models
{
    public class CreateTareaDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        // Recibir la fecha como string para parseo tolerante en el controller
        public string? FechaVencimiento { get; set; }
        public bool Completada { get; set; } = false;
        public int OportunidadId { get; set; }
    }
}