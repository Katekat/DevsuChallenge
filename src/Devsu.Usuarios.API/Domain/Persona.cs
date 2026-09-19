namespace Devsu.Usuarios.API.Domain
{
    public class Persona
    {
        public Guid Id { get; protected set; } 
        public string Nombre { get; protected set; } = string.Empty;
        public string Genero { get; protected set; } = string.Empty;
        public int Edad { get; protected set; }
        public string Identificacion { get; protected set; } = string.Empty;
        public string Direccion { get; protected set; } = string.Empty;
        public string Telefono { get; protected set; } = string.Empty;

        protected Persona() { }

        // Constructor de dominio para asegurar que se registre datos correctos
        public Persona(Guid id, string nombre, string genero, int edad, string identificacion, string direccion, string telefono)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(identificacion))
                throw new ArgumentException("La identificación es obligatoria.");

            if (edad < 0)
                throw new ArgumentException("La edad no puede ser negativa.");

            Id = id != Guid.Empty ? id : Guid.NewGuid();
            Nombre = nombre;
            Genero = genero ?? string.Empty;
            Edad = edad;
            Identificacion = identificacion;
            Direccion = direccion ?? string.Empty;
            Telefono = telefono ?? string.Empty;
        }
    }
}
