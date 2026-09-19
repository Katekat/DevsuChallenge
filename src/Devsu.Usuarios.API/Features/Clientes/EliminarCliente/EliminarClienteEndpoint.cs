using Devsu.Usuarios.API.Domain;
using Devsu.Usuarios.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Usuarios.API.Features.Clientes.EliminarCliente
{
    [ApiController]
    [Route("api/clientes")]
    public class EliminarClienteEndpoint(UsuariosDbContext dbContext) : ControllerBase
    {
        [HttpDelete("{idCliente}")]
        public async Task<IActionResult> Handle(Guid idCliente)
        {
            var cliente = await dbContext.Clientes.FirstOrDefaultAsync(c => c.Id == idCliente);

            if (cliente == null)
                return NotFound(new { mensaje = "Cliente no encontrado" });

            // eliminacion logica
            try
            {
                cliente.Desactivar();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }

            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
