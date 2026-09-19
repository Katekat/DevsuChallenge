using Devsu.Usuarios.API.Domain;
using Devsu.Usuarios.API.Infrastructure;
using MassTransit;
using Devsu.Shared.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Polly;

namespace Devsu.Usuarios.API.Features.Clientes.RegistrarCliente
{
    public record RegistrarClienteCommand(
     string Nombre,
     string Genero,
     int Edad,
     string Identificacion,
     string Direccion,
     string Telefono,
     string Contrasena
 );

    [ApiController]
    [Route("api/clientes")]
    public class RegistrarClienteEndpoint(UsuariosDbContext dbContext, IPublishEndpoint publishEndpoint) : ControllerBase
    {
        private static readonly IAsyncPolicy PublicacionRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(1, _ => TimeSpan.FromMilliseconds(500));

        [HttpPost]
        public async Task<IActionResult> Handle([FromBody] RegistrarClienteCommand request, CancellationToken cancellationToken)
        {
            //  Verificar si ya existe en BD
            var existe = await dbContext.Clientes.AnyAsync(c => c.Identificacion == request.Identificacion);
            if (existe)
            {
                return Conflict(new { mensaje = "El cliente ya se encuentra registrado", Identificacion = request.Identificacion });
            }
            //encriptando password
            string hash = BCrypt.Net.BCrypt.HashPassword(request.Contrasena);
            // Instanciación  mediante el constructor de la entidad 

            var cliente = new Cliente(
                Guid.NewGuid(), request.Nombre, request.Genero, request.Edad,
                request.Identificacion, request.Direccion, request.Telefono,
                hash);


            dbContext.Clientes.Add(cliente);
            await dbContext.SaveChangesAsync(cancellationToken);

            try
            {
                await PublicacionRetryPolicy.ExecuteAsync(
                    ct => publishEndpoint.Publish(new ClienteCreadoEvent(cliente.Id, cliente.Identificacion, cliente.Nombre), ct),
                    cancellationToken);
            }
            catch (Exception)
            {
                // El cliente ya está guardado; se sincronizara con Financiero 
                // cuando el broker este disponible nuevamente. No se aborta el registro por esto.
                return StatusCode(207, new
                {
                    mensaje = "Cliente creado, pero la sincronización con Financiero podría retrasarse.",
                    clienteId = cliente.Id
                });
            }

            var response = new ClienteResponseDto(
                 ClienteId: cliente.Id, 
                 Nombre: cliente.Nombre,
                 Genero: cliente.Genero,
                 Edad: cliente.Edad,
                 Identificacion: cliente.Identificacion,
                 Direccion: cliente.Direccion,
                 Telefono: cliente.Telefono,
                 Estado: cliente.Estado
             );

            return Created($"/api/clientes/{cliente.Id}", response);
        }
    }
}
