using ProyectoInventario.Application.DTOs.Auth;
using ProyectoInventario.Application.Interfaces;
using ProyectoInventario.Domain.Entities;
using ProyectoInventario.Domain.Interfaces;

namespace ProyectoInventario.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuariosRepo _usuarios;
        private readonly IPasswordHasher _hasher;
        private readonly ITokenService _token;

        public AuthService(IUsuariosRepo usuarios, IPasswordHasher hasher, ITokenService token)
        {
            _usuarios = usuarios;
            _hasher = hasher;
            _token = token;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var usuario = await _usuarios.ObtenerPorCorreoAsync(request.Email);
            if (usuario is null || usuario.Estado != "A")
                return null;

            if (!_hasher.Verify(request.Password, usuario.PasswordHash))
                return null;

            var (token, expiration) = _token.GenerateToken(usuario);
            return new LoginResponseDto(token, expiration, new UsuarioDto(usuario.Id, usuario.Nombre, usuario.Email, usuario.Rol));
        }

        public async Task<UsuarioDto?> RegisterAsync(RegisterRequestDto request)
        {
            var existente = await _usuarios.ObtenerPorCorreoAsync(request.Email);
            if (existente is not null)
                return null;

            var usuario = new Usuario
            {
                Nombre = request.Nombre,
                Email = request.Email,
                PasswordHash = _hasher.Hash(request.Password),
                Estado = "A",
                Rol = "User",
                FechaCreacion = DateTime.UtcNow
            };

            var id = await _usuarios.CrearUsuario(usuario);
            usuario.Id = id;

            return new UsuarioDto(id, usuario.Nombre, usuario.Email, usuario.Rol);
        }
    }
}
