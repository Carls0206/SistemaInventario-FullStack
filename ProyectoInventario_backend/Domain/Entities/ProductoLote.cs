namespace ProyectoInventario.Domain.Entities
{
    public class ProductoLote
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int LoteId { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public string Estado { get; set; } = "A";
    }
}
