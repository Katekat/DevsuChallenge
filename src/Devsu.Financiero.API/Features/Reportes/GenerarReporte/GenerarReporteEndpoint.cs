using Devsu.Financiero.API.Domain;
using Devsu.Financiero.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Text.Json.Serialization;

namespace Devsu.Financiero.API.Features.Reportes.GenerarReporte
{
    public record ReporteResponseDto(
        string Fecha,
        string Cliente,
        string NumeroCuenta,
        string Tipo,
        decimal SaldoInicial,
        bool Estado,
        decimal Movimiento,
        decimal SaldoDisponible
    );

    [ApiController]
    [Route("api/reportes")]
    public class GenerarReporteEndpoint(FinancieroDbContext dbContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Handle(
            [FromQuery] DateOnly fechaInicio,
            [FromQuery] DateOnly fechaFin,
            [FromQuery] Guid clienteId,
            CancellationToken cancellationToken)
        {
            if (fechaInicio > fechaFin)
                return BadRequest(new { Mensaje = "El rango de fechas es inválido." });

            var fechaInicioDt = fechaInicio.ToDateTime(TimeOnly.MinValue);
            var fechaFinDt = fechaFin.ToDateTime(TimeOnly.MaxValue);


            var reporte = await dbContext.Movimientos
                .AsNoTracking()
                .Where(m => m.Cuenta.TitularCuenta.ClienteId == clienteId &&
                            m.Fecha >= fechaInicioDt &&
                            m.Fecha <= fechaFinDt)
                .OrderBy(m => m.Fecha) // se ordena 
                .Select(m => new ReporteResponseDto(
                    m.Fecha.ToString("dd/MM/yyyy"), // Formato  del ejemplo del enunciado
                    m.Cuenta.TitularCuenta.Nombre,  // el nombre que tenemos en tabla titulares previamente sincronizado por rbmq
                    m.Cuenta.NumeroCuenta,
                    m.Cuenta.TipoCuenta,
                    m.Cuenta.SaldoInicial,
                    m.Cuenta.Estado,
                    m.Valor,
                    m.Saldo
                ))
                .ToListAsync(cancellationToken);

            return Ok(reporte);
        }
    }
}

