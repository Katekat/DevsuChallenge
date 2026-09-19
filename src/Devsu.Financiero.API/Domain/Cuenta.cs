using Devsu.Financiero.API.Exceptions;

using System;

namespace Devsu.Financiero.API.Domain
{
    public class Cuenta
    {
        public Guid Id { get; private set; }
        public string NumeroCuenta { get; private set; } = string.Empty;
        public string TipoCuenta { get; private set; } = string.Empty;
        public decimal SaldoInicial { get; private set; }
        public bool Estado { get; private set; }

        // Foreign Key de tabla clieente
     
        public Guid TitularId { get; set; }
        public Titular TitularCuenta { get; set; } = null!;

        public ICollection<Movimiento> Movimientos { get; private set; } = new List<Movimiento>();

        protected Cuenta() { }

        // Constructor de dominio para la creación 
        public Cuenta(Guid id, string numeroCuenta, string tipoCuenta, decimal saldoInicial, Guid titularId)
        {
            if (string.IsNullOrWhiteSpace(numeroCuenta))
                throw new ArgumentException("El número de cuenta es obligatorio.");

            if (saldoInicial < 0)
                throw new BusinessRuleException("El saldo inicial no puede ser negativo.");

            Id = id != Guid.Empty ? id : Guid.NewGuid();
            NumeroCuenta = numeroCuenta;
            TipoCuenta = tipoCuenta ?? "Ahorros";
            SaldoInicial = saldoInicial;
            Estado = true;
            TitularId = titularId;
        }

        public Movimiento RealizarMovimiento(decimal valor, decimal saldoActual)
        {
            if (!Estado)
                throw new BusinessRuleException("La cuenta no se encuentra activa");

            if (valor == 0)
                throw new BusinessRuleException("El valor del movimiento no puede ser cero");

            decimal nuevoSaldo = saldoActual + valor;

            if (nuevoSaldo < 0)
                throw new BusinessRuleException("Saldo no disponible");

            string tipo = valor > 0 ? "Deposito" : "Retiro";

            //usamos constructor de dominio de Movimiento
            return new Movimiento(Guid.NewGuid(), DateTime.UtcNow, tipo, valor, nuevoSaldo, Id);
        }

        public void ActualizarDatos(string tipoCuenta, bool estado)
        {
            if (string.IsNullOrWhiteSpace(tipoCuenta))
                throw new ArgumentException("El tipo de cuenta es obligatorio.");

            TipoCuenta = tipoCuenta;
            Estado = estado;
        }
    }
}
