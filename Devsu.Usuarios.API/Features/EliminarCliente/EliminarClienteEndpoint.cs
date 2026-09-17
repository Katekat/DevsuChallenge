using Devsu.Usuarios.API.Domain;
using Devsu.Usuarios.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Usuarios.API.Features.EliminarCliente
{
    [ApiController]
    [Route("api/clientes")]
    public class EliminarClienteEndpoint(UsuariosDbContext dbContext) : ControllerBase
    {
        [HttpDelete("{idCliente}")]
        public async Task<IActionResult> Handle(string clienteId)
        {
            var cliente = await dbContext.Clientes
                .FirstOrDefaultAsync(c => c.ClienteId == clienteId);

            if (cliente == null)
                return NotFound(new { mensaje = "Cliente no encontrado" });

            dbContext.Clientes.Remove(cliente);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
