using ProyectoInventario.Domain.Entities;

namespace ProyectoInventario.Domain.Interfaces
{
    public interface IProductoLotesRepo
    {
        Task<IEnumerable<ProductoLote>> GetAllAsync();
        Task<IEnumerable<ProductoLote>> GetByProductoIdAsync(int productoId);
        Task<ProductoLote?> GetByIdAsync(int id);
        Task<int> CrearLoteAsync(ProductoLote lote);
        Task<bool> ActualizarLoteAsync(ProductoLote lote);
        Task<bool> EliminarLoteAsync(int id);
    }
}
