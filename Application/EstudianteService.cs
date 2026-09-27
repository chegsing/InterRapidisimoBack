using InterRapidisimoBack.Domain;
using InterRapidisimoBack.Application.Dtos;
using InterRapidisimoBack.Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace InterRapidisimoBack.Application
{
    public class EstudianteService
    {
        private const int RequiredSubjectCount = 3;
        private readonly ApplicationDbContext _context;

        public EstudianteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<StudentResponseDto>> GetAllAsync()
        {
            var students = await _context.Estudiantes.AsNoTracking()
                .Include(student => student.AcademicProgram)
                .Include(student => student.StudentSubjects)
                    .ThenInclude(registration => registration.Materia)
                .OrderBy(student => student.LastName)
                .ThenBy(student => student.FirstName)
                .ToListAsync();

            return students.Select(ToResponse).ToList();
        }

        public async Task<StudentResponseDto?> GetByIdAsync(int id)
        {
            var student = await _context.Estudiantes.AsNoTracking()
                .Include(item => item.AcademicProgram)
                .Include(item => item.StudentSubjects)
                    .ThenInclude(registration => registration.Materia)
                .FirstOrDefaultAsync(item => item.Id == id);

            return student is null ? null : ToResponse(student);
        }

        public async Task<IReadOnlyList<ClassmatesBySubjectDto>?> GetClassmatesAsync(int studentId)
        {
            var student = await _context.Estudiantes.AsNoTracking()
                .Where(item => item.Id == studentId)
                .Select(item => new
                {
                    item.Id,
                    Subjects = item.StudentSubjects.Select(registration => new
                    {
                        registration.MateriaId,
                        registration.Materia.Name
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (student is null)
                return null;

            var subjectIds = student.Subjects.Select(subject => subject.MateriaId).ToList();
            var classmates = await _context.EstudiantesMaterias.AsNoTracking()
                .Where(registration => subjectIds.Contains(registration.MateriaId)
                    && registration.EstudianteId != studentId)
                .Select(registration => new
                {
                    registration.MateriaId,
                    registration.Estudiante.FirstName,
                    registration.Estudiante.LastName
                })
                .ToListAsync();

            return student.Subjects.Select(subject => new ClassmatesBySubjectDto
            {
                Subject = subject.Name,
                Classmates = classmates
                    .Where(classmate => classmate.MateriaId == subject.MateriaId)
                    .Select(classmate => new StudentNameDto
                    {
                        FirstName = classmate.FirstName,
                        LastName = classmate.LastName
                    })
                    .OrderBy(classmate => classmate.LastName)
                    .ThenBy(classmate => classmate.FirstName)
                    .ToList()
            }).ToList();
        }

        public async Task<StudentResponseDto> CreateAsync(EstudianteCreateDto request)
        {
            var subjects = await ValidateRegistrationAsync(request);
            var student = new Estudiante
            {
                IdentificationNumber = request.IdentificationNumber.Trim(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim(),
                Phone = request.Phone?.Trim(),
                AcademicProgramId = request.AcademicProgramId,
                StudentSubjects = subjects.Select(subject => new EstudianteMateria
                {
                    MateriaId = subject.Id
                }).ToList()
            };

            _context.Estudiantes.Add(student);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(student.Id))!;
        }

        public async Task<bool> UpdateAsync(int id, EstudianteCreateDto request)
        {
            var student = await _context.Estudiantes
                .Include(item => item.StudentSubjects)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (student is null)
                return false;

            var subjects = await ValidateRegistrationAsync(request, id);
            student.IdentificationNumber = request.IdentificationNumber.Trim();
            student.FirstName = request.FirstName.Trim();
            student.LastName = request.LastName.Trim();
            student.Email = request.Email.Trim();
            student.Phone = request.Phone?.Trim();
            student.AcademicProgramId = request.AcademicProgramId;
            student.UpdatedAt = DateTime.UtcNow;
            var requestedSubjectIds = subjects.Select(subject => subject.Id).ToHashSet();
            var removedRegistrations = student.StudentSubjects
                .Where(registration => !requestedSubjectIds.Contains(registration.MateriaId))
                .ToList();
            _context.EstudiantesMaterias.RemoveRange(removedRegistrations);

            foreach (var subject in subjects)
            {
                if (!student.StudentSubjects.Any(registration => registration.MateriaId == subject.Id))
                {
                    student.StudentSubjects.Add(new EstudianteMateria { MateriaId = subject.Id });
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var student = await _context.Estudiantes
                .Include(item => item.StudentSubjects)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (student is null)
                return false;

            _context.EstudiantesMaterias.RemoveRange(student.StudentSubjects);
            _context.Estudiantes.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<List<Materia>> ValidateRegistrationAsync(EstudianteCreateDto request, int? studentId = null)
        {
            if (request.SubjectIds.Count != RequiredSubjectCount
                || request.SubjectIds.Distinct().Count() != RequiredSubjectCount)
            {
                throw new BusinessRuleException("Debes seleccionar exactamente 3 materias diferentes.");
            }

            if (!await _context.ProgramasCredito.AnyAsync(program =>
                program.Id == request.AcademicProgramId && program.IsActive))
            {
                throw new BusinessRuleException("El programa académico no existe o está inactivo.");
            }

            var identificationNumber = request.IdentificationNumber.Trim();
            var email = request.Email.Trim();
            if (await _context.Estudiantes.AnyAsync(student => student.Id != studentId
                && (student.IdentificationNumber == identificationNumber || student.Email == email)))
            {
                throw new BusinessRuleException("El documento o correo ya está registrado.");
            }

            var subjects = await _context.Materias
                .Where(subject => request.SubjectIds.Contains(subject.Id) && subject.IsActive)
                .Include(subject => subject.Professor)
                .ToListAsync();

            if (subjects.Count != RequiredSubjectCount)
                throw new BusinessRuleException("Una o más materias no existen o están inactivas.");

            if (subjects.Select(subject => subject.ProfessorId).Distinct().Count() != RequiredSubjectCount)
                throw new BusinessRuleException("No puedes seleccionar materias dictadas por el mismo profesor.");

            return subjects;
        }

        private static StudentResponseDto ToResponse(Estudiante student) => new()
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            AcademicProgram = student.AcademicProgram.Name,
            Subjects = student.StudentSubjects
                .Select(registration => registration.Materia.Name)
                .OrderBy(name => name)
                .ToList()
        };
    }
}
