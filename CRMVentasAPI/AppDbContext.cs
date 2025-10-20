using CRMVentasAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CRMVentasAPI
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // 🔹 ELIMINADO: DbSet<Cliente> ya que usamos API externa
        public DbSet<Oportunidad> Oportunidades { get; set; }
        public DbSet<Embudo> Embudos { get; set; }
        public DbSet<Propuesta> Propuestas { get; set; }
        public DbSet<Tarea> Tareas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar Embudo para almacenar List<string> como JSON
            modelBuilder.Entity<Embudo>()
                .Property(e => e.Etapas)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<string>()
                );
        }
    }
}
