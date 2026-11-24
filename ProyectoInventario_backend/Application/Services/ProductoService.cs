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
            return productos.Select(p => new ProductoResponseDto(p.Id, p.Codigo, p.Nombre, p.Descripcion, p.Activo));
        }

        public async Task<ProductoResponseDto?> GetByIdAsync(int id)
        {
            var p = await _productos.GetByIdAsync(id);
            return p is null ? null : new ProductoResponseDto(p.Id, p.Codigo, p.Nombre, p.Descripcion, p.Activo);
        }

        public async Task<ProductoDetalleDto?> GetDetalleAsync(int id)
        {
            var p = await _productos.GetByIdAsync(id);
            if (p is null) return null;

            var lotes = await _lotes.GetByProductoIdAsync(id);
            var lotesDto = lotes.Select(l => new ProductoLoteResponseDto(
                l.Id, l.ProductoId, l.NumeroLote, l.FechaIngreso, l.PrecioUnitario, l.Cantidad)).ToList();

            return new ProductoDetalleDto(p.Id, p.Codigo, p.Nombre, p.Descripcion, p.Activo, lotesDto);
        }

        public async Task<ProductoResponseDto> CreateAsync(ProductoCreateDto dto)
        {
            var producto = new Producto
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Activo = true
            };

            var id = await _productos.CrearProductoAsync(producto);
            producto.Id = id;

            return new ProductoResponseDto(producto.Id, producto.Codigo, producto.Nombre, producto.Descripcion, producto.Activo);
        }

        public async Task<bool> UpdateAsync(int id, ProductoUpdateDto dto)
        {
            var existente = await _productos.GetByIdAsync(id);
            if (existente is null) return false;

            existente.Nombre = dto.Nombre;
            existente.Descripcion = dto.Descripcion;
            existente.Activo = dto.Activo;

            return await _productos.ActualizarProductoAsync(existente);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _productos.EliminarProductoAsync(id);
        }
    }
}