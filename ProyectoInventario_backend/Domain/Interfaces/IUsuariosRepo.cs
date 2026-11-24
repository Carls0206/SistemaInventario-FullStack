using ProyectoInventario.Dominio.Entidades;

namespace ProyectoInventario.Domain.Interfaces
{
    public interface IUsuariosRepo
    {
        Task<Usuario?> ObtenerPorCorreoAsync(string email);
        Task<Usuario?> ObtenerPorIdAsync(int id);
        Task<int> CrearUsuario(Usuario usuario);
    }
}
