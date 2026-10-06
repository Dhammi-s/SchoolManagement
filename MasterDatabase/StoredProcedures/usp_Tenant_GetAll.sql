CREATE PROCEDURE [dbo].[usp_Tenant_GetAll]
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
