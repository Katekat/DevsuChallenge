using Devsu.Financiero.API.Domain;

using Microsoft.EntityFrameworkCore;

namespace Devsu.Financiero.API.Infrastructure
{
    public class FinancieroDbContext (DbContextOptions <FinancieroDbContext> options) : DbContext(options)
    {
        public DbSet<ClienteLocal> ClientesLocales => Set<ClienteLocal>();
        public DbSet<Cuenta> Cuentas => Set<Cuenta>();
        public DbSet<Movimiento> Movimientos => Set<Movimiento>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClienteLocal>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasIndex(x => x.ClienteId).IsUnique(); // implica la idempotencia desde RabbitMQ
            });

            modelBuilder.Entity<Cuenta>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasIndex(x => x.NumeroCuenta).IsUnique();
                b.Property(x => x.SaldoInicial).HasPrecision(18, 4);
                b.Property(x => x.TipoCuenta).HasMaxLength(20);

            });

            modelBuilder.Entity<Movimiento>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Valor).HasPrecision(18, 4);
                b.Property(x => x.Saldo).HasPrecision(18, 4);
                b.Property(x => x.TipoMovimiento).HasMaxLength(20);
            });
        }
    }
}
