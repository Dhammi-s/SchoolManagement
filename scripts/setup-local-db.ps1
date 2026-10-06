<#
    Local development database bootstrap.
    Creates the master registry DB and one demo school (tenant) DB on a local
    SQL Server, applies the schema + stored procedures + seed data, and registers
    the demo tenant in the master DB.

    Usage:
        pwsh ./scripts/setup-local-db.ps1 -ServerInstance ".\SQLEXPRESS"

    This uses raw sqlcmd against the .sql source files so it works without
    SqlPackage. The CI pipeline (GitHub Actions) uses DACPAC publish instead.
#>
param(
    [string]$ServerInstance = ".\SQLEXPRESS",
    [string]$MasterDbName   = "SchoolManagement_Master",
    [string]$TenantDbName   = "SchoolManagement_Greenwood",
    [string]$TenantDomain   = "greenwood.localhost",
    [string]$TenantSchoolName = "Greenwood High School"
)

$ErrorActionPreference = "Stop"
$repoRoot   = Split-Path -Parent $PSScriptRoot
$masterDir  = Join-Path $repoRoot "MasterDatabase"
$tenantDir  = "G:\SchoolManagementDataBase\SchoolManagement-DB"

function Invoke-Sql([string]$Database, [string]$Query) {
    sqlcmd -S $ServerInstance -d $Database -b -Q $Query
    if ($LASTEXITCODE -ne 0) { throw "sqlcmd query failed on $Database" }
}
function Invoke-SqlFile([string]$Database, [string]$Path) {
    Write-Host ("  - {0}" -f (Split-Path $Path -Leaf))
    sqlcmd -S $ServerInstance -d $Database -b -i $Path
    if ($LASTEXITCODE -ne 0) { throw "sqlcmd file failed: $Path" }
}

Write-Host "==> Creating databases on $ServerInstance" -ForegroundColor Cyan
Invoke-Sql "master" "IF DB_ID('$MasterDbName') IS NULL CREATE DATABASE [$MasterDbName];"
Invoke-Sql "master" "IF DB_ID('$TenantDbName') IS NULL CREATE DATABASE [$TenantDbName];"

# ---------------------------------------------------------------------------
# Master DB
# ---------------------------------------------------------------------------
Write-Host "==> Applying MASTER schema" -ForegroundColor Cyan
Invoke-SqlFile $MasterDbName (Join-Path $masterDir "Tables\Tenants.sql")
Get-ChildItem (Join-Path $masterDir "StoredProcedures") -Filter *.sql |
    ForEach-Object { Invoke-SqlFile $MasterDbName $_.FullName }

# ---------------------------------------------------------------------------
# Tenant (school) DB — tables in dependency order, then functions, SPs, seed
# ---------------------------------------------------------------------------
Write-Host "==> Applying TENANT schema ($TenantDbName)" -ForegroundColor Cyan
$tableOrder = @(
    "Roles","Employees","Subjects","BusRoutes","Classes","Sections","Students",
    "Users","ClassSubjects","TimetablePeriods","StudentDocuments","StudentInterests",
    "FeeStructures","StudentFees","PerformanceTests","TestResults","SchoolSettings"
)
foreach ($t in $tableOrder) {
    Invoke-SqlFile $TenantDbName (Join-Path $tenantDir "Tables\$t.sql")
}
Get-ChildItem (Join-Path $tenantDir "Functions") -Filter *.sql |
    ForEach-Object { Invoke-SqlFile $TenantDbName $_.FullName }
Get-ChildItem (Join-Path $tenantDir "StoredProcedures") -Filter *.sql |
    ForEach-Object { Invoke-SqlFile $TenantDbName $_.FullName }
Invoke-SqlFile $TenantDbName (Join-Path $tenantDir "Scripts\PostDeployment.sql")

# ---------------------------------------------------------------------------
# Register the demo tenant in the master registry
# ---------------------------------------------------------------------------
Write-Host "==> Registering demo tenant '$TenantDomain'" -ForegroundColor Cyan
$tenantConn = "Server=$ServerInstance;Database=$TenantDbName;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False"
$tenantConnEscaped = $tenantConn.Replace("'", "''")
$register = @"
IF NOT EXISTS (SELECT 1 FROM dbo.Tenants WHERE Domain = N'$TenantDomain')
    INSERT INTO dbo.Tenants (SchoolName, Domain, DatabaseName, ConnectionString)
    VALUES (N'$TenantSchoolName', N'$TenantDomain', N'$TenantDbName', N'$tenantConnEscaped');
ELSE
    UPDATE dbo.Tenants SET ConnectionString = N'$tenantConnEscaped', DatabaseName = N'$TenantDbName'
    WHERE Domain = N'$TenantDomain';
"@
Invoke-Sql $MasterDbName $register

Write-Host ""
Write-Host "Done." -ForegroundColor Green
Write-Host "  Master DB : $MasterDbName"
Write-Host "  Tenant DB : $TenantDbName  (domain: $TenantDomain)"
Write-Host "  Login     : principal / Principal@123"
