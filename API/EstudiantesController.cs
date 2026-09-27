using InterRapidisimoBack.Application;
using InterRapidisimoBack.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace InterRapidisimoBack.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstudiantesController : ControllerBase
    {
        private readonly EstudianteService _service;

        public EstudiantesController(EstudianteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<StudentResponseDto>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentResponseDto>> GetById(int id)
        {
            var student = await _service.GetByIdAsync(id);
            return student is null ? NotFound() : Ok(student);
        }

        [HttpPost]
        public async Task<ActionResult<StudentResponseDto>> Create(EstudianteCreateDto request)
        {
            var student = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, EstudianteCreateDto request)
        {
            return await _service.UpdateAsync(id, request) ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _service.DeleteAsync(id) ? NoContent() : NotFound();
        }

        [HttpGet("{id}/companeros")]
        public async Task<IActionResult> GetClassmates(int id)
        {
            var classmates = await _service.GetClassmatesAsync(id);
            return classmates is null ? NotFound() : Ok(classmates);
        }
    }
}
