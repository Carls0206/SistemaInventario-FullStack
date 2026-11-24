namespace ProyectoInventario.Application.DTOs
{
    public record ErrorResponseDto(string Mensaje, string? Detalle, string? TraceId);
}
