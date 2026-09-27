using InterRapidisimoBack.Domain;
using Microsoft.EntityFrameworkCore;

namespace InterRapidisimoBack.Infraestructure
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<Profesor> Profesores { get; set; }
        public DbSet<ProgramaCredito> ProgramasCredito { get; set; }
        public DbSet<EstudianteMateria> EstudiantesMaterias { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            const string defaultTimestamp = "SYSUTCDATETIME()";

            modelBuilder.Entity<ProgramaCredito>(entity =>
            {
                entity.ToTable("AcademicPrograms");
                entity.HasKey(program => program.Id);
                entity.Property(program => program.Name).HasMaxLength(150).IsRequired();
                entity.Property(program => program.Description).HasMaxLength(500);
                entity.Property(program => program.IsActive).HasDefaultValue(true);
                entity.Property(program => program.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql(defaultTimestamp);
                entity.Property(program => program.UpdatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql(defaultTimestamp);
                entity.HasIndex(program => program.Name).IsUnique();
                entity.HasData(
                    new ProgramaCredito { Id = 1, Name = "Ingeniería de Sistemas", Description = "Programa académico de Ingeniería de Sistemas", CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                    new ProgramaCredito { Id = 2, Name = "Ingeniería Industrial", Description = "Programa académico de Ingeniería Industrial", CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) });
            });

            modelBuilder.Entity<Profesor>(entity =>
            {
                entity.ToTable("Professors");
                entity.HasKey(professor => professor.Id);
                entity.Property(professor => professor.IdentificationNumber).HasMaxLength(30).IsRequired();
                entity.Property(professor => professor.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(professor => professor.LastName).HasMaxLength(100).IsRequired();
                entity.Property(professor => professor.Email).HasMaxLength(254).IsRequired();
                entity.Property(professor => professor.IsActive).HasDefaultValue(true);
                entity.Property(professor => professor.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql(defaultTimestamp);
                entity.Property(professor => professor.UpdatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql(defaultTimestamp);
                entity.HasIndex(professor => professor.IdentificationNumber).IsUnique();
                entity.HasIndex(professor => professor.Email).IsUnique();
                entity.HasData(
                    new Profesor { Id = 1, IdentificationNumber = "P1001", FirstName = "Carlos", LastName = "Rodríguez", Email = "carlos.rodriguez@universidad.edu", CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                    new Profesor { Id = 2, IdentificationNumber = "P1002", FirstName = "María", LastName = "Gómez", Email = "maria.gomez@universidad.edu", CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                    new Profesor { Id = 3, IdentificationNumber = "P1003", FirstName = "Juan", LastName = "Martínez", Email = "juan.martinez@universidad.edu", CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                    new Profesor { Id = 4, IdentificationNumber = "P1004", FirstName = "Ana", LastName = "López", Email = "ana.lopez@universidad.edu", CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                    new Profesor { Id = 5, IdentificationNumber = "P1005", FirstName = "Pedro", LastName = "Torres", Email = "pedro.torres@universidad.edu", CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) });
            });

            modelBuilder.Entity<Estudiante>(entity =>
            {
                entity.ToTable("Students");
                entity.HasKey(student => student.Id);
                entity.Property(student => student.IdentificationNumber).HasMaxLength(30).IsRequired();
                entity.Property(student => student.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(student => student.LastName).HasMaxLength(100).IsRequired();
                entity.Property(student => student.Email).HasMaxLength(254).IsRequired();
                entity.Property(student => student.Phone).HasMaxLength(30);
                entity.Property(student => student.IsActive).HasDefaultValue(true);
                entity.Property(student => student.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql(defaultTimestamp);
                entity.Property(student => student.UpdatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql(defaultTimestamp);
                entity.HasIndex(student => student.IdentificationNumber).IsUnique();
                entity.HasIndex(student => student.Email).IsUnique();
                entity.HasIndex(student => student.AcademicProgramId);
                entity.HasOne(student => student.AcademicProgram)
                    .WithMany(program => program.Students)
                    .HasForeignKey(student => student.AcademicProgramId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Materia>(entity =>
            {
                entity.ToTable("Subjects", table => table.HasCheckConstraint("CK_Subjects_Credits", "Credits = 3"));
                entity.HasKey(subject => subject.Id);
                entity.Property(subject => subject.Code).HasMaxLength(30).IsRequired();
                entity.Property(subject => subject.Name).HasMaxLength(150).IsRequired();
                entity.Property(subject => subject.Credits).HasColumnType("tinyint").HasDefaultValue((byte)3);
                entity.Property(subject => subject.IsActive).HasDefaultValue(true);
                entity.Property(subject => subject.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql(defaultTimestamp);
                entity.Property(subject => subject.UpdatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql(defaultTimestamp);
                entity.HasIndex(subject => subject.Code).IsUnique();
                entity.HasIndex(subject => subject.ProfessorId);
                entity.HasOne(subject => subject.Professor)
                    .WithMany(professor => professor.Subjects)
                    .HasForeignKey(subject => subject.ProfessorId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasData(
                    new Materia { Id = 1, Code = "SUB001", Name = "Programación I", ProfessorId = 1, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                    new Materia { Id = 2, Code = "SUB002", Name = "Bases de Datos", ProfessorId = 1, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                    new Materia { Id = 3, Code = "SUB003", Name = "Programación II", ProfessorId = 2, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                    new Materia { Id = 4, Code = "SUB004", Name = "Ingeniería de Software", ProfessorId = 2, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                    new Materia { Id = 5, Code = "SUB005", Name = "Sistemas Operativos", ProfessorId = 3, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                    new Materia { Id = 6, Code = "SUB006", Name = "Redes de Computadores", ProfessorId = 3, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                    new Materia { Id = 7, Code = "SUB007", Name = "Arquitectura de Software", ProfessorId = 4, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                    new Materia { Id = 8, Code = "SUB008", Name = "Seguridad Informática", ProfessorId = 4, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                    new Materia { Id = 9, Code = "SUB009", Name = "Inteligencia Artificial", ProfessorId = 5, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                    new Materia { Id = 10, Code = "SUB010", Name = "Computación en la Nube", ProfessorId = 5, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) });
            });

            modelBuilder.Entity<EstudianteMateria>(entity =>
            {
                entity.ToTable("StudentSubjects");
                entity.HasKey(registration => new { registration.EstudianteId, registration.MateriaId });
                entity.Property(registration => registration.EstudianteId).HasColumnName("StudentId");
                entity.Property(registration => registration.MateriaId).HasColumnName("SubjectId");
                entity.Property(registration => registration.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql(defaultTimestamp);
                entity.HasIndex(registration => new { registration.MateriaId, registration.EstudianteId });
                entity.HasOne(registration => registration.Estudiante)
                    .WithMany(student => student.StudentSubjects)
                    .HasForeignKey(registration => registration.EstudianteId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(registration => registration.Materia)
                    .WithMany(subject => subject.StudentSubjects)
                    .HasForeignKey(registration => registration.MateriaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
