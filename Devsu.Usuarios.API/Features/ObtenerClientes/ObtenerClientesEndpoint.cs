using Devsu.Usuarios.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Usuarios.API.Features.ObtenerClientes
{
    [ApiController]
    [Route("api/clientes")]
    public class ObtenerClientesEndpoint(UsuariosDbContext dbContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Handle()
        {
            var clientes = await dbContext.Clientes
                .Select(c => new
                {
                    c.ClienteId,
                    c.Nombre,
                    c.Genero,
                    c.Edad,
                    c.Identificacion,
                    c.Direccion,
                    c.Telefono,
                    c.Estado
                })
                .ToListAsync();

            return Ok(clientes);
        }
    }
}
