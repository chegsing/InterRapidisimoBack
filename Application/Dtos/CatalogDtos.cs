namespace InterRapidisimoBack.Application.Dtos
{
    public class SubjectCatalogDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public byte Credits { get; set; }
        public string Professor { get; set; } = string.Empty;
    }

    public class AcademicProgramDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}