using CRMVentasAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CRMVentasAPI
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Oportunidad> Oportunidades { get; set; }
        public DbSet<Embudo> Embudos { get; set; }
        public DbSet<Propuesta> Propuestas { get; set; }
        public DbSet<Tarea> Tareas { get; set; }

        // 🔹 NUEVO: Agregar DbSet para condiciones comerciales y descuentos
        public DbSet<CondicionComercial> CondicionesComerciales { get; set; }
        public DbSet<DescuentoAplicado> DescuentosAplicados { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar Embudo para almacenar List<string> como JSON (CORREGIDO)
            modelBuilder.Entity<Embudo>()
                .Property(e => e.Etapas)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, new System.Text.Json.JsonSerializerOptions()),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, new System.Text.Json.JsonSerializerOptions()) ?? new List<string>()
                );
        }
    }
}
