namespace Devsu.Usuarios.API.Domain
{
    public class Cliente : Persona
    {
       
        public string Contrasena { get; private set; } = string.Empty;
        public bool Estado { get; private set; } = true;


        protected Cliente() : base() { }

        // Constructor de dominio:
        public Cliente(
            Guid id,
            string nombre,
            string genero,
            int edad,
            string identificacion,
            string direccion,
            string telefono,
            string contrasena)
            : base(id, nombre, genero, edad, identificacion, direccion, telefono)
        {

            if (string.IsNullOrWhiteSpace(contrasena))
                throw new ArgumentException("La contraseña es obligatoria.");

           
            Contrasena = contrasena;
            Estado = true;
        }

        // Desactivar el cliente protegiendo su estado
        public void Desactivar()
        {
            if (!Estado)
            {
                throw new InvalidOperationException("El cliente ya se encuentra inactivo.");
            }

            Estado = false;
        }

        
        public void ActualizarDatos(string nombre, string genero, int edad, string direccion, string telefono)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio.");

            if (edad < 0)
                throw new ArgumentException("La edad no puede ser negativa.");

            Nombre = nombre;
            Genero = genero ?? string.Empty;
            Edad = edad;
            Direccion = direccion ?? string.Empty;
            Telefono = telefono ?? string.Empty;
        }
    }

}

