namespace ProyectoInventario.Application.DTOs
{
    public record ProductoLoteCreateDto(int ProductoId, string NumeroLote, DateTime? FechaIngreso, decimal PrecioUnitario, int Cantidad);

    public record ProductoLoteUpdateDto(string NumeroLote, DateTime? FechaIngreso, decimal PrecioUnitario, int Cantidad);

    public record ProductoLoteResponseDto(int Id, int ProductoId, string NumeroLote, DateTime? FechaIngreso, decimal PrecioUnitario, int Cantidad);
}
