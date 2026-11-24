using ProyectoInventario.Application.DTOs;
using ProyectoInventario.Application.Interfaces;
using ProyectoInventario.Domain.Entities;
using ProyectoInventario.Domain.Interfaces;

namespace ProyectoInventario.Application.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductosRepo _productos;
        private readonly IProductoLotesRepo _lotes;

        public ProductoService(IProductosRepo productos, IProductoLotesRepo lotes)
        {
            _productos = productos;
            _lotes = lotes;
        }

        public async Task<IEnumerable<ProductoResponseDto>> GetAllAsync()
        {
            var productos = await _productos.GetAllAsync();
            return productos.Select(p => new ProductoResponseDto(p.Id, p.Codigo, p.Nombre, p.Descripcion, p.Precio, p.Estado));
        }

        public async Task<ProductoResponseDto?> GetByIdAsync(int id)
        {
            var p = await _productos.GetByIdAsync(id);
            return p is null ? null : new ProductoResponseDto(p.Id, p.Codigo, p.Nombre, p.Descripcion, p.Precio, p.Estado);
        }

        public async Task<ProductoDetalleDto?> GetDetalleAsync(int id)
        {
            var p = await _productos.GetByIdAsync(id);
            if (p is null) return null;

            var lotes = await _lotes.GetByProductoIdAsync(id);
            var lotesDto = lotes.Select(l => new ProductoLoteResponseDto(
                l.Id, l.ProductoId, l.LoteId, l.FechaIngreso, l.PrecioUnitario, l.Cantidad, l.Estado)).ToList();

            return new ProductoDetalleDto(p.Id, p.Codigo, p.Nombre, p.Descripcion, p.Precio, p.Estado, lotesDto);
        }

        public async Task<ProductoResponseDto> CreateAsync(ProductoCreateDto dto)
        {
            var producto = new Producto
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                Estado = "A"
            };

            var id = await _productos.CrearProductoAsync(producto);
            producto.Id = id;

            return new ProductoResponseDto(producto.Id, producto.Codigo, producto.Nombre, producto.Descripcion, producto.Precio, producto.Estado);
        }

        public async Task<bool> UpdateAsync(int id, ProductoUpdateDto dto)
        {
            var existente = await _productos.GetByIdAsync(id);
            if (existente is null) return false;

            existente.Nombre = dto.Nombre;
            existente.Descripcion = dto.Descripcion;
            existente.Precio = dto.Precio;
            existente.Estado = dto.Estado;

            return await _productos.ActualizarProductoAsync(existente);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _productos.EliminarProductoAsync(id);
        }
    }
}
