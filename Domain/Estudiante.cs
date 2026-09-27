namespace InterRapidisimoBack.Domain
{
    public class Estudiante
    {
        public int Id { get; set; }
        public string IdentificationNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int AcademicProgramId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ProgramaCredito AcademicProgram { get; set; } = null!;
        public ICollection<EstudianteMateria> StudentSubjects { get; set; } = new List<EstudianteMateria>();
    }
}
