using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUsuariosRepo usuarios, IPasswordHasher hasher, ITokenService token, ILogger<AuthService> logger)
        {
            _usuarios = usuarios;
            _hasher = hasher;
            _token = token;
            _logger = logger;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var usuario = await _usuarios.ObtenerPorCorreoAsync(request.Email);
            if (usuario is null || usuario.Estado != "A")
            {
                _logger.LogWarning("Intento de login fallido para {Email}: usuario no encontrado o inactivo", request.Email);
                return null;
            }

            if (!_hasher.Verify(request.Password, usuario.PasswordHash))
            {
                _logger.LogWarning("Intento de login fallido para {Email}: contraseña inválida", request.Email);
                return null;
            }

            var (token, expiration) = _token.GenerateToken(usuario);
            _logger.LogInformation("Login exitoso para {Email} con rol {Rol}", usuario.Email, usuario.Rol);
            return new LoginResponseDto(token, expiration, new UsuarioDto(usuario.Id, usuario.Nombre, usuario.Email, usuario.Rol));
        }

        public async Task<UsuarioDto?> RegisterAsync(RegisterRequestDto request)
        {
            var existente = await _usuarios.ObtenerPorCorreoAsync(request.Email);
            if (existente is not null)
            {
                _logger.LogWarning("Registro rechazado: el correo {Email} ya existe", request.Email);
                return null;
            }

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

            _logger.LogInformation("Usuario registrado: {Email} con Id {UsuarioId}", usuario.Email, usuario.Id);
            return new UsuarioDto(id, usuario.Nombre, usuario.Email, usuario.Rol);
        }
    }
}
