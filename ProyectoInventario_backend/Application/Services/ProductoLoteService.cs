using Microsoft.Extensions.Logging;
using ProyectoInventario.Application.DTOs;
using ProyectoInventario.Application.Interfaces;
using ProyectoInventario.Domain.Entities;
using ProyectoInventario.Domain.Interfaces;

namespace ProyectoInventario.Application.Services
{
    public class ProductoLoteService : IProductoLoteService
    {
        private readonly IProductoLotesRepo _productoLotes;
        private readonly IProductosRepo _productos;
        private readonly ILotesRepo _lotes;
        private readonly ILogger<ProductoLoteService> _logger;

        public ProductoLoteService(IProductoLotesRepo productoLotes, IProductosRepo productos, ILotesRepo lotes, ILogger<ProductoLoteService> logger)
        {
            _productoLotes = productoLotes;
            _productos = productos;
            _lotes = lotes;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductoLoteResponseDto>> GetAllAsync()
        {
            var lotes = await _productoLotes.GetAllAsync();
            return lotes.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductoLoteResponseDto>> GetByProductoIdAsync(int productoId)
        {
            var lotes = await _productoLotes.GetByProductoIdAsync(productoId);
            return lotes.Select(MapToDto);
        }

        public async Task<ProductoLoteResponseDto?> GetByIdAsync(int id)
        {
            var lote = await _productoLotes.GetByIdAsync(id);
            return lote is null ? null : MapToDto(lote);
        }

        public async Task<ProductoLoteResponseDto> CreateAsync(ProductoLoteCreateDto dto)
        {
            var producto = await _productos.GetByIdAsync(dto.ProductoId);
            if (producto is null)
            {
                _logger.LogWarning("Creación de entrada fallida: producto {ProductoId} no existe", dto.ProductoId);
                throw new KeyNotFoundException($"No existe el producto con Id {dto.ProductoId}.");
            }

            var lote = await _lotes.GetByIdAsync(dto.LoteId);
            if (lote is null)
            {
                _logger.LogWarning("Creación de entrada fallida: lote {LoteId} no existe", dto.LoteId);
                throw new KeyNotFoundException($"No existe el lote con Id {dto.LoteId}.");
            }

            var entity = new ProductoLote
            {
                ProductoId = dto.ProductoId,
                LoteId = dto.LoteId,
                FechaIngreso = dto.FechaIngreso,
                PrecioUnitario = dto.PrecioUnitario,
                Cantidad = dto.Cantidad,
                Estado = "A"
            };

            entity.Id = await _productoLotes.CrearLoteAsync(entity);
            _logger.LogInformation("Entrada de lote creada: Id {ProductoLoteId} Producto {ProductoId} Lote {LoteId}", entity.Id, entity.ProductoId, entity.LoteId);
            return MapToDto(entity);
        }

        public async Task<bool> UpdateAsync(int id, ProductoLoteUpdateDto dto)
        {
            var existente = await _productoLotes.GetByIdAsync(id);
            if (existente is null)
            {
                _logger.LogWarning("Actualización de entrada fallida: Id {ProductoLoteId} no existe", id);
                return false;
            }

            var lote = await _lotes.GetByIdAsync(dto.LoteId);
            if (lote is null)
            {
                _logger.LogWarning("Actualización de entrada fallida: lote {LoteId} no existe", dto.LoteId);
                throw new KeyNotFoundException($"No existe el lote con Id {dto.LoteId}.");
            }

            existente.LoteId = dto.LoteId;
            existente.FechaIngreso = dto.FechaIngreso;
            existente.PrecioUnitario = dto.PrecioUnitario;
            existente.Cantidad = dto.Cantidad;
            existente.Estado = dto.Estado;

            var ok = await _productoLotes.ActualizarLoteAsync(existente);
            if (ok) _logger.LogInformation("Entrada de lote actualizada: Id {ProductoLoteId}", id);
            return ok;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ok = await _productoLotes.EliminarLoteAsync(id);
            if (ok) _logger.LogInformation("Entrada de lote eliminada lógicamente: Id {ProductoLoteId}", id);
            else _logger.LogWarning("Eliminación de entrada fallida: Id {ProductoLoteId} no existe", id);
            return ok;
        }

        private static ProductoLoteResponseDto MapToDto(ProductoLote lote)
        {
            return new ProductoLoteResponseDto(
                lote.Id,
                lote.ProductoId,
                lote.LoteId,
                lote.FechaIngreso,
                lote.PrecioUnitario,
                lote.Cantidad,
                lote.Estado);
        }
    }
}
