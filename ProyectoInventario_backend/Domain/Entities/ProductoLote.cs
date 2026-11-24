namespace ProyectoInventario.Dominio.Entidades
{
    public class ProductoLote
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string NumeroLote { get; set; } = string.Empty;
        public DateTime? FechaIngreso { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
    }
}

