namespace InterRapidisimoBack.Domain
{
    public class EstudianteMateria
    {
        public int EstudianteId { get; set; }
        public int MateriaId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Estudiante Estudiante { get; set; } = null!;
        public Materia Materia { get; set; } = null!;
    }
}
