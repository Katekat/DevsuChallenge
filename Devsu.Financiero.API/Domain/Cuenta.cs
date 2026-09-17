namespace Devsu.Financiero.API.Domain
{
    public class Cuenta
    {
        public Guid Id { get; set; }
        public string NumeroCuenta { get; set; } = string.Empty;
        public string TipoCuenta { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
        public bool Estado { get; set; }

        // Foreign Key a nuestra tabla espejo
        public Guid ClienteLocalId { get; set; }
        public ClienteLocal ClienteLocal { get; set; } = null!;

        public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
    }
}
