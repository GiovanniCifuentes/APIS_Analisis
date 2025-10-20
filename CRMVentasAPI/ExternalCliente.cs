using System.ComponentModel.DataAnnotations;

namespace CRMVentasAPI.Models
{
    public class ExternalCliente
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Fecha_Alta { get; set; }
        public int CategoriaCliente { get; set; }
        public string CategoriaClienteNombre { get; set; } = string.Empty;
        public long Nit { get; set; }
        public bool Estado { get; set; }
        public Contacto ContactoId { get; set; } = new Contacto();
    }

    public class Contacto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long Dpi { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime Fecha_Nacimiento { get; set; }
        public bool Estado { get; set; }
        public bool IsVerified { get; set; }
    }
}
