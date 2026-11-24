using System.Data;
using Dapper;
using ProyectoInventario.Domain.Interfaces;
using ProyectoInventario.Domain.Entities;

namespace ProyectoInventario.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuariosRepo
    {
        private readonly IDbConnection _conexion;
        public UsuarioRepository(IDbConnection conn)
        {
            _conexion = conn;
        }

        public async Task<Usuario?> ObtenerPorCorreoAsync(string email)
        {
            const string sql = @"SELECT Id, Nombre, Email, contrasena AS PasswordHash, Estado, Rol, FechaCreacion
                             FROM Usuario WHERE Email = @Email";
            return await _conexion.QueryFirstOrDefaultAsync<Usuario>(sql, new { Email = email });
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            const string sql = @"SELECT Id, Nombre, Email, contrasena AS PasswordHash, Estado, Rol, FechaCreacion
                             FROM Usuario WHERE Id = @Id";
            return await _conexion.QueryFirstOrDefaultAsync<Usuario>(sql, new { Id = id });
        }

        public async Task<int> CrearUsuario(Usuario usuario)
        {
            const string sql = @"INSERT INTO Usuario (Nombre, Email, contrasena, Estado, Rol, FechaCreacion)
                             VALUES (@Nombre, @Email, @PasswordHash, @Estado, @Rol, @FechaCreacion);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await _conexion.ExecuteScalarAsync<int>(sql, usuario);
        }
    }
}
