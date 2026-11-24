using ProyectoInventario.Application.DTOs.Auth;
using ProyectoInventario.Domain.Entities;

namespace ProyectoInventario.Application.Interfaces
{
    public interface ITokenService
    {
        (string Token, DateTime Expiration) GenerateToken(Usuario usuario);
    }

    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }

    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<UsuarioDto?> RegisterAsync(RegisterRequestDto request);
    }
}