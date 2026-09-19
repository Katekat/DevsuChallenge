using Devsu.Usuarios.API.Domain;
using Devsu.Usuarios.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Usuarios.API.Features.Clientes.ActualizarCliente
{
    public record ActualizarClienteCommand(
     string Nombre,
     string Genero,
     int Edad,
     string Direccion,
     string Telefono
 );

   
    [ApiController]
    [Route("api/clientes")]
    public class ActualizarClienteEndpoint(UsuariosDbContext dbContext) : ControllerBase
    {
        [HttpPut("{idCliente}")]
        public async Task<IActionResult> Handle(Guid idCliente, [FromBody] ActualizarClienteCommand request)
        {
            var cliente = await dbContext.Clientes
                .FirstOrDefaultAsync(c => c.Id == idCliente);

            if (cliente == null)
                return NotFound(new { mensaje = "Cliente no encontrado" });

            cliente.ActualizarDatos(
                        request.Nombre,
                        request.Genero,
                        request.Edad,
                        request.Direccion,
                        request.Telefono
                    );

            await dbContext.SaveChangesAsync();

            return Ok(new ClienteResponseDto(
                    cliente.Id, cliente.Nombre, cliente.Genero, cliente.Edad,
                    cliente.Identificacion, cliente.Direccion, cliente.Telefono, cliente.Estado));
        }
    }
}
