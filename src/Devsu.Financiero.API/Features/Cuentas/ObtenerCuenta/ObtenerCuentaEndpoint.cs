using Devsu.Financiero.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Financiero.API.Features.Cuentas.ObtenerCuenta
{

    public record CuentaResponseDto(
        string NumeroCuenta,
        string TipoCuenta,
        decimal SaldoInicial,
        decimal SaldoActual,
        bool Estado,
        string Cliente);

    [ApiController]
    [Route("api/cuentas")]
    public class ObtenerCuentaEndpoint(FinancieroDbContext dbContext) : ControllerBase
    {
        [HttpGet("{numeroCuenta}")]
        public async Task<IActionResult> Handle(string numeroCuenta, CancellationToken cancellationToken)
        {
            var cuenta = await dbContext.Cuentas
                 .AsNoTracking()
                 .Include(c => c.TitularCuenta)
                 .Include(c => c.Movimientos.OrderByDescending(m => m.Fecha).Take(1))
                 .FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta, cancellationToken);

            if (cuenta == null)
                return NotFound(new { Mensaje = "Cuenta no encontrada" });

            var response = new CuentaResponseDto(
                cuenta.NumeroCuenta,
                cuenta.TipoCuenta,
                cuenta.SaldoInicial,
                cuenta.Movimientos.FirstOrDefault()?.Saldo ?? cuenta.SaldoInicial,
                cuenta.Estado,
                cuenta.TitularCuenta.Nombre);

            return Ok(response);
        }
    }
}
