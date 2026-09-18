using Devsu.Financiero.API.Domain;
using Devsu.Financiero.API.Infrastructure;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Devsu.Usuarios.API.Features.RegistrarCliente;

// 1. EL CONTRATO: Debe tener el namespace exacto de donde se origina el mensaje.
// Esta es la clave que enlaza la cola de Financiero con el Exchange de Usuarios.
namespace Devsu.Usuarios.API.Features.RegistrarCliente
{
    public record ClienteCreadoEvent(Guid Id, string ClienteId, string Nombre);
}


namespace Devsu.Financiero.API.Features.Clientes
{
    public class ClienteCreadoConsumer(
        FinancieroDbContext dbContext,
        ILogger<ClienteCreadoConsumer> logger) : IConsumer<ClienteCreadoEvent>
    {
        public async Task Consume(ConsumeContext<ClienteCreadoEvent> context)
        {
            var evento = context.Message;

            var existe = await dbContext.ClientesLocales
                .AnyAsync(c => c.ClienteId == evento.ClienteId, context.CancellationToken);

            if (existe)
            {
                logger.LogInformation("El cliente {ClienteId} ya fue sincronizado previamente. Ignorando evento duplicado.", evento.ClienteId);
                return;
            }

            var clienteEspejo = new ClienteLocal
            {
                Id = evento.Id, // se usa  el ID original
                ClienteId = evento.ClienteId,
                Nombre = evento.Nombre
            };

            dbContext.ClientesLocales.Add(clienteEspejo);
            await dbContext.SaveChangesAsync(context.CancellationToken);

            logger.LogInformation("Cliente {ClienteId} sincronizado exitosamente en Financiero.API.", evento.ClienteId);
        }
    }
}

