using ProyectoInventario.Application.DTOs;

namespace ProyectoInventario.Application.Interfaces
{
    public interface ILoteService
    {
        Task<IEnumerable<LoteResponseDto>> GetAllAsync();
        Task<LoteResponseDto?> GetByIdAsync(int id);
        Task<LoteResponseDto> CreateAsync(LoteCreateDto dto);
        Task<bool> UpdateAsync(int id, LoteUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
