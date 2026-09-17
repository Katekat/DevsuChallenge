using Devsu.Financiero.API.Domain;
using Devsu.Financiero.API.Exceptions;
using Devsu.Financiero.API.Infrastructure;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Financiero.API.Features.Movimientos
{
    public record RegistrarMovimientoCommand(string NumeroCuenta, decimal Valor);

    [ApiController]
    [Route("api/movimientos")]
    public class RegistrarMovimientoEndpoint(FinancieroDbContext dbContext) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Handle([FromBody] RegistrarMovimientoCommand request)
        {

            var cuenta = await dbContext.Cuentas
                .FirstOrDefaultAsync(c => c.NumeroCuenta == request.NumeroCuenta);

            if (cuenta == null) 
            {
                return NotFound(new { Mensaje = "Cuenta no encontrada" });

            }

            var ultimoMovimiento = await dbContext.Movimientos
             .Where(m => m.CuentaId == cuenta.Id)
             .OrderByDescending(m => m.Fecha)
             .FirstOrDefaultAsync();


            //Si no hay movimientos, el base es el Saldo Inicial
            decimal saldoActual = ultimoMovimiento != null ? ultimoMovimiento.Saldo : cuenta.SaldoInicial;
            decimal nuevoSaldo = saldoActual + request.Valor;

            // Regla F3: Validación estricta
            if (nuevoSaldo < 0)
            {
                throw new BusinessRuleException("Saldo no disponible");
            }

            var movimiento = new Movimiento
            {
                Id = Guid.NewGuid(),
                Fecha = DateTime.UtcNow,
                TipoMovimiento = request.Valor > 0 ? "Deposito" : "Retiro",
                Valor = request.Valor,
                Saldo = nuevoSaldo,
                CuentaId = cuenta.Id
            };

            dbContext.Movimientos.Add(movimiento);
            await dbContext.SaveChangesAsync();

            return Ok(movimiento);
        }
    }
   
}
