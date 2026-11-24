using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoInventario.Application.DTOs;
using ProyectoInventario.Application.Interfaces;

namespace ProyectoInventario.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _service;
        public ProductosController(IProductoService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoResponseDto>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }
            

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductoResponseDto>> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpGet("{id:int}/detalle")]
        public async Task<ActionResult<ProductoDetalleDto>> GetDetalle(int id)
        {
            var dto = await _service.GetDetalleAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<ProductoResponseDto>> Create([FromBody] ProductoCreateDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductoUpdateDto dto)
        {
            var ok = await _service.UpdateAsync(id, dto);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
