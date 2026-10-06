/* One-shot HOSTED deploy (master+tenant in one DB). Idempotent. Edit @Domain/@ConnStr at bottom. Run in SSMS/Azure Data Studio. */
GO
/* ---- TABLES ---- */
IF OBJECT_ID(N'dbo.Tenants', N'U') IS NULL
BEGIN
-- Registry of all schools (tenants). One row per deployed school site.
-- The API resolves the current tenant from the request domain, then uses
-- [ConnectionString] to open that school's own database.
CREATE TABLE [dbo].[Tenants]
(
    [Id]               INT             IDENTITY (1, 1) NOT NULL,
    [SchoolName]       NVARCHAR (200)  NOT NULL,
    [Domain]           NVARCHAR (256)  NOT NULL, -- e.g. greenwood.myschools.com
    [DatabaseName]     NVARCHAR (128)  NULL,
    [ConnectionString] NVARCHAR (1000) NOT NULL, -- connection string to the school DB
    [IsActive]         BIT             NOT NULL CONSTRAINT [DF_Tenants_IsActive] DEFAULT (1),
    [CreatedAt]        DATETIME2 (0)   NOT NULL CONSTRAINT [DF_Tenants_CreatedAt] DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt]        DATETIME2 (0)   NULL,
    CONSTRAINT [PK_Tenants] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Tenants_Domain] UNIQUE NONCLUSTERED ([Domain] ASC)
);

END
GO
IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Roles]
(
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (200) NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Roles_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

END
GO
IF OBJECT_ID(N'dbo.Employees', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Employees]
(
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeCode]    NVARCHAR (30)  NOT NULL,
    [FirstName]       NVARCHAR (100) NOT NULL,
    [LastName]        NVARCHAR (100) NULL,
    [RoleId]          INT            NOT NULL,
    [Designation]     NVARCHAR (100) NULL,
    [Email]           NVARCHAR (256) NULL,
    [Phone]           NVARCHAR (20)  NULL,
    [Gender]          NVARCHAR (10)  NULL,
    [DateOfBirth]     DATE           NULL,
    [Qualification]   NVARCHAR (200) NULL,
    [Address]         NVARCHAR (500) NULL,
    [DateOfJoining]   DATE           NULL,
    [PhotoUrl]        NVARCHAR (500) NULL,
    [IsActive]        BIT            NOT NULL CONSTRAINT [DF_Employees_IsActive] DEFAULT (1),
    [CreatedAt]       DATETIME2 (0)  NOT NULL CONSTRAINT [DF_Employees_CreatedAt] DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Employees_Code] UNIQUE NONCLUSTERED ([EmployeeCode] ASC),
    CONSTRAINT [FK_Employees_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id])
);

END
GO
IF OBJECT_ID(N'dbo.Subjects', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Subjects]
(
    [Id]   INT           IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (100) NOT NULL,
    [Code] NVARCHAR (20)  NULL,
    CONSTRAINT [PK_Subjects] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Subjects_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

END
GO
IF OBJECT_ID(N'dbo.BusRoutes', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[BusRoutes]
(
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [RouteName]     NVARCHAR (100) NOT NULL,
    [VehicleNumber] NVARCHAR (30)  NULL,
    [DriverName]    NVARCHAR (100) NULL,
    [DriverPhone]   NVARCHAR (20)  NULL,
    [Fee]           DECIMAL (10, 2) NOT NULL CONSTRAINT [DF_BusRoutes_Fee] DEFAULT (0),
    [IsActive]      BIT            NOT NULL CONSTRAINT [DF_BusRoutes_IsActive] DEFAULT (1),
    CONSTRAINT [PK_BusRoutes] PRIMARY KEY CLUSTERED ([Id] ASC)
);

END
GO
IF OBJECT_ID(N'dbo.Classes', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Classes]
(
    [Id]                      INT           IDENTITY (1, 1) NOT NULL,
    [Name]                    NVARCHAR (50) NOT NULL,
    [AcademicYear]            NVARCHAR (12) NOT NULL,
    [ClassInchargeEmployeeId] INT           NULL,
    [CreatedAt]               DATETIME2 (0) NOT NULL CONSTRAINT [DF_Classes_CreatedAt] DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_Classes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Classes_Name_Year] UNIQUE NONCLUSTERED ([Name] ASC, [AcademicYear] ASC),
    CONSTRAINT [FK_Classes_Incharge] FOREIGN KEY ([ClassInchargeEmployeeId]) REFERENCES [dbo].[Employees] ([Id])
);

END
GO
IF OBJECT_ID(N'dbo.Sections', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Sections]
(
    [Id]      INT           IDENTITY (1, 1) NOT NULL,
    [ClassId] INT           NOT NULL,
    [Name]    NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_Sections] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Sections_Class_Name] UNIQUE NONCLUSTERED ([ClassId] ASC, [Name] ASC),
    CONSTRAINT [FK_Sections_Classes] FOREIGN KEY ([ClassId]) REFERENCES [dbo].[Classes] ([Id])
);

END
GO
IF OBJECT_ID(N'dbo.Students', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Students]
(
    [Id]                    INT            IDENTITY (1, 1) NOT NULL,
    [AdmissionNumber]       NVARCHAR (30)  NOT NULL,
    [FirstName]             NVARCHAR (100) NOT NULL,
    [LastName]              NVARCHAR (100) NULL,
    [Gender]                NVARCHAR (10)  NULL,
    [DateOfBirth]           DATE           NULL,
    [ClassId]               INT            NULL,
    [SectionId]             INT            NULL,
    [RollNumber]            NVARCHAR (20)  NULL,
    [PhotoUrl]              NVARCHAR (500) NULL,
    [Address]               NVARCHAR (500) NULL,
    [Email]                 NVARCHAR (256) NULL,
    [GuardianName]          NVARCHAR (150) NULL,
    [GuardianPhone]         NVARCHAR (20)  NULL,
    [PreviousSchoolName]    NVARCHAR (200) NULL,
    [PreviousSchoolDetails] NVARCHAR (MAX) NULL,
    [UsesBusService]        BIT            NOT NULL CONSTRAINT [DF_Students_UsesBus] DEFAULT (0),
    [BusRouteId]            INT            NULL,
    [AdmissionDate]         DATE           NULL,
    [IsActive]              BIT            NOT NULL CONSTRAINT [DF_Students_IsActive] DEFAULT (1),
    [CreatedAt]             DATETIME2 (0)  NOT NULL CONSTRAINT [DF_Students_CreatedAt] DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_Students] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Students_Admission] UNIQUE NONCLUSTERED ([AdmissionNumber] ASC),
    CONSTRAINT [FK_Students_Classes] FOREIGN KEY ([ClassId]) REFERENCES [dbo].[Classes] ([Id]),
    CONSTRAINT [FK_Students_Sections] FOREIGN KEY ([SectionId]) REFERENCES [dbo].[Sections] ([Id]),
    CONSTRAINT [FK_Students_BusRoutes] FOREIGN KEY ([BusRouteId]) REFERENCES [dbo].[BusRoutes] ([Id])
);

