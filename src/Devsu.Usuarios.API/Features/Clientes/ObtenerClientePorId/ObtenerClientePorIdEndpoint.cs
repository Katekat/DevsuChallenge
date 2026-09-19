using Devsu.Usuarios.API.Domain;
using Devsu.Usuarios.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Threading;

namespace Devsu.Usuarios.API.Features.Clientes.ObtenerClientePorId
{
    [ApiController]
    [Route("api/clientes")]
    public class ObtenerClientePorIdEndpoint(UsuariosDbContext dbContext) : ControllerBase
    {
        [HttpGet("{idCliente}")]
        public async Task<IActionResult> Handle(Guid idCliente, CancellationToken cancellationToken)
        {
            var cliente = await dbContext.Clientes
                .AsNoTracking()
                .Where(c => c.Id == idCliente)
                .Select(c => new ClienteResponseDto(
                    c.Id, c.Nombre, c.Genero, c.Edad,
                    c.Identificacion, c.Direccion, c.Telefono, c.Estado))
                .FirstOrDefaultAsync(cancellationToken);

            return cliente == null ? NotFound(new { mensaje = "Cliente no encontrado" }) : Ok(cliente);
        }
    }
}
