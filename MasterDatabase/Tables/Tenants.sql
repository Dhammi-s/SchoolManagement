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