END
GO
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Users]
(
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Username]     NVARCHAR (100) NOT NULL,
    [PasswordHash] NVARCHAR (300) NOT NULL,
    [RoleId]       INT            NOT NULL,
    [EmployeeId]   INT            NULL,
    [StudentId]    INT            NULL,
    [IsActive]     BIT            NOT NULL CONSTRAINT [DF_Users_IsActive] DEFAULT (1),
    [LastLoginAt]  DATETIME2 (0)  NULL,
    [CreatedAt]    DATETIME2 (0)  NOT NULL CONSTRAINT [DF_Users_CreatedAt] DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Users_Username] UNIQUE NONCLUSTERED ([Username] ASC),
    CONSTRAINT [FK_Users_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]),
    CONSTRAINT [FK_Users_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]),
    CONSTRAINT [FK_Users_Students] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Students] ([Id])
);

END
GO
IF OBJECT_ID(N'dbo.ClassSubjects', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[ClassSubjects]
(
    [Id]               INT IDENTITY (1, 1) NOT NULL,
    [ClassId]          INT NOT NULL,
    [SubjectId]        INT NOT NULL,
    [TeacherEmployeeId] INT NULL,
    CONSTRAINT [PK_ClassSubjects] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_ClassSubjects] UNIQUE NONCLUSTERED ([ClassId] ASC, [SubjectId] ASC),
    CONSTRAINT [FK_ClassSubjects_Classes] FOREIGN KEY ([ClassId]) REFERENCES [dbo].[Classes] ([Id]),
    CONSTRAINT [FK_ClassSubjects_Subjects] FOREIGN KEY ([SubjectId]) REFERENCES [dbo].[Subjects] ([Id]),
    CONSTRAINT [FK_ClassSubjects_Teacher] FOREIGN KEY ([TeacherEmployeeId]) REFERENCES [dbo].[Employees] ([Id])
);

END
GO
IF OBJECT_ID(N'dbo.TimetablePeriods', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[TimetablePeriods]
(
    [Id]                INT          IDENTITY (1, 1) NOT NULL,
    [ClassId]           INT          NOT NULL,
    [SectionId]         INT          NULL,
    [SubjectId]         INT          NOT NULL,
    [TeacherEmployeeId] INT          NOT NULL,
    [DayOfWeek]         TINYINT      NOT NULL, -- 1=Mon ... 7=Sun
    [PeriodNumber]      TINYINT      NOT NULL,
    [StartTime]         TIME (0)     NOT NULL,
    [EndTime]           TIME (0)     NOT NULL,
    CONSTRAINT [PK_TimetablePeriods] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Timetable_Classes] FOREIGN KEY ([ClassId]) REFERENCES [dbo].[Classes] ([Id]),
    CONSTRAINT [FK_Timetable_Sections] FOREIGN KEY ([SectionId]) REFERENCES [dbo].[Sections] ([Id]),
    CONSTRAINT [FK_Timetable_Subjects] FOREIGN KEY ([SubjectId]) REFERENCES [dbo].[Subjects] ([Id]),
    CONSTRAINT [FK_Timetable_Teacher] FOREIGN KEY ([TeacherEmployeeId]) REFERENCES [dbo].[Employees] ([Id])
);

END
GO
IF OBJECT_ID(N'dbo.StudentDocuments', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[StudentDocuments]
(
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [StudentId]    INT            NOT NULL,
    [DocumentType] NVARCHAR (100) NOT NULL, -- e.g. BirthCertificate, PreviousSchoolTC, Photo
    [FileName]     NVARCHAR (300) NULL,
    [FileUrl]      NVARCHAR (500) NOT NULL,  -- Cloudinary secure url
    [PublicId]     NVARCHAR (300) NULL,      -- Cloudinary public id
    [UploadedAt]   DATETIME2 (0)  NOT NULL CONSTRAINT [DF_StudentDocuments_UploadedAt] DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_StudentDocuments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_StudentDocuments_Students] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Students] ([Id])
);

END
GO
IF OBJECT_ID(N'dbo.StudentInterests', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[StudentInterests]
(
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [StudentId]    INT            NOT NULL,
    [InterestType] NVARCHAR (50)  NOT NULL, -- Game, Music, Art, etc.
    [InterestName] NVARCHAR (100) NOT NULL, -- Football, Guitar, etc.
    CONSTRAINT [PK_StudentInterests] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_StudentInterests_Students] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Students] ([Id])
);

END
GO
IF OBJECT_ID(N'dbo.FeeStructures', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[FeeStructures]
(
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [ClassId]      INT             NULL,
    [AcademicYear] NVARCHAR (12)   NOT NULL,
    [Title]        NVARCHAR (150)  NOT NULL, -- Tuition, Admission, Transport, etc.
    [Amount]       DECIMAL (10, 2) NOT NULL,
    [DueDate]      DATE            NULL,
    [IsActive]     BIT             NOT NULL CONSTRAINT [DF_FeeStructures_IsActive] DEFAULT (1),
    CONSTRAINT [PK_FeeStructures] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_FeeStructures_Classes] FOREIGN KEY ([ClassId]) REFERENCES [dbo].[Classes] ([Id])
);

END
GO
IF OBJECT_ID(N'dbo.StudentFees', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[StudentFees]
(
    [Id]             INT             IDENTITY (1, 1) NOT NULL,
    [StudentId]      INT             NOT NULL,
    [FeeStructureId] INT             NOT NULL,
    [AmountDue]      DECIMAL (10, 2) NOT NULL,
    [AmountPaid]     DECIMAL (10, 2) NOT NULL CONSTRAINT [DF_StudentFees_Paid] DEFAULT (0),
    [Status]         NVARCHAR (20)   NOT NULL CONSTRAINT [DF_StudentFees_Status] DEFAULT (N'Pending'), -- Pending, Partial, Paid
    [DueDate]        DATE            NULL,
    [PaidDate]       DATE            NULL,
    CONSTRAINT [PK_StudentFees] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_StudentFees_Students] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Students] ([Id]),
    CONSTRAINT [FK_StudentFees_Structure] FOREIGN KEY ([FeeStructureId]) REFERENCES [dbo].[FeeStructures] ([Id])
);

