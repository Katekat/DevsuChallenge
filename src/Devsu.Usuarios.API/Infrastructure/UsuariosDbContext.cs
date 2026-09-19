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

            modelBuilder.Entity<Cliente>(builder =>
            {
                
                builder.ToTable("Clientes");

                // pk
                builder.Property(c => c.Id)
                      .HasColumnName("ClienteId");
                
                builder.Property(c => c.Nombre)
                       .HasMaxLength(100)
                       .IsRequired(); 

                builder.Property(c => c.Identificacion)
                       .HasMaxLength(20)
                       .IsRequired();

                // La identificación no se puede repetir es unica. 
                //
                builder.HasIndex(c => c.Identificacion)
                       .IsUnique();

                builder.Property(c => c.Contrasena)
                       .HasMaxLength(256) // recordar enciptar este campo
                       .IsRequired();

                builder.Property(c => c.Estado)
                       .IsRequired();

                builder.Property(c => c.Edad)
                       .IsRequired();

                builder.Property(c => c.Genero)
                       .HasMaxLength(20)
                       .IsRequired(false); 

                builder.Property(c => c.Direccion)
                       .HasMaxLength(200)
                       .IsRequired(false);

                builder.Property(c => c.Telefono)
                       .HasMaxLength(20)
                       .IsRequired(false);
            });
        }
    }
 }

