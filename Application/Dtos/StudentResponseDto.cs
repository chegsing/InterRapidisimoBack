namespace InterRapidisimoBack.Application.Dtos
{
    public class StudentResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string AcademicProgram { get; set; } = string.Empty;
        public IReadOnlyList<string> Subjects { get; set; } = [];
    }

    public class ClassmatesBySubjectDto
    {
        public string Subject { get; set; } = string.Empty;
        public IReadOnlyList<StudentNameDto> Classmates { get; set; } = [];
    }

    public class StudentNameDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}