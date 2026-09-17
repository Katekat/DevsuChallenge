namespace Devsu.Financiero.API.Domain
{
    public class Movimiento
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public decimal Saldo { get; set; }

        public Guid CuentaId { get; set; }
        public Cuenta Cuenta { get; set; } = null!;
    }
}
