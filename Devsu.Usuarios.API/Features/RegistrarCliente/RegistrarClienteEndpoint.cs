using Devsu.Usuarios.API.Domain;
using Devsu.Usuarios.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Devsu.Usuarios.API.Features.RegistrarCliente
{
    public record RegistrarClienteCommand(
     string ClienteId,
     string Nombre,
     string Genero,
     int Edad,
     string Identificacion,
     string Direccion,
     string Telefono,
     string Contrasena,
     bool Estado
 );

    [ApiController]
    [Route("api/clientes")]
    public class RegistrarClienteEndpoint(UsuariosDbContext dbContext) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Handle([FromBody] RegistrarClienteCommand request)
        {
            var cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                ClienteId = request.ClienteId,
                Nombre = request.Nombre,
                Genero = request.Genero,
                Edad = request.Edad,
                Identificacion = request.Identificacion,
                Direccion = request.Direccion,
                Telefono = request.Telefono,
                Contrasena = request.Contrasena,
                Estado = request.Estado
            };

            dbContext.Clientes.Add(cliente);
            await dbContext.SaveChangesAsync();

            return Ok(new { mensaje = "Cliente creado exitosamente", cliente.ClienteId });
        }
    }
}
