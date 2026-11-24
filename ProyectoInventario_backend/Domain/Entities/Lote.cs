namespace ProyectoInventario.Domain.Entities
{
    public class Lote
    {
        public int Id { get; set; }
        public string NumeroLote { get; set; } = string.Empty;
        public DateTime? FechaCreacion { get; set; }
        public string Estado { get; set; } = "A";
    }
}
