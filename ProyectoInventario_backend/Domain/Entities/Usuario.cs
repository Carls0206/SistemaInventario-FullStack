namespace ProyectoInventario.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Estado { get; set; } = "A";
        public string Rol { get; set; } = "User";
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
