using Devsu.Financiero.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Financiero.API.Features.Cuentas.ActualizarCuenta
{
    public record ActualizarCuentaRequest(string TipoCuenta, bool Estado);

    [ApiController]
    [Route("api/cuentas")]
    public class ActualizarCuentaEndpoint(FinancieroDbContext dbContext) : ControllerBase
    {
        [HttpPut("{numeroCuenta}")]
        public async Task<IActionResult> Handle(string numeroCuenta, [FromBody] ActualizarCuentaRequest request, CancellationToken cancellationToken)
        {
            var cuenta = await dbContext.Cuentas
                .FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta, cancellationToken);

            if (cuenta == null)
                return NotFound(new { Mensaje = "Cuenta no encontrada" });

            cuenta.ActualizarDatos(request.TipoCuenta, request.Estado);

            await dbContext.SaveChangesAsync(cancellationToken);

            return Ok(new
            {
                cuenta.NumeroCuenta,
                cuenta.TipoCuenta,
                cuenta.SaldoInicial,
                cuenta.Estado
            });
        }
    }
}
