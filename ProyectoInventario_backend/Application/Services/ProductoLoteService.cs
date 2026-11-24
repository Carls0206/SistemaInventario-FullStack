using ProyectoInventario.Application.DTOs;
using ProyectoInventario.Application.Interfaces;
using ProyectoInventario.Domain.Entities;
using ProyectoInventario.Domain.Interfaces;

namespace ProyectoInventario.Application.Services
{
    public class ProductoLoteService : IProductoLoteService
    {
        private readonly IProductoLotesRepo _lotes;
        private readonly IProductosRepo _productos;

        public ProductoLoteService(IProductoLotesRepo lotes, IProductosRepo productos)
        {
            _lotes = lotes;
            _productos = productos;
        }

        public async Task<IEnumerable<ProductoLoteResponseDto>> GetByProductoIdAsync(int productoId)
        {
            var lotes = await _lotes.GetByProductoIdAsync(productoId);
            return lotes.Select(l => new ProductoLoteResponseDto(l.Id, l.ProductoId, l.NumeroLote, l.FechaIngreso, l.PrecioUnitario, l.Cantidad));
        }

        public async Task<ProductoLoteResponseDto?> GetByIdAsync(int id)
        {
            var l = await _lotes.GetByIdAsync(id);
            return l is null ? null : new ProductoLoteResponseDto(l.Id, l.ProductoId, l.NumeroLote, l.FechaIngreso, l.PrecioUnitario, l.Cantidad);
        }

        public async Task<ProductoLoteResponseDto> CreateAsync(ProductoLoteCreateDto dto)
        {
            var producto = await _productos.GetByIdAsync(dto.ProductoId);
            if (producto is null)
                throw new KeyNotFoundException($"No existe el producto con Id {dto.ProductoId}.");

            var lote = new ProductoLote
            {
                ProductoId = dto.ProductoId,
                NumeroLote = dto.NumeroLote,
                FechaIngreso = dto.FechaIngreso,
                PrecioUnitario = dto.PrecioUnitario,
                Cantidad = dto.Cantidad
            };

            var id = await _lotes.CrearLoteAsync(lote);
            lote.Id = id;

            return new ProductoLoteResponseDto(lote.Id, lote.ProductoId, lote.NumeroLote, lote.FechaIngreso, lote.PrecioUnitario, lote.Cantidad);
        }

        public async Task<bool> UpdateAsync(int id, ProductoLoteUpdateDto dto)
        {
            var existente = await _lotes.GetByIdAsync(id);
            if (existente is null) return false;

            existente.NumeroLote = dto.NumeroLote;
            existente.FechaIngreso = dto.FechaIngreso;
            existente.PrecioUnitario = dto.PrecioUnitario;
            existente.Cantidad = dto.Cantidad;

            return await _lotes.ActualizarLoteAsync(existente);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _lotes.EliminarLoteAsync(id);
        }
    }
}