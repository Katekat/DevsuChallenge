namespace Devsu.Usuarios.API.Domain
{
    public class Cliente : Persona
    {
        public string ClienteId { get; set; } = string.Empty; // Identificador único de negocio (F1)
        public string Contrasena { get; set; } = string.Empty;
        public bool Estado { get; set; } = true;

    }
}
