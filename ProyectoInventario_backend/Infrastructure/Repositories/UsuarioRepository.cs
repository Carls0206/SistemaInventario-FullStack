using System.Data;
using Dapper;
using ProyectoInventario.Domain.Interfaces;
using ProyectoInventario.Dominio.Entidades;

namespace ProyectoInventario.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuariosRepo
    {
        private readonly IDbConnection _conexion;
        public UsuarioRepository(IDbConnection conn) {
            _conexion = conn;
        } 

        public async Task<Usuario?> ObtenerPorCorreoAsync(string email)
        {
            const string sql = @"SELECT Id, Nombre, Email, contraseña AS PasswordHash, Activo, FechaCreacion
                             FROM Usuario WHERE Email = @Email";
            return await _conexion.QueryFirstOrDefaultAsync<Usuario>(sql, new { Email = email });
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            const string sql = @"SELECT Id, Nombre, Email, contraseña AS PasswordHash, Activo, FechaCreacion
                             FROM Usuario WHERE Id = @Id";
            return await _conexion.QueryFirstOrDefaultAsync<Usuario>(sql, new { Id = id });
        }

        public async Task<int> CrearUsuario(Usuario usuario)
        {
            const string sql = @"INSERT INTO Usuario (Nombre, Email, contraseña, Activo, FechaCreacion)
                             VALUES (@Nombre, @Email, @PasswordHash, @Activo, @FechaCreacion);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await _conexion.ExecuteScalarAsync<int>(sql, usuario);
        }

    }
}
