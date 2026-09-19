namespace Devsu.Financiero.API.Domain
{
    public class Movimiento
    {
        public Guid Id { get; private set; }
        public DateTime Fecha { get; private set; }
        public string TipoMovimiento { get; private set; } = string.Empty;
        public decimal Valor { get; private set; }
        public decimal Saldo { get; private set; }

        public Guid CuentaId { get; private  set; }
        public Cuenta Cuenta { get; private set; } = null!;


        protected Movimiento() { }

        // Constructor de dominio 
        public Movimiento(Guid id, DateTime fecha, string tipoMovimiento, decimal valor, decimal saldo, Guid cuentaId)
        {
            if (valor == 0)
                throw new ArgumentException("El valor del movimiento no puede ser cero.");

            Id = id != Guid.Empty ? id : Guid.NewGuid();
            Fecha = fecha != default ? fecha : DateTime.Now;
            TipoMovimiento = tipoMovimiento ?? throw new ArgumentNullException(nameof(tipoMovimiento));
            Valor = valor;
            Saldo = saldo;
            CuentaId = cuentaId;
        }

        // Única modificación permitida: Actualizar metadatos para cumplir el CRU de la rúbrica
        // sin comprometer la inmutabilidad matemática del saldo y el valor.
        public void ActualizarTipoMovimiento(string nuevoTipo)
        {
            if (string.IsNullOrWhiteSpace(nuevoTipo))
                throw new ArgumentException("El tipo de movimiento no puede estar vacío.");

            TipoMovimiento = nuevoTipo;
        }
    }
}
