using CRMVentasAPI.Models;
using Microsoft.EntityFrameworkCore;

// 🔹 Namespace corregido para que coincida con la estructura de tu proyecto
namespace CRMVentasAPI
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Oportunidad> Oportunidades { get; set; }
        public DbSet<Embudo> Embudos { get; set; }
        public DbSet<Propuesta> Propuestas { get; set; }
        public DbSet<Tarea> Tareas { get; set; }
    }
}

