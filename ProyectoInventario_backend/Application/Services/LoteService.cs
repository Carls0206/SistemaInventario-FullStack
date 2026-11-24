using ProyectoInventario.Application.DTOs;
using ProyectoInventario.Application.Interfaces;
using ProyectoInventario.Domain.Entities;
using ProyectoInventario.Domain.Interfaces;

namespace ProyectoInventario.Application.Services
{
    public class LoteService : ILoteService
    {
        private readonly ILotesRepo _lotes;

        public LoteService(ILotesRepo lotes)
        {
            _lotes = lotes;
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
                    throw new InvalidOperationException($"Ya existe el lote con número {dto.NumeroLote}.");

                existente.Estado = "A";
                existente.NumeroLote = dto.NumeroLote;
                await _lotes.ActualizarAsync(existente);
                return new LoteResponseDto(existente.Id, existente.NumeroLote, existente.FechaCreacion, existente.Estado);
            }

            var lote = new Lote
            {
                NumeroLote = dto.NumeroLote,
                FechaCreacion = DateTime.UtcNow,
                Estado = "A"
            };

            lote.Id = await _lotes.CrearAsync(lote);
            return new LoteResponseDto(lote.Id, lote.NumeroLote, lote.FechaCreacion, lote.Estado);
        }

        public async Task<bool> UpdateAsync(int id, LoteUpdateDto dto)
        {
            var existente = await _lotes.GetByIdAsync(id);
            if (existente is null) return false;

            var duplicado = await _lotes.GetByNumeroAsync(dto.NumeroLote);
            if (duplicado is not null && duplicado.Id != id && duplicado.Estado != "N")
                throw new InvalidOperationException($"Ya existe el lote con número {dto.NumeroLote}.");

            existente.NumeroLote = dto.NumeroLote;
            existente.Estado = dto.Estado;
            return await _lotes.ActualizarAsync(existente);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _lotes.EliminarAsync(id);
        }
    }
}
