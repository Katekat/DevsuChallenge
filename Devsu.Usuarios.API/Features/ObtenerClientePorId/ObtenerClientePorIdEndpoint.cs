using Devsu.Usuarios.API.Domain;
using Devsu.Usuarios.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Usuarios.API.Features.ObtenerClientePorId
{
    [ApiController]
    [Route("api/clientes")]
    public class ObtenerClientePorIdEndpoint(UsuariosDbContext dbContext) : ControllerBase
    {
        [HttpGet("{idCliente}")]
        public async Task<IActionResult> Handle(string clienteId)
        {
            var cliente = await dbContext.Clientes
                .Where(c => c.ClienteId == clienteId)
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
                .FirstOrDefaultAsync();

            return cliente == null ? NotFound(new { mensaje = "Cliente no encontrado" }) : Ok(cliente);
        }
    }
}
