using ProyectoInventario.Domain.Entities;

namespace ProyectoInventario.Domain.Interfaces
{
    public interface ILotesRepo
    {
        Task<IEnumerable<Lote>> GetAllAsync();
        Task<Lote?> GetByIdAsync(int id);
        Task<Lote?> GetByNumeroAsync(string numeroLote);
        Task<int> CrearAsync(Lote lote);
        Task<bool> ActualizarAsync(Lote lote);
        Task<bool> EliminarAsync(int id);
    }
}
