using System.ComponentModel.DataAnnotations;

namespace ProyectoInventario.Application.DTOs.Auth
{
    public record LoginRequestDto(string Email, string Password);

    public record LoginResponseDto(string Token, DateTime ExpiresAt, UsuarioDto Usuario);

    public record UsuarioDto(int Id, string Nombre, string Email, string Rol);

    public record RegisterRequestDto(string Nombre,string Email,string Password);
}