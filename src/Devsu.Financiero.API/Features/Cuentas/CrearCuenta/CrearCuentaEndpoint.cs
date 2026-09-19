using Devsu.Financiero.API.Domain;
using Devsu.Financiero.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Financiero.API.Features.Cuentas.CrearCuenta
{
    public record CrearCuentaCommand(Guid ClienteId, string NumeroCuenta, string TipoCuenta, decimal SaldoInicial);

    [ApiController]
    [Route("api/cuentas")]
    public class CrearCuentaEndpoint(FinancieroDbContext dbContext) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Handle([FromBody] CrearCuentaCommand request, CancellationToken cancellationToken)
        {
            var titular = await dbContext.Titulares
                .FirstOrDefaultAsync(t => t.ClienteId == request.ClienteId, cancellationToken);

            if (titular == null)
            {
                return BadRequest(new { Mensaje = $"El cliente con el id '{request.ClienteId}' no existe o no ha sido sincronizado." });
            }

            var existeCuenta = await dbContext.Cuentas
                .AnyAsync(c => c.NumeroCuenta == request.NumeroCuenta, cancellationToken);

            if (existeCuenta)
            {
                return Conflict(new { Mensaje = $"El número de cuenta {request.NumeroCuenta} ya se encuentra registrado." });
            }

           
            var cuenta = new Cuenta(
                Guid.NewGuid(),
                request.NumeroCuenta,
                request.TipoCuenta,
                request.SaldoInicial,
                titular.Id
            );

            dbContext.Cuentas.Add(cuenta);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Created($"/api/cuentas/{cuenta.Id}", new
            {
                cuenta.Id,
                cuenta.NumeroCuenta,
                cuenta.TipoCuenta,
                cuenta.SaldoInicial,
                cuenta.Estado,
                request.ClienteId
            });
        }
    }
}
