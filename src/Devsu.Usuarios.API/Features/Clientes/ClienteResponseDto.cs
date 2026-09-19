namespace Devsu.Usuarios.API.Features.Clientes
{
    public record ClienteResponseDto(
       Guid ClienteId,
       string Nombre,
       string Genero,
       int Edad,
       string Identificacion,
       string Direccion,
       string Telefono,
       bool Estado);

}
