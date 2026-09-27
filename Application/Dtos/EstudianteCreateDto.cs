using System.ComponentModel.DataAnnotations;

namespace InterRapidisimoBack.Application.Dtos
{
    public class EstudianteCreateDto
    {
        [Required, StringLength(30)]
        public string IdentificationNumber { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(254)]
        public string Email { get; set; } = string.Empty;

        [StringLength(30)]
        public string? Phone { get; set; }

        [Range(1, int.MaxValue)]
        public int AcademicProgramId { get; set; }

        [Required, MinLength(3), MaxLength(3)]
        public List<int> SubjectIds { get; set; } = [];
    }
}
