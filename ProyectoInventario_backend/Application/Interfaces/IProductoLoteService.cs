using ProyectoInventario.Application.DTOs;

namespace ProyectoInventario.Application.Interfaces
{
    public interface IProductoLoteService
    {
        Task<IEnumerable<ProductoLoteResponseDto>> GetByProductoIdAsync(int productoId);
        Task<ProductoLoteResponseDto?> GetByIdAsync(int id);
        Task<ProductoLoteResponseDto> CreateAsync(ProductoLoteCreateDto dto);
        Task<bool> UpdateAsync(int id, ProductoLoteUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
