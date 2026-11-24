using System.Data;
using Dapper;
using ProyectoInventario.Domain.Interfaces;
using ProyectoInventario.Dominio.Entidades;

namespace ProyectoInventario.Infrastructure.Repositories
{
    public class ProductoRepository: IProductosRepo
    {
        private readonly IDbConnection _conn;
        public ProductoRepository(IDbConnection conn) {
            _conn = conn;
        } 

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            const string sql = "SELECT Id, Codigo, Nombre, Descripcion, Activo FROM Producto WHERE Activo = 1";
            return await _conn.QueryAsync<Producto>(sql);
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            const string sql = "SELECT Id, Codigo, Nombre, Descripcion, Activo FROM Producto WHERE Id = @Id";
            return await _conn.QueryFirstOrDefaultAsync<Producto>(sql, new { Id = id });
        }

        public async Task<Producto?> GetByCodigoAsync(string codigo)
        {
            const string sql = "SELECT Id, Codigo, Nombre, Descripcion, Activo FROM Producto WHERE Codigo = @Codigo";
            return await _conn.QueryFirstOrDefaultAsync<Producto>(sql, new { Codigo = codigo });
        }

        public async Task<int> CrearProductoAsync(Producto producto)
        {
            const string sql = @"INSERT INTO Producto (Codigo, Nombre, Descripcion, Activo)
                             VALUES (@Codigo, @Nombre, @Descripcion, @Activo);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await _conn.ExecuteScalarAsync<int>(sql, producto);
        }

        public async Task<bool> ActualizarProductoAsync(Producto producto)
        {
            const string sql = @"UPDATE Producto SET Nombre = @Nombre, Descripcion = @Descripcion, Activo = @Activo WHERE Id = @Id";
            var rows = await _conn.ExecuteAsync(sql, producto);
            return rows > 0;
        }

        public async Task<bool> EliminarProductoAsync(int id)
        {
            const string sql = "UPDATE Producto SET Activo = 0 WHERE Id = @Id";
            var rows = await _conn.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}
