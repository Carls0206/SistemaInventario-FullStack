using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoInventario.Application.DTOs.Auth;
using ProyectoInventario.Application.Interfaces;

namespace ProyectoInventario.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            var result = await _auth.LoginAsync(request);
            if (result is null) return Unauthorized(new { mensaje = "Credenciales inválidas" });
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UsuarioDto>> Register([FromBody] RegisterRequestDto request)
        {
            var creado = await _auth.RegisterAsync(request);
            if (creado is null)
                return Conflict(new { mensaje = "El correo ya está registrado." });

            return Created(string.Empty, creado);
        }
    }
}
