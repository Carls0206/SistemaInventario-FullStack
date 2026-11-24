namespace ProyectoInventario.Application.DTOs
{
    public record LoteCreateDto(string NumeroLote);

    public record LoteUpdateDto(string NumeroLote, string Estado);

    public record LoteResponseDto(int Id, string NumeroLote, DateTime? FechaCreacion, string Estado);

    public record ProductoLoteCreateDto(int ProductoId, int LoteId, DateTime? FechaIngreso, decimal PrecioUnitario, int Cantidad);

    public record ProductoLoteUpdateDto(int LoteId, DateTime? FechaIngreso, decimal PrecioUnitario, int Cantidad, string Estado);

    public record ProductoLoteResponseDto(int Id, int ProductoId, int LoteId, DateTime? FechaIngreso, decimal PrecioUnitario, int Cantidad, string Estado);
}
