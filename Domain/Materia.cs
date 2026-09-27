namespace InterRapidisimoBack.Domain
{
    public class Materia
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public byte Credits { get; set; } = 3;
        public int ProfessorId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Profesor Professor { get; set; } = null!;
        public ICollection<EstudianteMateria> StudentSubjects { get; set; } = new List<EstudianteMateria>();
    }
}
