using Devsu.Usuarios.API.Domain;
using Devsu.Usuarios.API.Infrastructure;

using MassTransit;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Polly;

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
    public class RegistrarClienteEndpoint(UsuariosDbContext dbContext, IPublishEndpoint publishEndpoint) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Handle([FromBody] RegistrarClienteCommand request)
        {
            //  Verificar si ya existe en BD
            var existe = await dbContext.Clientes.AnyAsync(c => c.ClienteId == request.ClienteId);
            if (existe)
            {
                return Ok(new { mensaje = "El cliente ya se encuentra registrado", clienteId = request.ClienteId });
            }

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
            // publicar el msg con resiliencia (Polly)
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            try
            {
                await retryPolicy.ExecuteAsync(async () =>
                {
                    await publishEndpoint.Publish(new ClienteCreadoEvent(cliente.Id, cliente.ClienteId, cliente.Nombre));
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Cliente guardado, pero ocurrió un error en la integración tras múltiples intentos", error = ex.Message });
            }

            return Ok(new { mensaje = "Cliente creado exitosamente", clienteId = cliente.ClienteId });
        }
    }
}
