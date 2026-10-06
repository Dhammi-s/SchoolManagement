<#
    Deploys the database schema using DACPAC publish (SqlPackage).

      1. Publishes the MASTER dacpac to the master registry DB.
      2. Reads every active tenant's connection string from the master DB.
      3. Publishes the TENANT dacpac to each school DB.

    Requires: SqlPackage (dotnet tool 'microsoft.sqlpackage') on PATH, and
    Windows PowerShell (for System.Data.SqlClient) or a loadable SqlClient.

    Example:
      powershell -File ./scripts/Deploy-Databases.ps1 `
        -MasterConnectionString $env:MASTER_CONNECTION_STRING `
        -MasterDacpac ./MasterDatabase/bin/Release/SchoolManagement-MasterDb.dacpac `
        -TenantDacpac ./artifacts/SchoolManagement-TenantDb.dacpac
#>
param(
    [Parameter(Mandatory = $true)][string]$MasterConnectionString,
    [Parameter(Mandatory = $true)][string]$MasterDacpac,
    [Parameter(Mandatory = $true)][string]$TenantDacpac
)

$ErrorActionPreference = "Stop"

function Publish-Dacpac([string]$Dacpac, [string]$ConnectionString, [string]$Label) {
    Write-Host "==> Publishing '$Label'" -ForegroundColor Cyan
    sqlpackage /Action:Publish `
        /SourceFile:"$Dacpac" `
        /TargetConnectionString:"$ConnectionString" `
        /p:BlockOnPossibleDataLoss=true
    if ($LASTEXITCODE -ne 0) { throw "SqlPackage publish failed for '$Label'." }
}

function Get-ActiveTenants([string]$ConnectionString) {
    $tenants = @()
    $conn = New-Object System.Data.SqlClient.SqlConnection($ConnectionString)
    $conn.Open()
    try {
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = "SELECT SchoolName, ConnectionString FROM dbo.Tenants WHERE IsActive = 1;"
        $reader = $cmd.ExecuteReader()
        while ($reader.Read()) {
            $tenants += [pscustomobject]@{
                SchoolName       = $reader.GetString(0)
                ConnectionString = $reader.GetString(1)
            }
        }
        $reader.Close()
    }
    finally { $conn.Close() }
    return $tenants
}

# 1) Master DB ---------------------------------------------------------------
Publish-Dacpac -Dacpac $MasterDacpac -ConnectionString $MasterConnectionString -Label "Master registry DB"

# 2) Read active tenant connection strings -----------------------------------
Write-Host "==> Reading active tenants from master DB" -ForegroundColor Cyan
$tenants = Get-ActiveTenants -ConnectionString $MasterConnectionString
Write-Host "    Found $($tenants.Count) active tenant(s)."

# 3) Publish tenant dacpac to each school DB ---------------------------------
foreach ($t in $tenants) {
    Publish-Dacpac -Dacpac $TenantDacpac -ConnectionString $t.ConnectionString -Label "School: $($t.SchoolName)"
}

Write-Host "All deployments complete." -ForegroundColor Green
