using Devsu.Financiero.API.Domain;
using Devsu.Financiero.API.Infrastructure;

using MassTransit;

using Microsoft.EntityFrameworkCore;

namespace Devsu.Financiero.API.Features.Clientes
{
    public record ClienteCreadoEvent(string ClienteId, string Nombre);

    public class ClienteCreadoConsumer(
        FinancieroDbContext dbContext,
        ILogger<ClienteCreadoConsumer> logger) : IConsumer<ClienteCreadoEvent>
    {
        public async Task Consume(ConsumeContext<ClienteCreadoEvent> context)
        {
            var evento = context.Message;

            // 1. Idempotencia obligatoria: En RabbitMQ un evento puede entregarse más de una vez (At-least-once).
            // Si no validas esto, un reintento reventará tu base de datos por duplicidad de PK/UK.
            var siExiste = await dbContext.ClientesLocales
                .AnyAsync(c => c.ClienteId == evento.ClienteId, context.CancellationToken);

            if (siExiste)
            {
                logger.LogInformation("El cliente {ClienteId} ya fue sincronizado previamente. Ignorando evento duplicado.", evento.ClienteId);
                return;
            }

            // 2. Mapeo a la entidad espejo local
            var clienteEspejo = new ClienteLocal
            {
                Id = Guid.NewGuid(),
                ClienteId = evento.ClienteId,
                Nombre = evento.Nombre
            };

            dbContext.ClientesLocales.Add(clienteEspejo);

            // 3. Persistencia directa
            await dbContext.SaveChangesAsync(context.CancellationToken);

            logger.LogInformation("Cliente {ClienteId} sincronizado exitosamente en Financiero.API.", evento.ClienteId);
        }
    }
}