END
GO
IF OBJECT_ID(N'dbo.PerformanceTests', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[PerformanceTests]
(
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [ClassId]           INT            NOT NULL,
    [SectionId]         INT            NULL,
    [SubjectId]         INT            NOT NULL,
    [TeacherEmployeeId] INT            NOT NULL,
    [Title]             NVARCHAR (200) NOT NULL,
    [TestDate]          DATE           NOT NULL,
    [MaxMarks]          DECIMAL (6, 2) NOT NULL,
    [CreatedAt]         DATETIME2 (0)  NOT NULL CONSTRAINT [DF_PerformanceTests_CreatedAt] DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_PerformanceTests] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PerfTests_Classes] FOREIGN KEY ([ClassId]) REFERENCES [dbo].[Classes] ([Id]),
    CONSTRAINT [FK_PerfTests_Sections] FOREIGN KEY ([SectionId]) REFERENCES [dbo].[Sections] ([Id]),
    CONSTRAINT [FK_PerfTests_Subjects] FOREIGN KEY ([SubjectId]) REFERENCES [dbo].[Subjects] ([Id]),
    CONSTRAINT [FK_PerfTests_Teacher] FOREIGN KEY ([TeacherEmployeeId]) REFERENCES [dbo].[Employees] ([Id])
);

END
GO
IF OBJECT_ID(N'dbo.TestResults', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[TestResults]
(
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [PerformanceTestId] INT            NOT NULL,
    [StudentId]         INT            NOT NULL,
    [MarksObtained]     DECIMAL (6, 2) NULL,
    [Grade]             NVARCHAR (5)   NULL,
    [Remarks]           NVARCHAR (300) NULL,
    CONSTRAINT [PK_TestResults] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_TestResults] UNIQUE NONCLUSTERED ([PerformanceTestId] ASC, [StudentId] ASC),
    CONSTRAINT [FK_TestResults_Test] FOREIGN KEY ([PerformanceTestId]) REFERENCES [dbo].[PerformanceTests] ([Id]),
    CONSTRAINT [FK_TestResults_Students] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Students] ([Id])
);

END
GO
IF OBJECT_ID(N'dbo.SchoolSettings', N'U') IS NULL
BEGIN
-- Per-school branding / appearance. Editable by the Principal.
-- Single-row table (enforced by a check constraint on a fixed Id).
CREATE TABLE [dbo].[SchoolSettings]
(
    [Id]                  INT            NOT NULL CONSTRAINT [DF_SchoolSettings_Id] DEFAULT (1),
    [SchoolName]          NVARCHAR (200) NULL,
    [LogoUrl]             NVARCHAR (500) NULL,
    [PrimaryColor]        NVARCHAR (20)  NULL CONSTRAINT [DF_SchoolSettings_Primary] DEFAULT (N'#2563eb'),
    [SecondaryColor]      NVARCHAR (20)  NULL CONSTRAINT [DF_SchoolSettings_Secondary] DEFAULT (N'#1e293b'),
    [LoginBackgroundUrl]  NVARCHAR (500) NULL,
    [LoginTitle]          NVARCHAR (200) NULL,
    [LoginSubtitle]       NVARCHAR (300) NULL,
    [UpdatedAt]           DATETIME2 (0)  NOT NULL CONSTRAINT [DF_SchoolSettings_UpdatedAt] DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_SchoolSettings] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CK_SchoolSettings_SingleRow] CHECK ([Id] = 1)
);

