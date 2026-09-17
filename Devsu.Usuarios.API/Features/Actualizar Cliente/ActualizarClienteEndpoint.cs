using Devsu.Usuarios.API.Domain;
using Devsu.Usuarios.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Usuarios.API.Features.Actualizar_Cliente
{
    public record ActualizarClienteCommand(
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
    public class ActualizarClienteEndpoint(UsuariosDbContext dbContext) : ControllerBase
    {
        [HttpPut("{idCliente}")]
        public async Task<IActionResult> Handle(string clienteId, [FromBody] ActualizarClienteCommand request)
        {
            var cliente = await dbContext.Clientes
                .FirstOrDefaultAsync(c => c.ClienteId == clienteId);

            if (cliente == null)
                return NotFound(new { mensaje = "Cliente no encontrado" });

            cliente.Nombre = request.Nombre;
            cliente.Genero = request.Genero;
            cliente.Edad = request.Edad;
            cliente.Identificacion = request.Identificacion;
            cliente.Direccion = request.Direccion;
            cliente.Telefono = request.Telefono;
            cliente.Contrasena = request.Contrasena;
            cliente.Estado = request.Estado;

            await dbContext.SaveChangesAsync();

            return Ok(cliente);
        }
    }
}
