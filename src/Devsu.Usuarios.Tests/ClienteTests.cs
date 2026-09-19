using Devsu.Usuarios.API.Domain;

using System;


namespace Devsu.Usuarios.Tests
{
    public class ClienteTests
    {
        [Fact]
        public void CrearCliente_ConDatosValidos_DebeInstanciarCorrectamente()
        {
           
            var idEsperado = Guid.NewGuid();

            
            var cliente = new Cliente(
                idEsperado,
                "Ana Torres",
                "Femenino",
                28,
                "0987654321",
                "Calle Falsa 123",
                "0991122334",
                "Password123"
            );

            // Assert
            Assert.NotNull(cliente);
            Assert.Equal(idEsperado, cliente.Id); // Validamos contra la PK real
            Assert.Equal("0987654321", cliente.Identificacion);
            Assert.True(cliente.Estado);
        }


        [Fact]
        public void Desactivar_ClienteActivo_DebeCambiarEstadoAInactivo()
        {
            var cliente = new Cliente(
                 Guid.NewGuid(), "Ana", "F", 25, "123", "Dir", "Tel", "Pass"
             );

            cliente.Desactivar();

            Assert.False(cliente.Estado);
        }

       
    }
}