END
GO
IF COL_LENGTH('dbo.Students','Email') IS NULL ALTER TABLE dbo.Students ADD [Email] NVARCHAR(256) NULL;
GO
/* ---- FUNCTIONS & PROCEDURES ---- */
-- Returns the total outstanding (unpaid) fee amount for a given student.
CREATE OR ALTER FUNCTION [dbo].[fn_Student_PendingFeeTotal]
(
    @StudentId INT
)
RETURNS DECIMAL (12, 2)
AS
BEGIN
    DECLARE @Pending DECIMAL (12, 2);

    SELECT @Pending = ISNULL(SUM(sf.[AmountDue] - sf.[AmountPaid]), 0)
    FROM   [dbo].[StudentFees] sf
    WHERE  sf.[StudentId] = @StudentId
      AND  sf.[Status] <> N'Paid';

    RETURN ISNULL(@Pending, 0);
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_BusRoute_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [RouteName], [VehicleNumber], [DriverName], [DriverPhone], [Fee], [IsActive]
    FROM   [dbo].[BusRoutes]
    WHERE  [IsActive] = 1
    ORDER BY [RouteName];
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_BusRoute_Insert]
    @RouteName     NVARCHAR (100),
    @VehicleNumber NVARCHAR (30)  = NULL,
    @DriverName    NVARCHAR (100) = NULL,
    @DriverPhone   NVARCHAR (20)  = NULL,
    @Fee           DECIMAL (10, 2) = 0,
    @NewId         INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[BusRoutes] ([RouteName], [VehicleNumber], [DriverName], [DriverPhone], [Fee])
    VALUES (@RouteName, @VehicleNumber, @DriverName, @DriverPhone, @Fee);

    SET @NewId = CAST(SCOPE_IDENTITY() AS INT);
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_ClassSubject_Assign]
    @ClassId           INT,
    @SubjectId         INT,
    @TeacherEmployeeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [dbo].[ClassSubjects] WHERE [ClassId] = @ClassId AND [SubjectId] = @SubjectId)
        UPDATE [dbo].[ClassSubjects]
        SET    [TeacherEmployeeId] = @TeacherEmployeeId
        WHERE  [ClassId] = @ClassId AND [SubjectId] = @SubjectId;
    ELSE
        INSERT INTO [dbo].[ClassSubjects] ([ClassId], [SubjectId], [TeacherEmployeeId])
        VALUES (@ClassId, @SubjectId, @TeacherEmployeeId);
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Class_AssignIncharge]
    @ClassId    INT,
    @EmployeeId INT = NULL  -- NULL clears the incharge
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Classes]
    SET    [ClassInchargeEmployeeId] = @EmployeeId
    WHERE  [Id] = @ClassId;
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Class_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  c.[Id],
            c.[Name],
            c.[AcademicYear],
            c.[ClassInchargeEmployeeId],
            CASE WHEN e.[Id] IS NULL THEN NULL
                 ELSE LTRIM(RTRIM(e.[FirstName] + N' ' + ISNULL(e.[LastName], N''))) END AS ClassInchargeName,
            (SELECT COUNT(1) FROM [dbo].[Students] s
             WHERE s.[ClassId] = c.[Id] AND s.[IsActive] = 1)  AS StudentCount,
            c.[CreatedAt]
    FROM    [dbo].[Classes] c
    LEFT JOIN [dbo].[Employees] e ON e.[Id] = c.[ClassInchargeEmployeeId]
    ORDER BY c.[Name];
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Class_Insert]
    @Name                    NVARCHAR (50),
    @AcademicYear            NVARCHAR (12),
    @ClassInchargeEmployeeId INT = NULL,
    @NewId                   INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[Classes] ([Name], [AcademicYear], [ClassInchargeEmployeeId])
    VALUES (@Name, @AcademicYear, @ClassInchargeEmployeeId);

    SET @NewId = CAST(SCOPE_IDENTITY() AS INT);
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Employee_GetByRole]
    @RoleName NVARCHAR (50) = NULL  -- NULL = all employees
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  e.[Id],
            e.[EmployeeCode],
            e.[FirstName],
            e.[LastName],
            e.[RoleId],
            r.[Name] AS RoleName,
            e.[Designation],
            e.[Email],
            e.[Phone],
            e.[Gender],
            e.[DateOfBirth],
            e.[Qualification],
            e.[Address],
            e.[DateOfJoining],
            e.[PhotoUrl],
            e.[IsActive],
            -- Classes this employee is incharge of (comma-separated), if any
            (SELECT STRING_AGG(c.[Name], N', ')
             FROM [dbo].[Classes] c
             WHERE c.[ClassInchargeEmployeeId] = e.[Id]) AS InchargeClasses
    FROM    [dbo].[Employees] e
    INNER JOIN [dbo].[Roles] r ON r.[Id] = e.[RoleId]
    WHERE   (@RoleName IS NULL OR r.[Name] = @RoleName)
      AND   e.[IsActive] = 1
    ORDER BY e.[FirstName], e.[LastName];
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Employee_Insert]
    @EmployeeCode   NVARCHAR (30),
    @FirstName      NVARCHAR (100),
    @LastName       NVARCHAR (100) = NULL,
    @RoleName       NVARCHAR (50),
    @Designation    NVARCHAR (100) = NULL,
    @Email          NVARCHAR (256) = NULL,
    @Phone          NVARCHAR (20)  = NULL,
    @Gender         NVARCHAR (10)  = NULL,
    @DateOfBirth    DATE           = NULL,
    @Qualification  NVARCHAR (200) = NULL,
    @Address        NVARCHAR (500) = NULL,
    @DateOfJoining  DATE           = NULL,
    @PhotoUrl       NVARCHAR (500) = NULL,
    @NewId          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RoleId INT = (SELECT [Id] FROM [dbo].[Roles] WHERE [Name] = @RoleName);
    IF @RoleId IS NULL
        THROW 50001, 'Unknown role name.', 1;

    INSERT INTO [dbo].[Employees]
        ([EmployeeCode], [FirstName], [LastName], [RoleId], [Designation], [Email],
         [Phone], [Gender], [DateOfBirth], [Qualification], [Address], [DateOfJoining], [PhotoUrl])
    VALUES
        (@EmployeeCode, @FirstName, @LastName, @RoleId, @Designation, @Email,
         @Phone, @Gender, @DateOfBirth, @Qualification, @Address, @DateOfJoining, @PhotoUrl);

    SET @NewId = CAST(SCOPE_IDENTITY() AS INT);
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Employee_SetPhoto]
    @EmployeeId INT,
    @PhotoUrl   NVARCHAR (500)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Employees]
    SET    [PhotoUrl] = @PhotoUrl
    WHERE  [Id] = @EmployeeId;
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_FeeStructure_GetAll]
    @ClassId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  fs.[Id],
            fs.[ClassId],
            c.[Name] AS ClassName,
            fs.[AcademicYear],
            fs.[Title],
            fs.[Amount],
            fs.[DueDate],
            fs.[IsActive]
    FROM    [dbo].[FeeStructures] fs
    LEFT JOIN [dbo].[Classes] c ON c.[Id] = fs.[ClassId]
    WHERE   fs.[IsActive] = 1
      AND   (@ClassId IS NULL OR fs.[ClassId] = @ClassId OR fs.[ClassId] IS NULL)
    ORDER BY fs.[AcademicYear] DESC, c.[Name], fs.[Title];
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_FeeStructure_Insert]
    @ClassId      INT = NULL,
    @AcademicYear NVARCHAR (12),
    @Title        NVARCHAR (150),
    @Amount       DECIMAL (10, 2),
    @DueDate      DATE = NULL,
    @NewId        INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[FeeStructures] ([ClassId], [AcademicYear], [Title], [Amount], [DueDate])
    VALUES (@ClassId, @AcademicYear, @Title, @Amount, @DueDate);

    SET @NewId = CAST(SCOPE_IDENTITY() AS INT);
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_PerformanceTest_GetByClass]
    @ClassId           INT = NULL,
    @TeacherEmployeeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  t.[Id],
            t.[ClassId],
            c.[Name]   AS ClassName,
            t.[SectionId],
            sec.[Name] AS SectionName,
            t.[SubjectId],
            sub.[Name] AS SubjectName,
            t.[TeacherEmployeeId],
            LTRIM(RTRIM(e.[FirstName] + N' ' + ISNULL(e.[LastName], N''))) AS TeacherName,
            t.[Title],
            t.[TestDate],
            t.[MaxMarks]
    FROM    [dbo].[PerformanceTests] t
    INNER JOIN [dbo].[Classes]   c   ON c.[Id]   = t.[ClassId]
    INNER JOIN [dbo].[Subjects]  sub ON sub.[Id] = t.[SubjectId]
    INNER JOIN [dbo].[Employees] e   ON e.[Id]   = t.[TeacherEmployeeId]
    LEFT  JOIN [dbo].[Sections]  sec ON sec.[Id] = t.[SectionId]
    WHERE   (@ClassId IS NULL OR t.[ClassId] = @ClassId)
      AND   (@TeacherEmployeeId IS NULL OR t.[TeacherEmployeeId] = @TeacherEmployeeId)
    ORDER BY t.[TestDate] DESC, t.[Title];
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_PerformanceTest_Insert]
    @ClassId           INT,
    @SectionId         INT = NULL,
    @SubjectId         INT,
    @TeacherEmployeeId INT,
    @Title             NVARCHAR (200),
    @TestDate          DATE,
    @MaxMarks          DECIMAL (6, 2),
    @NewId             INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[PerformanceTests]
        ([ClassId], [SectionId], [SubjectId], [TeacherEmployeeId], [Title], [TestDate], [MaxMarks])
    VALUES
        (@ClassId, @SectionId, @SubjectId, @TeacherEmployeeId, @Title, @TestDate, @MaxMarks);

    SET @NewId = CAST(SCOPE_IDENTITY() AS INT);
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_SchoolSettings_Get]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
            [Id],
            [SchoolName],
            [LogoUrl],
            [PrimaryColor],
            [SecondaryColor],
            [LoginBackgroundUrl],
            [LoginTitle],
            [LoginSubtitle],
            [UpdatedAt]
    FROM    [dbo].[SchoolSettings]
    WHERE   [Id] = 1;
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_SchoolSettings_Update]
    @SchoolName         NVARCHAR (200) = NULL,
    @LogoUrl            NVARCHAR (500) = NULL,
    @PrimaryColor       NVARCHAR (20)  = NULL,
    @SecondaryColor     NVARCHAR (20)  = NULL,
    @LoginBackgroundUrl NVARCHAR (500) = NULL,
    @LoginTitle         NVARCHAR (200) = NULL,
    @LoginSubtitle      NVARCHAR (300) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[SchoolSettings] WHERE [Id] = 1)
        INSERT INTO [dbo].[SchoolSettings] ([Id]) VALUES (1);

    UPDATE [dbo].[SchoolSettings]
    SET    [SchoolName]         = @SchoolName,
           [LogoUrl]            = @LogoUrl,
           [PrimaryColor]       = @PrimaryColor,
           [SecondaryColor]     = @SecondaryColor,
           [LoginBackgroundUrl] = @LoginBackgroundUrl,
           [LoginTitle]         = @LoginTitle,
           [LoginSubtitle]      = @LoginSubtitle,
           [UpdatedAt]          = SYSUTCDATETIME()
    WHERE  [Id] = 1;
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Section_GetByClass]
    @ClassId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [ClassId], [Name]
    FROM   [dbo].[Sections]
    WHERE  [ClassId] = @ClassId
    ORDER BY [Name];
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Section_Insert]
    @ClassId INT,
    @Name    NVARCHAR (20),
    @NewId   INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[Sections] ([ClassId], [Name])
    VALUES (@ClassId, @Name);

    SET @NewId = CAST(SCOPE_IDENTITY() AS INT);
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_StudentDocument_Add]
    @StudentId    INT,
    @DocumentType NVARCHAR (100),
    @FileName     NVARCHAR (300) = NULL,
    @FileUrl      NVARCHAR (500),
    @PublicId     NVARCHAR (300) = NULL,
    @NewId        INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[StudentDocuments] ([StudentId], [DocumentType], [FileName], [FileUrl], [PublicId])
    VALUES (@StudentId, @DocumentType, @FileName, @FileUrl, @PublicId);

    SET @NewId = CAST(SCOPE_IDENTITY() AS INT);
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_StudentDocument_GetByStudent]
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [StudentId], [DocumentType], [FileName], [FileUrl], [PublicId], [UploadedAt]
    FROM   [dbo].[StudentDocuments]
    WHERE  [StudentId] = @StudentId
    ORDER BY [UploadedAt] DESC;
