using Devsu.Usuarios.API.Domain;

using Microsoft.EntityFrameworkCore;



namespace Devsu.Usuarios.API.Infrastructure
{
    public class UsuariosDbContext(DbContextOptions<UsuariosDbContext> options) : DbContext(options)
    {
        public DbSet<Cliente> Clientes => Set<Cliente>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Garantiza la unicidad del identificador de negocio en la capa de persistencia
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.ClienteId)
                .IsUnique();

            // Nota arquitectónica: EF Core mapea por defecto la herencia usando TPH 
            // (Table-Per-Hierarchy), unificando Persona y Cliente en una sola tabla eficiente.
        }
    }
}
