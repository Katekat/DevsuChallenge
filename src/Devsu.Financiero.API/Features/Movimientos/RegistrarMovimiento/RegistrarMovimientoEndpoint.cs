using Devsu.Financiero.API.Domain;
using Devsu.Financiero.API.Exceptions;
using Devsu.Financiero.API.Infrastructure;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using System.Data;

namespace Devsu.Financiero.API.Features.Movimientos.CrearMovimiento
{
    public record RegistrarMovimientoRequest(string NumeroCuenta, decimal Valor);

    [ApiController]
    [Route("api/movimientos")]
    public class RegistrarMovimientoEndpoint(FinancieroDbContext dbContext) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Handle([FromBody] RegistrarMovimientoRequest request, CancellationToken cancellationToken)
        {

            await using var transaction = await dbContext.Database
                .BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            try
            {
                var cuenta = await dbContext.Cuentas
                .FirstOrDefaultAsync(c => c.NumeroCuenta == request.NumeroCuenta, cancellationToken);

                if (cuenta == null)
                    return NotFound(new { Mensaje = "Cuenta no encontrada" });

                // Obtener el último saldo registrado
                var ultimoMovimiento = await dbContext.Movimientos
                    .Where(m => m.CuentaId == cuenta.Id)
                    .OrderByDescending(m => m.Fecha)
                    .FirstOrDefaultAsync(cancellationToken);

                decimal saldoActual = ultimoMovimiento != null ? ultimoMovimiento.Saldo : cuenta.SaldoInicial;

                //la regla de negocio a la entidad 
                var movimiento = cuenta.RealizarMovimiento(request.Valor, saldoActual);

                dbContext.Movimientos.Add(movimiento);
                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return Created($"/api/movimientos/{movimiento.Id}", new
                {
                    movimiento.Id,
                    movimiento.Fecha,
                    movimiento.TipoMovimiento,
                    movimiento.Valor,
                    movimiento.Saldo
                });
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 1205 or 1222 })
            {
                // 1205 = deadlock, 1222 = lock timeout — ambos son conflictos de concurrencia bajo Serializable
                await transaction.RollbackAsync(cancellationToken);
                return Conflict(new { Mensaje = "No se pudo procesar el movimiento por concurrencia, intente nuevamente." });
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

    }
}