END

GO
-- Assigns a fee structure to a student, creating a StudentFee line (idempotent).
CREATE OR ALTER PROCEDURE [dbo].[usp_StudentFee_Assign]
    @StudentId      INT,
    @FeeStructureId INT,
    @NewId          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [dbo].[StudentFees] WHERE [StudentId] = @StudentId AND [FeeStructureId] = @FeeStructureId)
    BEGIN
        SET @NewId = (SELECT [Id] FROM [dbo].[StudentFees] WHERE [StudentId] = @StudentId AND [FeeStructureId] = @FeeStructureId);
        RETURN;
    END

    DECLARE @Amount DECIMAL (10, 2), @DueDate DATE;
    SELECT @Amount = [Amount], @DueDate = [DueDate] FROM [dbo].[FeeStructures] WHERE [Id] = @FeeStructureId;

    INSERT INTO [dbo].[StudentFees] ([StudentId], [FeeStructureId], [AmountDue], [AmountPaid], [Status], [DueDate])
    VALUES (@StudentId, @FeeStructureId, ISNULL(@Amount, 0), 0, N'Pending', @DueDate);

    SET @NewId = CAST(SCOPE_IDENTITY() AS INT);
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_StudentFee_GetByStudent]
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  sf.[Id],
            sf.[StudentId],
            sf.[FeeStructureId],
            fs.[Title],
            fs.[AcademicYear],
            sf.[AmountDue],
            sf.[AmountPaid],
            (sf.[AmountDue] - sf.[AmountPaid]) AS Balance,
            sf.[Status],
            sf.[DueDate],
            sf.[PaidDate]
    FROM    [dbo].[StudentFees] sf
    INNER JOIN [dbo].[FeeStructures] fs ON fs.[Id] = sf.[FeeStructureId]
    WHERE   sf.[StudentId] = @StudentId
    ORDER BY sf.[DueDate], fs.[Title];
END

