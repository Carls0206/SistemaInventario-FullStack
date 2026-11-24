using ProyectoInventario.Domain.Entities;

namespace ProyectoInventario.Domain.Interfaces
{
    public interface IProductosRepo
    {
        Task<IEnumerable<Producto>> GetAllAsync();
        Task<Producto?> GetByIdAsync(int id);
        Task<Producto?> GetByCodigoAsync(string codigo);
        Task<int> CrearProductoAsync(Producto producto);
        Task<bool> ActualizarProductoAsync(Producto producto);
        Task<bool> EliminarProductoAsync(int id);
    }
}
