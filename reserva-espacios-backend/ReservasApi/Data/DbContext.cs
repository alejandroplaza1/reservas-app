using global::ReservasApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ReservasApi.Data
{
    namespace ReservasApi.Data
    {
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

            // DbSets para las entidades
            public DbSet<Usuario> Usuarios { get; set; }
            public DbSet<Espacio> Espacios { get; set; }
            public DbSet<Reserva> Reservas { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // Relación Usuario -> Reservas
                modelBuilder.Entity<Reserva>()
                    .HasOne(r => r.Usuario)
                    .WithMany(u => u.Reservas)
                    .HasForeignKey(r => r.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación Espacio -> Reservas
                modelBuilder.Entity<Reserva>()
                    .HasOne(r => r.Espacio)
                    .WithMany(e => e.Reservas)
                    .HasForeignKey(r => r.EspacioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Configuración opcional: restricciones
                modelBuilder.Entity<Usuario>()
                    .Property(u => u.Nombre)
                    .HasMaxLength(100)
                    .IsRequired();

                modelBuilder.Entity<Espacio>()
                    .Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsRequired();
            }
        }
    }
}