GO
-- Students who still owe fees, with their outstanding total.
CREATE OR ALTER PROCEDURE [dbo].[usp_StudentFee_GetPending]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  s.[Id]              AS StudentId,
            s.[AdmissionNumber],
            LTRIM(RTRIM(s.[FirstName] + N' ' + ISNULL(s.[LastName], N''))) AS StudentName,
            c.[Name]            AS ClassName,
            SUM(sf.[AmountDue] - sf.[AmountPaid]) AS PendingAmount
    FROM    [dbo].[StudentFees] sf
    INNER JOIN [dbo].[Students] s ON s.[Id] = sf.[StudentId]
    LEFT  JOIN [dbo].[Classes]  c ON c.[Id] = s.[ClassId]
    WHERE   sf.[Status] <> N'Paid'
    GROUP BY s.[Id], s.[AdmissionNumber], s.[FirstName], s.[LastName], c.[Name]
    HAVING  SUM(sf.[AmountDue] - sf.[AmountPaid]) > 0
    ORDER BY PendingAmount DESC;
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_StudentFee_RecordPayment]
    @StudentFeeId INT,
    @Amount       DECIMAL (10, 2)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[StudentFees]
    SET    [AmountPaid] = [AmountPaid] + @Amount,
           [PaidDate]   = CAST(SYSUTCDATETIME() AS DATE),
           [Status]     = CASE
                              WHEN ([AmountPaid] + @Amount) >= [AmountDue] THEN N'Paid'
                              WHEN ([AmountPaid] + @Amount) > 0 THEN N'Partial'
                              ELSE N'Pending'
                          END
    WHERE  [Id] = @StudentFeeId;
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_StudentInterest_Add]
    @StudentId    INT,
    @InterestType NVARCHAR (50),
    @InterestName NVARCHAR (100),
    @NewId        INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[StudentInterests] ([StudentId], [InterestType], [InterestName])
    VALUES (@StudentId, @InterestType, @InterestName);

    SET @NewId = CAST(SCOPE_IDENTITY() AS INT);
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_StudentInterest_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[StudentInterests] WHERE [Id] = @Id;
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_StudentInterest_GetByStudent]
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [StudentId], [InterestType], [InterestName]
    FROM   [dbo].[StudentInterests]
    WHERE  [StudentId] = @StudentId
    ORDER BY [InterestType], [InterestName];
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Student_GetAll]
    @ClassId INT = NULL,   -- optional filter
    @Search  NVARCHAR (100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  s.[Id],
            s.[AdmissionNumber],
            s.[FirstName],
            s.[LastName],
            s.[Gender],
            s.[RollNumber],
            s.[ClassId],
            c.[Name]   AS ClassName,
            s.[SectionId],
            sec.[Name] AS SectionName,
            s.[PhotoUrl],
            s.[GuardianName],
            s.[GuardianPhone],
            s.[UsesBusService],
            [dbo].[fn_Student_PendingFeeTotal](s.[Id]) AS PendingFees
    FROM    [dbo].[Students] s
    LEFT JOIN [dbo].[Classes]  c   ON c.[Id]   = s.[ClassId]
    LEFT JOIN [dbo].[Sections] sec ON sec.[Id] = s.[SectionId]
    WHERE   s.[IsActive] = 1
      AND   (@ClassId IS NULL OR s.[ClassId] = @ClassId)
      AND   (@Search IS NULL OR s.[FirstName] LIKE N'%' + @Search + N'%'
                             OR s.[LastName]  LIKE N'%' + @Search + N'%'
                             OR s.[AdmissionNumber] LIKE N'%' + @Search + N'%')
    ORDER BY c.[Name], s.[RollNumber], s.[FirstName];
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Student_GetByClass]
    @ClassId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  s.[Id],
            s.[AdmissionNumber],
            s.[FirstName],
            s.[LastName],
            s.[Gender],
            s.[RollNumber],
            s.[ClassId],
            s.[SectionId],
            sec.[Name] AS SectionName,
            s.[PhotoUrl],
            s.[GuardianName],
            s.[GuardianPhone]
    FROM    [dbo].[Students] s
    LEFT JOIN [dbo].[Sections] sec ON sec.[Id] = s.[SectionId]
    WHERE   s.[ClassId] = @ClassId
      AND   s.[IsActive] = 1
    ORDER BY s.[RollNumber], s.[FirstName];
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Student_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  s.[Id],
            s.[AdmissionNumber],
            s.[FirstName],
            s.[LastName],
            s.[Gender],
            s.[DateOfBirth],
            s.[ClassId],
            c.[Name]   AS ClassName,
            s.[SectionId],
            sec.[Name] AS SectionName,
            s.[RollNumber],
            s.[Address],
            s.[Email],
            s.[GuardianName],
            s.[GuardianPhone],
            s.[PreviousSchoolName],
            s.[PreviousSchoolDetails],
            s.[UsesBusService],
            s.[BusRouteId],
            br.[RouteName] AS BusRouteName,
            s.[AdmissionDate],
            s.[PhotoUrl],
            s.[IsActive],
            [dbo].[fn_Student_PendingFeeTotal](s.[Id]) AS PendingFees
    FROM    [dbo].[Students] s
    LEFT JOIN [dbo].[Classes]   c   ON c.[Id]   = s.[ClassId]
    LEFT JOIN [dbo].[Sections]  sec ON sec.[Id] = s.[SectionId]
    LEFT JOIN [dbo].[BusRoutes] br  ON br.[Id]  = s.[BusRouteId]
    WHERE   s.[Id] = @Id;
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Student_Insert]
    @AdmissionNumber       NVARCHAR (30),
    @FirstName             NVARCHAR (100),
    @LastName              NVARCHAR (100) = NULL,
    @Gender                NVARCHAR (10)  = NULL,
    @DateOfBirth           DATE           = NULL,
    @ClassId               INT            = NULL,
    @SectionId             INT            = NULL,
    @RollNumber            NVARCHAR (20)  = NULL,
    @Address               NVARCHAR (500) = NULL,
    @Email                 NVARCHAR (256) = NULL,
    @GuardianName          NVARCHAR (150) = NULL,
    @GuardianPhone         NVARCHAR (20)  = NULL,
    @PreviousSchoolName    NVARCHAR (200) = NULL,
    @PreviousSchoolDetails NVARCHAR (MAX) = NULL,
    @UsesBusService        BIT            = 0,
    @BusRouteId            INT            = NULL,
    @AdmissionDate         DATE           = NULL,
    @PhotoUrl              NVARCHAR (500) = NULL,
    @NewId                 INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[Students]
        ([AdmissionNumber], [FirstName], [LastName], [Gender], [DateOfBirth], [ClassId], [SectionId],
         [RollNumber], [Address], [Email], [GuardianName], [GuardianPhone], [PreviousSchoolName],
         [PreviousSchoolDetails], [UsesBusService], [BusRouteId], [AdmissionDate], [PhotoUrl])
    VALUES
        (@AdmissionNumber, @FirstName, @LastName, @Gender, @DateOfBirth, @ClassId, @SectionId,
         @RollNumber, @Address, @Email, @GuardianName, @GuardianPhone, @PreviousSchoolName,
         @PreviousSchoolDetails, @UsesBusService, @BusRouteId, @AdmissionDate, @PhotoUrl);

    SET @NewId = CAST(SCOPE_IDENTITY() AS INT);
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Student_SetPhoto]
    @StudentId INT,
    @PhotoUrl  NVARCHAR (500)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Students]
    SET    [PhotoUrl] = @PhotoUrl
    WHERE  [Id] = @StudentId;
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Subject_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [Name], [Code]
    FROM   [dbo].[Subjects]
    ORDER BY [Name];
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Subject_Insert]
    @Name  NVARCHAR (100),
    @Code  NVARCHAR (20) = NULL,
    @NewId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[Subjects] ([Name], [Code])
    VALUES (@Name, @Code);

    SET @NewId = CAST(SCOPE_IDENTITY() AS INT);
