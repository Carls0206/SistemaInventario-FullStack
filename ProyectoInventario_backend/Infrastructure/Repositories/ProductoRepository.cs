using System.Data;
using Dapper;
using ProyectoInventario.Domain.Interfaces;
using ProyectoInventario.Domain.Entities;

namespace ProyectoInventario.Infrastructure.Repositories
{
    public class ProductoRepository : IProductosRepo
    {
        private readonly IDbConnection _conn;
        public ProductoRepository(IDbConnection conn)
        {
            _conn = conn;
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            const string sql = "SELECT Id, Codigo, Nombre, Descripcion, Precio, Estado FROM Producto WHERE Estado <> 'N'";
            return await _conn.QueryAsync<Producto>(sql);
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            const string sql = "SELECT Id, Codigo, Nombre, Descripcion, Precio, Estado FROM Producto WHERE Id = @Id";
            return await _conn.QueryFirstOrDefaultAsync<Producto>(sql, new { Id = id });
        }

        public async Task<Producto?> GetByCodigoAsync(string codigo)
        {
            const string sql = "SELECT Id, Codigo, Nombre, Descripcion, Precio, Estado FROM Producto WHERE Codigo = @Codigo";
            return await _conn.QueryFirstOrDefaultAsync<Producto>(sql, new { Codigo = codigo });
        }

        public async Task<int> CrearProductoAsync(Producto producto)
        {
            const string sql = @"INSERT INTO Producto (Codigo, Nombre, Descripcion, Precio, Estado)
                             VALUES (@Codigo, @Nombre, @Descripcion, @Precio, @Estado);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await _conn.ExecuteScalarAsync<int>(sql, producto);
        }

        public async Task<bool> ActualizarProductoAsync(Producto producto)
        {
            const string sql = @"UPDATE Producto SET Nombre = @Nombre, Descripcion = @Descripcion, Precio = @Precio, Estado = @Estado WHERE Id = @Id";
            var rows = await _conn.ExecuteAsync(sql, producto);
            return rows > 0;
        }

        public async Task<bool> EliminarProductoAsync(int id)
        {
            const string sql = "UPDATE Producto SET Estado = 'N' WHERE Id = @Id";
            var rows = await _conn.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}
