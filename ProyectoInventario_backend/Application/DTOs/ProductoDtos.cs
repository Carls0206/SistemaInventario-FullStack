namespace ProyectoInventario.Application.DTOs
{
    public record ProductoCreateDto(string Codigo, string Nombre, string? Descripcion);

    public record ProductoUpdateDto(string Nombre, string? Descripcion, bool Activo);

    public record ProductoResponseDto(int Id, string Codigo, string Nombre, string? Descripcion, bool Activo);

    public record ProductoDetalleDto(int Id,string Codigo,string Nombre,string? Descripcion,bool Activo,IReadOnlyList<ProductoLoteResponseDto> Lotes);
}
