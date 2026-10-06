CREATE PROCEDURE [dbo].[usp_Tenant_Create]
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
