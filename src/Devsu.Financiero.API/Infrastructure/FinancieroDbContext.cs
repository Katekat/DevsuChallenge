using Devsu.Financiero.API.Domain;

using Microsoft.EntityFrameworkCore;

namespace Devsu.Financiero.API.Infrastructure
{
    public class FinancieroDbContext (DbContextOptions <FinancieroDbContext> options) : DbContext(options)
    {
        public DbSet<Titular> Titulares => Set<Titular>();
        public DbSet<Cuenta> Cuentas => Set<Cuenta>();
        public DbSet<Movimiento> Movimientos => Set<Movimiento>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Titular>(b =>
            //{
            //    b.HasKey(x => x.Id);
            //    b.HasIndex(x => x.ClienteId).IsUnique(); // clave para la idempotencia desde RabbitMQ
            //});

            //modelBuilder.Entity<Cuenta>(b =>
            //{
            //    b.HasKey(x => x.Id);
            //    b.HasIndex(x => x.NumeroCuenta).IsUnique();
            //    b.Property(x => x.SaldoInicial).HasPrecision(18, 4);
            //    b.Property(x => x.TipoCuenta).HasMaxLength(20);
            //    // Relación fuerte usando el Guid interno
            //    b.HasOne(x => x.TitularCuenta)
            //     .WithMany(t => t.Cuentas)
            //     .HasForeignKey(x => x.TitularId)
            //     .OnDelete(DeleteBehavior.Restrict);

            //});

            //modelBuilder.Entity<Movimiento>(b =>
            //{
            //    b.HasKey(x => x.Id);
            //    b.Property(x => x.Valor).HasPrecision(18, 4);
            //    b.Property(x => x.Saldo).HasPrecision(18, 4);
            //    b.Property(x => x.TipoMovimiento).HasMaxLength(20);


            //    b.HasOne(x => x.Cuenta)
            //      .WithMany(c => c.Movimientos)
            //      .HasForeignKey(x => x.CuentaId)
            //      .IsRequired()
            //      .OnDelete(DeleteBehavior.Restrict);
            //});

            modelBuilder.Entity<Titular>(b =>
            {
                b.ToTable("Titulares");
                b.HasKey(x => x.Id);

                // Mapeamos explícitamente la columna externa que viene del microservicio de Usuarios
                b.Property(x => x.ClienteId)
                 .HasColumnName("ClienteId")
                 .IsRequired();

                b.Property(x => x.Nombre)
                 .HasMaxLength(100)
                 .IsRequired();

                b.Property(x => x.Identificacion)
                 .HasMaxLength(20)
                 .IsRequired();

                // Índice único para evitar duplicados en la sincronización por eventos
                b.HasIndex(x => x.ClienteId).IsUnique();
            });

            modelBuilder.Entity<Cuenta>(b =>
            {
                b.ToTable("Cuentas");
                b.HasKey(x => x.Id);

                b.HasIndex(x => x.NumeroCuenta).IsUnique();

                b.Property(x => x.NumeroCuenta)
                 .HasMaxLength(50)
                 .IsRequired();

                b.Property(x => x.SaldoInicial)
                 .HasPrecision(18, 4);

                b.Property(x => x.TipoCuenta)
                 .HasMaxLength(20);

                
                // Enlazamos Cuenta.TitularId con Titular.Id 
                b.HasOne(x => x.TitularCuenta)
                 .WithMany(t => t.Cuentas)
                 .HasForeignKey(x => x.TitularId)
                 .HasPrincipalKey(t => t.Id) // Refer a la PK de Titular
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Movimiento>(b =>
            {
                b.ToTable("Movimientos");
                b.HasKey(x => x.Id);

                b.Property(x => x.Valor).HasPrecision(18, 4);
                b.Property(x => x.Saldo).HasPrecision(18, 4);
                b.Property(x => x.TipoMovimiento).HasMaxLength(20);

                b.HasOne(x => x.Cuenta)
                  .WithMany(c => c.Movimientos)
                  .HasForeignKey(x => x.CuentaId)
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
