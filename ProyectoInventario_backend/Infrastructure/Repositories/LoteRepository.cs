using System.Data;
using Dapper;
using ProyectoInventario.Domain.Entities;
using ProyectoInventario.Domain.Interfaces;

namespace ProyectoInventario.Infrastructure.Repositories
{
    public class LoteRepository : ILotesRepo
    {
        private readonly IDbConnection _conn;

        public LoteRepository(IDbConnection conn)
        {
            _conn = conn;
        }

        public async Task<IEnumerable<Lote>> GetAllAsync()
        {
            const string sql = "SELECT Id, NumeroLote, FechaCreacion, Estado FROM Lote WHERE Estado <> 'N' ORDER BY Id DESC";
            return await _conn.QueryAsync<Lote>(sql);
        }

        public async Task<Lote?> GetByIdAsync(int id)
        {
            const string sql = "SELECT Id, NumeroLote, FechaCreacion, Estado FROM Lote WHERE Id = @Id";
            return await _conn.QueryFirstOrDefaultAsync<Lote>(sql, new { Id = id });
        }

        public async Task<Lote?> GetByNumeroAsync(string numeroLote)
        {
            const string sql = "SELECT Id, NumeroLote, FechaCreacion, Estado FROM Lote WHERE NumeroLote = @NumeroLote";
            return await _conn.QueryFirstOrDefaultAsync<Lote>(sql, new { NumeroLote = numeroLote });
        }

        public async Task<int> CrearAsync(Lote lote)
        {
            const string sql = @"INSERT INTO Lote (NumeroLote, FechaCreacion, Estado)
                                 VALUES (@NumeroLote, @FechaCreacion, @Estado);
                                 SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await _conn.ExecuteScalarAsync<int>(sql, lote);
        }

        public async Task<bool> ActualizarAsync(Lote lote)
        {
            const string sql = "UPDATE Lote SET NumeroLote = @NumeroLote, Estado = @Estado WHERE Id = @Id";
            var rows = await _conn.ExecuteAsync(sql, lote);
            return rows > 0;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var rows = await _conn.ExecuteAsync("UPDATE Lote SET Estado = 'N' WHERE Id = @Id", new { Id = id });
            return rows > 0;
        }
    }
}