END

GO
-- A student's results across all tests (subject-wise) â€” for the student portal.
CREATE OR ALTER PROCEDURE [dbo].[usp_TestResult_GetByStudent]
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  tr.[Id] AS ResultId,
            t.[Id]  AS PerformanceTestId,
            t.[Title],
            t.[TestDate],
            sub.[Name] AS SubjectName,
            t.[MaxMarks],
            tr.[MarksObtained],
            tr.[Grade],
            tr.[Remarks]
    FROM    [dbo].[TestResults] tr
    INNER JOIN [dbo].[PerformanceTests] t   ON t.[Id]   = tr.[PerformanceTestId]
    INNER JOIN [dbo].[Subjects]         sub ON sub.[Id] = t.[SubjectId]
    WHERE   tr.[StudentId] = @StudentId
    ORDER BY t.[TestDate] DESC, sub.[Name];
END

GO
-- All students in the test's class, with their result (if entered yet).
CREATE OR ALTER PROCEDURE [dbo].[usp_TestResult_GetByTest]
    @PerformanceTestId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ClassId INT = (SELECT [ClassId] FROM [dbo].[PerformanceTests] WHERE [Id] = @PerformanceTestId);

    SELECT  s.[Id] AS StudentId,
            LTRIM(RTRIM(s.[FirstName] + N' ' + ISNULL(s.[LastName], N''))) AS StudentName,
            s.[RollNumber],
            tr.[Id] AS ResultId,
            tr.[MarksObtained],
            tr.[Grade],
            tr.[Remarks]
    FROM    [dbo].[Students] s
    LEFT JOIN [dbo].[TestResults] tr
           ON tr.[StudentId] = s.[Id] AND tr.[PerformanceTestId] = @PerformanceTestId
    WHERE   s.[ClassId] = @ClassId
      AND   s.[IsActive] = 1
    ORDER BY s.[RollNumber], s.[FirstName];
END

GO
-- Upserts a student's result for a test.
CREATE OR ALTER PROCEDURE [dbo].[usp_TestResult_Save]
    @PerformanceTestId INT,
    @StudentId         INT,
    @MarksObtained     DECIMAL (6, 2) = NULL,
    @Grade             NVARCHAR (5)   = NULL,
    @Remarks           NVARCHAR (300) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [dbo].[TestResults] WHERE [PerformanceTestId] = @PerformanceTestId AND [StudentId] = @StudentId)
        UPDATE [dbo].[TestResults]
        SET    [MarksObtained] = @MarksObtained,
               [Grade]         = @Grade,
               [Remarks]       = @Remarks
        WHERE  [PerformanceTestId] = @PerformanceTestId AND [StudentId] = @StudentId;
    ELSE
        INSERT INTO [dbo].[TestResults] ([PerformanceTestId], [StudentId], [MarksObtained], [Grade], [Remarks])
        VALUES (@PerformanceTestId, @StudentId, @MarksObtained, @Grade, @Remarks);
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Timetable_GetByClass]
    @ClassId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  t.[Id],
            t.[ClassId],
            c.[Name]     AS ClassName,
            t.[SectionId],
            sec.[Name]   AS SectionName,
            t.[SubjectId],
            s.[Name]     AS SubjectName,
            t.[TeacherEmployeeId],
            LTRIM(RTRIM(e.[FirstName] + N' ' + ISNULL(e.[LastName], N''))) AS TeacherName,
            t.[DayOfWeek],
            t.[PeriodNumber],
            t.[StartTime],
            t.[EndTime]
    FROM    [dbo].[TimetablePeriods] t
    INNER JOIN [dbo].[Classes]   c   ON c.[Id]   = t.[ClassId]
    INNER JOIN [dbo].[Subjects]  s   ON s.[Id]   = t.[SubjectId]
    INNER JOIN [dbo].[Employees] e   ON e.[Id]   = t.[TeacherEmployeeId]
    LEFT  JOIN [dbo].[Sections]  sec ON sec.[Id] = t.[SectionId]
    WHERE   t.[ClassId] = @ClassId
    ORDER BY t.[DayOfWeek], t.[PeriodNumber];
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Timetable_GetByTeacher]
    @TeacherEmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  t.[Id],
            t.[ClassId],
            c.[Name]     AS ClassName,
            t.[SectionId],
            sec.[Name]   AS SectionName,
            t.[SubjectId],
            s.[Name]     AS SubjectName,
            t.[TeacherEmployeeId],
            t.[DayOfWeek],
            t.[PeriodNumber],
            t.[StartTime],
            t.[EndTime]
    FROM    [dbo].[TimetablePeriods] t
    INNER JOIN [dbo].[Classes]  c   ON c.[Id]   = t.[ClassId]
    INNER JOIN [dbo].[Subjects] s   ON s.[Id]   = t.[SubjectId]
    LEFT  JOIN [dbo].[Sections] sec ON sec.[Id] = t.[SectionId]
    WHERE   t.[TeacherEmployeeId] = @TeacherEmployeeId
    ORDER BY t.[DayOfWeek], t.[PeriodNumber];
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Timetable_Insert]
    @ClassId           INT,
    @SectionId         INT = NULL,
    @SubjectId         INT,
    @TeacherEmployeeId INT,
    @DayOfWeek         TINYINT,
    @PeriodNumber      TINYINT,
    @StartTime         TIME (0),
    @EndTime           TIME (0),
    @NewId             INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[TimetablePeriods]
        ([ClassId], [SectionId], [SubjectId], [TeacherEmployeeId], [DayOfWeek], [PeriodNumber], [StartTime], [EndTime])
    VALUES
        (@ClassId, @SectionId, @SubjectId, @TeacherEmployeeId, @DayOfWeek, @PeriodNumber, @StartTime, @EndTime);

    SET @NewId = CAST(SCOPE_IDENTITY() AS INT);
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_User_GetByUsername]
    @Username NVARCHAR (100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  u.[Id],
            u.[Username],
            u.[PasswordHash],
            u.[RoleId],
            r.[Name] AS RoleName,
            u.[EmployeeId],
            u.[StudentId],
            u.[IsActive]
    FROM    [dbo].[Users] u
    INNER JOIN [dbo].[Roles] r ON r.[Id] = u.[RoleId]
    WHERE   u.[Username] = @Username;
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_User_Insert]
    @Username     NVARCHAR (100),
    @PasswordHash NVARCHAR (300),
    @RoleName     NVARCHAR (50),
    @EmployeeId   INT = NULL,
    @StudentId    INT = NULL,
    @NewId        INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RoleId INT = (SELECT [Id] FROM [dbo].[Roles] WHERE [Name] = @RoleName);
    IF @RoleId IS NULL
        THROW 50001, 'Unknown role name.', 1;

    INSERT INTO [dbo].[Users] ([Username], [PasswordHash], [RoleId], [EmployeeId], [StudentId])
    VALUES (@Username, @PasswordHash, @RoleId, @EmployeeId, @StudentId);

    SET @NewId = CAST(SCOPE_IDENTITY() AS INT);
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_User_UpdateLastLogin]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Users]
    SET    [LastLoginAt] = SYSUTCDATETIME()
    WHERE  [Id] = @UserId;
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Tenant_Create]
    @SchoolName       NVARCHAR (200),
    @Domain           NVARCHAR (256),
    @DatabaseName     NVARCHAR (128) = NULL,
    @ConnectionString NVARCHAR (1000),
    @NewId            INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[Tenants] ([SchoolName], [Domain], [DatabaseName], [ConnectionString])
    VALUES (@SchoolName, @Domain, @DatabaseName, @ConnectionString);

    SET @NewId = CAST(SCOPE_IDENTITY() AS INT);
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Tenant_GetAll]
    @ActiveOnly BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  [Id],
            [SchoolName],
            [Domain],
            [DatabaseName],
            [ConnectionString],
            [IsActive],
            [CreatedAt]
    FROM    [dbo].[Tenants]
    WHERE   (@ActiveOnly = 0 OR [IsActive] = 1)
    ORDER BY [SchoolName];
