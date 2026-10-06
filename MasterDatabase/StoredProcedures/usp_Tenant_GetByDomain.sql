CREATE PROCEDURE [dbo].[usp_Tenant_GetByDomain]
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
