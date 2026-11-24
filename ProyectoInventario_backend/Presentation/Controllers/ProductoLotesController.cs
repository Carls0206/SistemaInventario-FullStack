using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoInventario.Application.DTOs;
using ProyectoInventario.Application.Interfaces;

namespace ProyectoInventario.Presentation.Controllers
{
    [ApiController]
    [Route("api/producto-lotes")]
    [Authorize]
    public class ProductoLotesController : ControllerBase
    {
        private readonly IProductoLoteService _service;

        public ProductoLotesController(IProductoLoteService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoLoteResponseDto>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("producto/{productoId:int}")]
        public async Task<ActionResult<IEnumerable<ProductoLoteResponseDto>>> GetByProducto(int productoId)
        {
            return Ok(await _service.GetByProductoIdAsync(productoId));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductoLoteResponseDto>> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<ProductoLoteResponseDto>> Create([FromBody] ProductoLoteCreateDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponseDto(ex.Message, null, HttpContext.TraceIdentifier));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductoLoteUpdateDto dto)
        {
            try
            {
                var ok = await _service.UpdateAsync(id, dto);
                return ok ? NoContent() : NotFound();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponseDto(ex.Message, null, HttpContext.TraceIdentifier));
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
