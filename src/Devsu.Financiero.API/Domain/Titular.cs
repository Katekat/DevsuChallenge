namespace Devsu.Financiero.API.Domain
{
    public class Titular
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; } // El ID que viene de clientes
        public string Nombre { get; set; } = string.Empty; 

        public string Identificacion { get; set; } = string.Empty;

        public ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
    }
}
