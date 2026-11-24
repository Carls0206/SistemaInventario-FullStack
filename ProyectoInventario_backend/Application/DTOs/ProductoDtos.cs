namespace ProyectoInventario.Application.DTOs
{
    public record ProductoCreateDto(string Codigo, string Nombre, string? Descripcion, decimal Precio);

    public record ProductoUpdateDto(string Nombre, string? Descripcion, decimal Precio, string Estado);

    public record ProductoResponseDto(int Id, string Codigo, string Nombre, string? Descripcion, decimal Precio, string Estado);

    public record ProductoDetalleDto(int Id, string Codigo, string Nombre, string? Descripcion, decimal Precio, string Estado, IReadOnlyList<ProductoLoteResponseDto> Lotes);
}
