using System.Data;
using Dapper;
using ProyectoInventario.Domain.Interfaces;
using ProyectoInventario.Dominio.Entidades;

namespace ProyectoInventario.Infrastructure.Repositories
{
    public class ProductoLoteRepository: IProductoLotesRepo
    {
        private readonly IDbConnection _conn;
        public ProductoLoteRepository(IDbConnection conn) {
            _conn = conn;
        } 

        public async Task<IEnumerable<ProductoLote>> GetByProductoIdAsync(int productoId)
        {
            const string sql = "SELECT Id, ProductoId, NumeroLote, FechaIngreso, PrecioUnitario, Cantidad FROM ProductoLote WHERE ProductoId = @ProductoId";
            return await _conn.QueryAsync<ProductoLote>(sql, new { ProductoId = productoId });
        }

        public async Task<ProductoLote?> GetByIdAsync(int id)
        {
            const string sql = "SELECT Id, ProductoId, NumeroLote, FechaIngreso, PrecioUnitario, Cantidad FROM ProductoLote WHERE Id = @Id";
            return await _conn.QueryFirstOrDefaultAsync<ProductoLote>(sql, new { Id = id });
        }

        public async Task<int> CrearLoteAsync(ProductoLote lote)
        {
            const string sql = @"INSERT INTO ProductoLote (ProductoId, NumeroLote, FechaIngreso, PrecioUnitario, Cantidad)
                             VALUES (@ProductoId, @NumeroLote, @FechaIngreso, @PrecioUnitario, @Cantidad);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await _conn.ExecuteScalarAsync<int>(sql, lote);
        }

        public async Task<bool> ActualizarLoteAsync(ProductoLote lote)
        {
            const string sql = @"UPDATE ProductoLote SET NumeroLote = @NumeroLote, FechaIngreso = @FechaIngreso,
                             PrecioUnitario = @PrecioUnitario, Cantidad = @Cantidad WHERE Id = @Id";
            var rows = await _conn.ExecuteAsync(sql, lote);
            return rows > 0;
        }

        public async Task<bool> EliminarLoteAsync(int id)
        {
            var rows = await _conn.ExecuteAsync("DELETE FROM ProductoLote WHERE Id = @Id", new { Id = id });
            return rows > 0;
        }

    }
}
