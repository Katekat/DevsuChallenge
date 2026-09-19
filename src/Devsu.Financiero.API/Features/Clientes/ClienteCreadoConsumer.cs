using Devsu.Financiero.API.Domain;
using Devsu.Financiero.API.Infrastructure;
using Devsu.Shared.Events;

using MassTransit;

using Microsoft.EntityFrameworkCore;


namespace Devsu.Financiero.API.Features.Clientes
{
    public class ClienteCreadoConsumer(
        FinancieroDbContext dbContext,
        ILogger<ClienteCreadoConsumer> logger) : IConsumer<ClienteCreadoEvent>
    {
        public async Task Consume(ConsumeContext<ClienteCreadoEvent> context)
        {
            var evento = context.Message;

            var existe = await dbContext.Titulares
                .AnyAsync(c => c.ClienteId == evento.Id, context.CancellationToken);

            if (existe)
            {
                logger.LogInformation("El cliente {ClienteId} ya fue sincronizado previamente. Ignorando evento duplicado.", evento.Id);
                return;
            }

            var clienteEspejo = new Titular
            {
                Id = Guid.NewGuid(),
                ClienteId = evento.Id, // referencia del cliente 
                Identificacion = evento.Identificacion,
                Nombre = evento.Nombre
            };

            dbContext.Titulares.Add(clienteEspejo);
            await dbContext.SaveChangesAsync(context.CancellationToken);

            logger.LogInformation("Cliente {ClienteId} sincronizado exitosamente en Financiero.API.", evento.Id);
        }
    }
}

