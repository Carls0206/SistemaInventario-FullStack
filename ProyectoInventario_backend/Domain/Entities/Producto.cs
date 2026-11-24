namespace ProyectoInventario.Dominio.Entidades
{
    public class Producto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;
        public List<ProductoLote> Lotes { get; set; } = new();
    }
}
