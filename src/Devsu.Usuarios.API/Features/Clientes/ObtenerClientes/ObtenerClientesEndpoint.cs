using Devsu.Usuarios.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Usuarios.API.Features.Clientes.ObtenerClientes
{
    [ApiController]
    [Route("api/clientes")]
    public class ObtenerClientesEndpoint(UsuariosDbContext dbContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Handle(CancellationToken cancellationToken)
        {
            var clientes = await dbContext.Clientes
                .AsNoTracking()
                .Select(c => new ClienteResponseDto(
                    c.Id, c.Nombre, c.Genero, c.Edad,
                    c.Identificacion, c.Direccion, c.Telefono, c.Estado))
                .ToListAsync(cancellationToken);

            return Ok(clientes);
        }
    }
}
