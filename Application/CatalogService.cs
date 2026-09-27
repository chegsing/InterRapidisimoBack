using InterRapidisimoBack.Application.Dtos;
using InterRapidisimoBack.Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace InterRapidisimoBack.Application
{
    public class CatalogService
    {
        private readonly ApplicationDbContext _context;

        public CatalogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<SubjectCatalogDto>> GetSubjectsAsync()
        {
            return await _context.Materias.AsNoTracking()
                .Where(subject => subject.IsActive)
                .OrderBy(subject => subject.Code)
                .Select(subject => new SubjectCatalogDto
                {
                    Id = subject.Id,
                    Code = subject.Code,
                    Name = subject.Name,
                    Credits = subject.Credits,
                    Professor = $"{subject.Professor.FirstName} {subject.Professor.LastName}"
                })
                .ToListAsync();
        }

        public async Task<IReadOnlyList<AcademicProgramDto>> GetProgramsAsync()
        {
            return await _context.ProgramasCredito.AsNoTracking()
                .Where(program => program.IsActive)
                .OrderBy(program => program.Name)
                .Select(program => new AcademicProgramDto
                {
                    Id = program.Id,
                    Name = program.Name,
                    Description = program.Description
                })
                .ToListAsync();
        }
    }
}