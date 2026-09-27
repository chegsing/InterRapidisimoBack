using InterRapidisimoBack.Application;
using InterRapidisimoBack.Application.Dtos;
using InterRapidisimoBack.Domain;
using InterRapidisimoBack.Infraestructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InterRapidisimoBack.Tests;

public class EstudianteServiceTests
{
    [Fact]
    public async Task CreateAsync_RejectsSubjectsFromTheSameProfessor()
    {
        await using var context = CreateContext();
        var service = new EstudianteService(context);

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            service.CreateAsync(CreateRequest([1, 2, 3])));

        Assert.Contains("mismo profesor", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_RegistersThreeSubjectsWithoutExposingContactData()
    {
        await using var context = CreateContext();
        var service = new EstudianteService(context);

        var student = await service.CreateAsync(CreateRequest([1, 3, 5]));

        Assert.Equal(3, student.Subjects.Count);
        Assert.Equal("Laura", student.FirstName);
        Assert.Equal(3, await context.EstudiantesMaterias.CountAsync());
        Assert.DoesNotContain("Email", typeof(StudentResponseDto).GetProperties().Select(property => property.Name));
    }

    [Fact]
    public async Task GetClassmatesAsync_ReturnsNamesGroupedBySharedSubject()
    {
        await using var context = CreateContext();
        var service = new EstudianteService(context);
        var student = await service.CreateAsync(CreateRequest([1, 3, 5]));
        await service.CreateAsync(CreateRequest([1, 4, 6], "S1002", "Andrés", "Martínez", "andres@example.com"));

        var subjects = await service.GetClassmatesAsync(student.Id);

        Assert.NotNull(subjects);
        Assert.Equal(3, subjects.Count);
        Assert.Single(subjects.Single(subject => subject.Subject == "Programación I").Classmates);
        Assert.Empty(subjects.Single(subject => subject.Subject == "Programación II").Classmates);
    }

    [Fact]
    public async Task UpdateAsync_ReplacesSubjectsAndDeleteAsyncRemovesStudent()
    {
        await using var context = CreateContext();
        var service = new EstudianteService(context);
        var student = await service.CreateAsync(CreateRequest([1, 3, 5]));

        var updated = await service.UpdateAsync(student.Id, CreateRequest([1, 4, 5]));
        var updatedStudent = await service.GetByIdAsync(student.Id);
        var deleted = await service.DeleteAsync(student.Id);

        Assert.True(updated);
        Assert.NotNull(updatedStudent);
        Assert.Contains("Ingeniería de Software", updatedStudent.Subjects);
        Assert.DoesNotContain("Programación II", updatedStudent.Subjects);
        Assert.True(deleted);
        Assert.Null(await service.GetByIdAsync(student.Id));
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new ApplicationDbContext(options);

        context.ProgramasCredito.Add(new ProgramaCredito { Id = 1, Name = "Ingeniería" });
        for (var professorId = 1; professorId <= 5; professorId++)
        {
            context.Profesores.Add(new Profesor
            {
                Id = professorId,
                IdentificationNumber = $"P{professorId}",
                FirstName = $"Profesor{professorId}",
                LastName = "Prueba",
                Email = $"profesor{professorId}@example.com"
            });
        }

        var names = new[]
        {
            "Programación I", "Bases de Datos", "Programación II", "Ingeniería de Software",
            "Sistemas Operativos", "Redes", "Arquitectura", "Seguridad", "IA", "Nube"
        };
        for (var index = 0; index < names.Length; index++)
        {
            context.Materias.Add(new Materia
            {
                Id = index + 1,
                Code = $"SUB{index + 1:000}",
                Name = names[index],
                ProfessorId = (index / 2) + 1
            });
        }

        context.SaveChanges();
        return context;
    }

    private static EstudianteCreateDto CreateRequest(
        List<int> subjectIds,
        string identificationNumber = "S1001",
        string firstName = "Laura",
        string lastName = "García",
        string email = "laura@example.com") => new()
        {
            IdentificationNumber = identificationNumber,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            AcademicProgramId = 1,
            SubjectIds = subjectIds
        };
}