
using Devsu.Financiero.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Financiero.API.Features.Movimientos.ObtenerMovimiento
{

    public record MovimientoResponseDto(
    Guid Id,
    DateTime Fecha,
    string TipoMovimiento,
    decimal Valor,
    decimal Saldo,
    Guid CuentaId);

    [ApiController]
    [Route("api/movimientos")]
    public class ObtenerMovimientoEndpoint(FinancieroDbContext dbContext) : ControllerBase
    {
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Handle(Guid id, CancellationToken cancellationToken)
        {
            // ObtenerMovimiento
            var movimiento = await dbContext.Movimientos
                .AsNoTracking()
                .Where(m => m.Id == id)
                .Select(m => new MovimientoResponseDto(m.Id, m.Fecha, m.TipoMovimiento, m.Valor, m.Saldo, m.CuentaId))
                .FirstOrDefaultAsync(cancellationToken);

           

            if (movimiento == null)
                return NotFound(new { Mensaje = "Movimiento no encontrado" });

            return Ok(movimiento);
        }
    }
}
