using InterRapidisimoBack.Application;
using InterRapidisimoBack.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace InterRapidisimoBack.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogosController : ControllerBase
    {
        private readonly CatalogService _service;

        public CatalogosController(CatalogService service)
        {
            _service = service;
        }

        [HttpGet("materias")]
        public async Task<ActionResult<IReadOnlyList<SubjectCatalogDto>>> GetSubjects()
        {
            return Ok(await _service.GetSubjectsAsync());
        }

        [HttpGet("programas-academicos")]
        public async Task<ActionResult<IReadOnlyList<AcademicProgramDto>>> GetPrograms()
        {
            return Ok(await _service.GetProgramsAsync());
        }
    }
}