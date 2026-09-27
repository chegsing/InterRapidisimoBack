/* ============================================================
   DATABASE
   ============================================================ */

IF DB_ID(N'StudentRegistrationDB') IS NULL
BEGIN
    CREATE DATABASE StudentRegistrationDB;
END
GO

USE StudentRegistrationDB;
GO

/* ============================================================
   CLEAN RE-CREATION
   Para un ambiente de desarrollo/pruebas.
   ============================================================ */

IF OBJECT_ID(N'dbo.trg_StudentSubjects_ValidateBusinessRules', N'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_StudentSubjects_ValidateBusinessRules;
GO

IF OBJECT_ID(N'dbo.trg_Subjects_ValidateProfessorLimit', N'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_Subjects_ValidateProfessorLimit;
GO

IF OBJECT_ID(N'dbo.trg_AcademicPrograms_UpdatedAt', N'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_AcademicPrograms_UpdatedAt;
GO

IF OBJECT_ID(N'dbo.trg_Students_UpdatedAt', N'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_Students_UpdatedAt;
GO

IF OBJECT_ID(N'dbo.trg_Professors_UpdatedAt', N'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_Professors_UpdatedAt;
GO

IF OBJECT_ID(N'dbo.trg_Subjects_UpdatedAt', N'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_Subjects_UpdatedAt;
GO

/* ============================================================
   TABLE: AcademicPrograms
   ============================================================ */

IF OBJECT_ID(N'dbo.StudentSubjects', N'U') IS NOT NULL
    DROP TABLE dbo.StudentSubjects;
GO

IF OBJECT_ID(N'dbo.Students', N'U') IS NOT NULL
    DROP TABLE dbo.Students;
GO

IF OBJECT_ID(N'dbo.Subjects', N'U') IS NOT NULL
    DROP TABLE dbo.Subjects;
GO

IF OBJECT_ID(N'dbo.Professors', N'U') IS NOT NULL
    DROP TABLE dbo.Professors;
GO

IF OBJECT_ID(N'dbo.AcademicPrograms', N'U') IS NOT NULL
    DROP TABLE dbo.AcademicPrograms;
GO

CREATE TABLE dbo.AcademicPrograms
(
    Id INT IDENTITY(1,1) NOT NULL,

    Name NVARCHAR(150) NOT NULL,

    Description NVARCHAR(500) NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_AcademicPrograms_IsActive
        DEFAULT (1),

    CreatedAt DATETIME2(3) NOT NULL
        CONSTRAINT DF_AcademicPrograms_CreatedAt
        DEFAULT (SYSUTCDATETIME()),

    UpdatedAt DATETIME2(3) NOT NULL
        CONSTRAINT DF_AcademicPrograms_UpdatedAt
        DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT PK_AcademicPrograms
        PRIMARY KEY CLUSTERED (Id),

    CONSTRAINT UQ_AcademicPrograms_Name
        UNIQUE (Name)
);
GO

/* ============================================================
   TABLE: Professors
   ============================================================ */

CREATE TABLE dbo.Professors
(
    Id INT IDENTITY(1,1) NOT NULL,

    IdentificationNumber NVARCHAR(30) NOT NULL,

    FirstName NVARCHAR(100) NOT NULL,

    LastName NVARCHAR(100) NOT NULL,

    Email NVARCHAR(254) NOT NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_Professors_IsActive
        DEFAULT (1),

    CreatedAt DATETIME2(3) NOT NULL
        CONSTRAINT DF_Professors_CreatedAt
        DEFAULT (SYSUTCDATETIME()),

    UpdatedAt DATETIME2(3) NOT NULL
        CONSTRAINT DF_Professors_UpdatedAt
        DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT PK_Professors
        PRIMARY KEY CLUSTERED (Id),

    CONSTRAINT UQ_Professors_IdentificationNumber
        UNIQUE (IdentificationNumber),

    CONSTRAINT UQ_Professors_Email
        UNIQUE (Email)
);
GO

/* ============================================================
   TABLE: Students
   ============================================================ */

CREATE TABLE dbo.Students
(
    Id INT IDENTITY(1,1) NOT NULL,

    IdentificationNumber NVARCHAR(30) NOT NULL,

    FirstName NVARCHAR(100) NOT NULL,

    LastName NVARCHAR(100) NOT NULL,

    Email NVARCHAR(254) NOT NULL,

    Phone NVARCHAR(30) NULL,

    AcademicProgramId INT NOT NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_Students_IsActive
        DEFAULT (1),

    CreatedAt DATETIME2(3) NOT NULL
        CONSTRAINT DF_Students_CreatedAt
        DEFAULT (SYSUTCDATETIME()),

    UpdatedAt DATETIME2(3) NOT NULL
        CONSTRAINT DF_Students_UpdatedAt
        DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT PK_Students
        PRIMARY KEY CLUSTERED (Id),

    CONSTRAINT UQ_Students_IdentificationNumber
        UNIQUE (IdentificationNumber),

    CONSTRAINT UQ_Students_Email
        UNIQUE (Email),

    CONSTRAINT FK_Students_AcademicPrograms
        FOREIGN KEY (AcademicProgramId)
        REFERENCES dbo.AcademicPrograms(Id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO

/* ============================================================
   TABLE: Subjects
   ============================================================ */

CREATE TABLE dbo.Subjects
(
    Id INT IDENTITY(1,1) NOT NULL,

    Code NVARCHAR(30) NOT NULL,

    Name NVARCHAR(150) NOT NULL,

    Credits TINYINT NOT NULL,

    ProfessorId INT NOT NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_Subjects_IsActive
        DEFAULT (1),

    CreatedAt DATETIME2(3) NOT NULL
        CONSTRAINT DF_Subjects_CreatedAt
        DEFAULT (SYSUTCDATETIME()),

    UpdatedAt DATETIME2(3) NOT NULL
        CONSTRAINT DF_Subjects_UpdatedAt
        DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT PK_Subjects
        PRIMARY KEY CLUSTERED (Id),

    CONSTRAINT UQ_Subjects_Code
        UNIQUE (Code),

    CONSTRAINT CK_Subjects_Credits
        CHECK (Credits = 3),

    CONSTRAINT FK_Subjects_Professors
        FOREIGN KEY (ProfessorId)
        REFERENCES dbo.Professors(Id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO

/* ============================================================
   TABLE: StudentSubjects
   ============================================================ */

CREATE TABLE dbo.StudentSubjects
(
    StudentId INT NOT NULL,

    SubjectId INT NOT NULL,

    CreatedAt DATETIME2(3) NOT NULL
        CONSTRAINT DF_StudentSubjects_CreatedAt
        DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT PK_StudentSubjects
        PRIMARY KEY CLUSTERED (StudentId, SubjectId),

    CONSTRAINT FK_StudentSubjects_Students
        FOREIGN KEY (StudentId)
        REFERENCES dbo.Students(Id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_StudentSubjects_Subjects
        FOREIGN KEY (SubjectId)
        REFERENCES dbo.Subjects(Id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO

/* ============================================================
   INDEXES
   ============================================================ */

CREATE INDEX IX_Students_AcademicProgramId
ON dbo.Students (AcademicProgramId);
GO

CREATE INDEX IX_Subjects_ProfessorId
ON dbo.Subjects (ProfessorId);
GO

CREATE INDEX IX_StudentSubjects_SubjectId_StudentId
ON dbo.StudentSubjects (SubjectId, StudentId);
GO


////////////////

CREATE TRIGGER dbo.trg_Subjects_ValidateProfessorLimit
ON dbo.Subjects
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Subjects s
        INNER JOIN
        (
            SELECT DISTINCT ProfessorId
            FROM inserted
        ) i
            ON i.ProfessorId = s.ProfessorId
        GROUP BY s.ProfessorId
        HAVING COUNT(*) > 2
    )
    BEGIN
        THROW 51001,
              'Un profesor no puede tener más de 2 materias.',
              1;
    END
END;
GO

///////////////
CREATE TRIGGER dbo.trg_StudentSubjects_ValidateBusinessRules
ON dbo.StudentSubjects
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    /*
        Bloqueamos las filas de estudiantes afectadas.

        UPDLOCK:
        reserva un bloqueo de actualización.

        HOLDLOCK:
        equivale a SERIALIZABLE para esta consulta.

        Esto evita que dos transacciones concurrentes
        puedan validar simultáneamente el mismo estudiante
        y ambas concluir que todavía tiene capacidad.
    */

    SELECT s.Id
    FROM dbo.Students s WITH (UPDLOCK, HOLDLOCK)
    INNER JOIN
    (
        SELECT DISTINCT StudentId
        FROM inserted
    ) i
        ON i.StudentId = s.Id;

    /* ========================================================
       REGLA: máximo 3 materias
       ======================================================== */

    IF EXISTS
    (
        SELECT 1
        FROM dbo.StudentSubjects ss
        INNER JOIN
        (
            SELECT DISTINCT StudentId
            FROM inserted
        ) i
            ON i.StudentId = ss.StudentId
        GROUP BY ss.StudentId
        HAVING COUNT(*) > 3
    )
    BEGIN
        THROW 51002,
              'Un estudiante no puede registrar más de 3 materias.',
              1;
    END;

    /* ========================================================
       REGLA: máximo 9 créditos
       ======================================================== */

    IF EXISTS
    (
        SELECT 1
        FROM dbo.StudentSubjects ss
        INNER JOIN dbo.Subjects s
            ON s.Id = ss.SubjectId
        INNER JOIN
        (
            SELECT DISTINCT StudentId
            FROM inserted
        ) i
            ON i.StudentId = ss.StudentId
        GROUP BY ss.StudentId
        HAVING SUM(s.Credits) > 9
    )
    BEGIN
        THROW 51003,
              'Un estudiante no puede superar 9 créditos.',
              1;
    END;

    /* ========================================================
       REGLA:
       Un estudiante no puede registrar dos materias
       impartidas por el mismo profesor.
       ======================================================== */

    IF EXISTS
    (
        SELECT 1
        FROM dbo.StudentSubjects ss
        INNER JOIN dbo.Subjects s
            ON s.Id = ss.SubjectId
        INNER JOIN
        (
            SELECT DISTINCT StudentId
            FROM inserted
        ) i
            ON i.StudentId = ss.StudentId
        GROUP BY
            ss.StudentId,
            s.ProfessorId
        HAVING COUNT(*) > 1
    )
    BEGIN
        THROW 51004,
              'Un estudiante no puede registrar dos materias impartidas por el mismo profesor.',
              1;
    END;
END;
GO


///////////////////

CREATE TRIGGER dbo.trg_AcademicPrograms_UpdatedAt
ON dbo.AcademicPrograms
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE ap
       SET UpdatedAt = SYSUTCDATETIME()
    FROM dbo.AcademicPrograms ap
    INNER JOIN inserted i
        ON i.Id = ap.Id;
END;
GO

CREATE TRIGGER dbo.trg_Students_UpdatedAt
ON dbo.Students
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE s
       SET UpdatedAt = SYSUTCDATETIME()
    FROM dbo.Students s
    INNER JOIN inserted i
        ON i.Id = s.Id;
END;
GO

CREATE TRIGGER dbo.trg_Professors_UpdatedAt
ON dbo.Professors
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE p
       SET UpdatedAt = SYSUTCDATETIME()
    FROM dbo.Professors p
    INNER JOIN inserted i
        ON i.Id = p.Id;
END;
GO

CREATE TRIGGER dbo.trg_Subjects_UpdatedAt
ON dbo.Subjects
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE s
       SET UpdatedAt = SYSUTCDATETIME()
    FROM dbo.Subjects s
    INNER JOIN inserted i
        ON i.Id = s.Id;
END;
GO

/////////

INSERT INTO dbo.AcademicPrograms
(
    Name,
    Description
)
VALUES
(
    N'Ingeniería de Sistemas',
    N'Programa académico de Ingeniería de Sistemas'
),
(
    N'Ingeniería Industrial',
    N'Programa académico de Ingeniería Industrial'
);
GO


/////////////

INSERT INTO dbo.Professors
(
    IdentificationNumber,
    FirstName,
    LastName,
    Email
)
VALUES
(N'P1001', N'Carlos', N'Rodríguez', N'carlos.rodriguez@universidad.edu'),
(N'P1002', N'María', N'Gómez',      N'maria.gomez@universidad.edu'),
(N'P1003', N'Juan',  N'Martínez',  N'juan.martinez@universidad.edu'),
(N'P1004', N'Ana',   N'López',     N'ana.lopez@universidad.edu'),
(N'P1005', N'Pedro', N'Torres',    N'pedro.torres@universidad.edu');
GO

////////////////

INSERT INTO dbo.Subjects
(
    Code,
    Name,
    Credits,
    ProfessorId
)
VALUES
(N'SUB001', N'Programación I',             3, 1),
(N'SUB002', N'Bases de Datos',              3, 1),

(N'SUB003', N'Programación II',             3, 2),
(N'SUB004', N'Ingeniería de Software',      3, 2),

(N'SUB005', N'Sistemas Operativos',         3, 3),
(N'SUB006', N'Redes de Computadores',       3, 3),

(N'SUB007', N'Arquitectura de Software',    3, 4),
(N'SUB008', N'Seguridad Informática',       3, 4),

(N'SUB009', N'Inteligencia Artificial',     3, 5),
(N'SUB010', N'Computación en la Nube',      3, 5);
GO


////////////

INSERT INTO dbo.Students
(
    IdentificationNumber,
    FirstName,
    LastName,
    Email,
    Phone,
    AcademicProgramId
)
VALUES
(
    N'S1001',
    N'Laura',
    N'García',
    N'laura.garcia@email.com',
    N'3001000001',
    1
),
(
    N'S1002',
    N'Andrés',
    N'Martínez',
    N'andres.martinez@email.com',
    N'3001000002',
    1
),
(
    N'S1003',
    N'Camila',
    N'Rodríguez',
    N'camila.rodriguez@email.com',
    N'3001000003',
    1
),
(
    N'S1004',
    N'Felipe',
    N'Gómez',
    N'felipe.gomez@email.com',
    N'3001000004',
    2
),
(
    N'S1005',
    N'Sofía',
    N'Pérez',
    N'sofia.perez@email.com',
    N'3001000005',
    2
);
GO


////////////////

INSERT INTO dbo.StudentSubjects
(
    StudentId,
    SubjectId
)
VALUES
-- Laura
(1, 1),
(1, 3),
(1, 5),

-- Andrés
(2, 1),
(2, 4),
(2, 7),

-- Camila
(3, 2),
(3, 3),
(3, 8),

-- Felipe
(4, 5),
(4, 7),
(4, 9),

-- Sofía
(5, 2),
(5, 4),
(5, 10);
GO