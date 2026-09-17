namespace Devsu.Financiero.API.Domain
{
    public class ClienteLocal
    {
        public Guid Id { get; set; }
        public string ClienteId { get; set; } = string.Empty; // El ID que viene de Usuarios
        public string Nombre { get; set; } = string.Empty; // Requerido para el JSON del F4

        public ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
    }
}
