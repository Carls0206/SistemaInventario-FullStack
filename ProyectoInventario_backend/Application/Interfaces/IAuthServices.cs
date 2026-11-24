using ProyectoInventario.Application.DTOs.Auth;
using ProyectoInventario.Dominio.Entidades;

namespace ProyectoInventario.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(Usuario usuario);
        DateTime GetExpiration();
    }

    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }

    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    }
}
