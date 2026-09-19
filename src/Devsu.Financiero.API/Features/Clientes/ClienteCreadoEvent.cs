using MassTransit;

namespace Devsu.Shared.Events
{
    [EntityName("cliente-creado-event")]
    public record ClienteCreadoEvent(Guid Id, string Identificacion, string Nombre);
}