END

GO
CREATE OR ALTER PROCEDURE [dbo].[usp_Tenant_GetByDomain]
    @Domain NVARCHAR (256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  [Id],
            [SchoolName],
            [Domain],
            [DatabaseName],
            [ConnectionString],
            [IsActive]
    FROM    [dbo].[Tenants]
    WHERE   [Domain] = @Domain
      AND   [IsActive] = 1;
END

GO
/* ---- SEED ---- */
/*
 Post-Deployment Script (runs on every tenant-DB publish; must be idempotent)
 -------------------------------------------------------------------------------
 Seeds the baseline reference data every school database needs:
   - Roles
   - A default SchoolSettings row
   - A bootstrap Principal login (username: principal)

 The principal password hash below is a BCrypt hash for the password: Principal@123
 Change the password from the app after first login.
*/

SET NOCOUNT ON;

-------------------------------------------------------------------------------
-- Roles
-------------------------------------------------------------------------------
MERGE INTO [dbo].[Roles] AS target
USING (VALUES
    (N'Principal',  N'School principal / super administrator'),
    (N'HeadMaster', N'Head master'),
    (N'Teacher',    N'Teaching staff'),
    (N'Accountant', N'Accounts / finance staff'),
    (N'Student',    N'Student portal user')
) AS source ([Name], [Description])
    ON target.[Name] = source.[Name]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([Name], [Description]) VALUES (source.[Name], source.[Description]);

-------------------------------------------------------------------------------
-- Common subjects
-------------------------------------------------------------------------------
MERGE INTO [dbo].[Subjects] AS target
USING (VALUES
    (N'English',    N'ENG'),
    (N'Mathematics',N'MATH'),
    (N'Science',    N'SCI'),
    (N'Social Studies', N'SST'),
    (N'Computer Science', N'CS'),
    (N'Physical Education', N'PE')
) AS source ([Name], [Code])
    ON target.[Name] = source.[Name]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([Name], [Code]) VALUES (source.[Name], source.[Code]);

-------------------------------------------------------------------------------
-- Default school settings (single row, Id = 1)
-------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [dbo].[SchoolSettings] WHERE [Id] = 1)
BEGIN
    INSERT INTO [dbo].[SchoolSettings] ([Id], [SchoolName], [LoginTitle], [LoginSubtitle])
    VALUES (1, N'My School', N'Welcome', N'Sign in to continue');
END

-------------------------------------------------------------------------------
-- Bootstrap Principal user (password: Principal@123)
-------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Username] = N'principal')
BEGIN
    DECLARE @PrincipalRoleId INT = (SELECT [Id] FROM [dbo].[Roles] WHERE [Name] = N'Principal');

    INSERT INTO [dbo].[Users] ([Username], [PasswordHash], [RoleId], [IsActive])
    VALUES (N'principal', N'$2a$11$k2uj5Qv./zVn6MaXnjxsvuuVJxriz2n71C2rpz6r4Pf.UZwtzr7NS', @PrincipalRoleId, 1);
END

GO
/* ==== REGISTER TENANT — EDIT THESE ==== */
DECLARE @Domain NVARCHAR(256) = N'localhost';            -- must match frontend tenantDomain
DECLARE @ConnStr NVARCHAR(1000) = N'PUT-YOUR-RUNASP-SQL-CONNECTION-STRING-HERE';
DECLARE @School NVARCHAR(200) = N'My School';
IF NOT EXISTS (SELECT 1 FROM dbo.Tenants WHERE Domain=@Domain)
  INSERT dbo.Tenants (SchoolName,Domain,DatabaseName,ConnectionString) VALUES (@School,@Domain,NULL,@ConnStr);
ELSE UPDATE dbo.Tenants SET ConnectionString=@ConnStr WHERE Domain=@Domain;
GO

