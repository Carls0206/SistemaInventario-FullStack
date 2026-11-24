using Microsoft.Extensions.Logging;
using ProyectoInventario.Application.DTOs;
using ProyectoInventario.Application.Interfaces;
using ProyectoInventario.Domain.Entities;
using ProyectoInventario.Domain.Interfaces;

namespace ProyectoInventario.Application.Services
{
    public class LoteService : ILoteService
    {
        private readonly ILotesRepo _lotes;
        private readonly ILogger<LoteService> _logger;

        public LoteService(ILotesRepo lotes, ILogger<LoteService> logger)
        {
            _lotes = lotes;
            _logger = logger;
        }

        public async Task<IEnumerable<LoteResponseDto>> GetAllAsync()
        {
            var lotes = await _lotes.GetAllAsync();
            return lotes.Select(l => new LoteResponseDto(l.Id, l.NumeroLote, l.FechaCreacion, l.Estado));
        }

        public async Task<LoteResponseDto?> GetByIdAsync(int id)
        {
            var lote = await _lotes.GetByIdAsync(id);
            return lote is null ? null : new LoteResponseDto(lote.Id, lote.NumeroLote, lote.FechaCreacion, lote.Estado);
        }

        public async Task<LoteResponseDto> CreateAsync(LoteCreateDto dto)
        {
            var existente = await _lotes.GetByNumeroAsync(dto.NumeroLote);
            if (existente is not null)
            {
                if (existente.Estado != "N")
                {
                    _logger.LogWarning("Creación de lote rechazada: {NumeroLote} ya existe", dto.NumeroLote);
                    throw new InvalidOperationException($"Ya existe el lote con número {dto.NumeroLote}.");
                }

                existente.Estado = "A";
                existente.NumeroLote = dto.NumeroLote;
                await _lotes.ActualizarAsync(existente);
                _logger.LogInformation("Lote reactivado: {NumeroLote} Id {LoteId}", existente.NumeroLote, existente.Id);
                return new LoteResponseDto(existente.Id, existente.NumeroLote, existente.FechaCreacion, existente.Estado);
            }

            var lote = new Lote
            {
                NumeroLote = dto.NumeroLote,
                FechaCreacion = DateTime.UtcNow,
                Estado = "A"
            };

            lote.Id = await _lotes.CrearAsync(lote);
            _logger.LogInformation("Lote creado: {NumeroLote} Id {LoteId}", lote.NumeroLote, lote.Id);
            return new LoteResponseDto(lote.Id, lote.NumeroLote, lote.FechaCreacion, lote.Estado);
        }

        public async Task<bool> UpdateAsync(int id, LoteUpdateDto dto)
        {
            var existente = await _lotes.GetByIdAsync(id);
            if (existente is null)
            {
                _logger.LogWarning("Actualización de lote fallida: Id {LoteId} no existe", id);
                return false;
            }

            var duplicado = await _lotes.GetByNumeroAsync(dto.NumeroLote);
            if (duplicado is not null && duplicado.Id != id && duplicado.Estado != "N")
            {
                _logger.LogWarning("Actualización de lote rechazada: {NumeroLote} ya existe", dto.NumeroLote);
                throw new InvalidOperationException($"Ya existe el lote con número {dto.NumeroLote}.");
            }

            existente.NumeroLote = dto.NumeroLote;
            existente.Estado = dto.Estado;
            var ok = await _lotes.ActualizarAsync(existente);
            if (ok) _logger.LogInformation("Lote actualizado: Id {LoteId}", id);
            return ok;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ok = await _lotes.EliminarAsync(id);
            if (ok) _logger.LogInformation("Lote eliminado lógicamente: Id {LoteId}", id);
            else _logger.LogWarning("Eliminación de lote fallida: Id {LoteId} no existe", id);
            return ok;
        }
    }
}
