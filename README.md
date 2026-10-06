# School Management System — Multitenant

A multitenant school management platform. One **master registry database** stores
each school's domain + connection string; the API resolves the current school from
the request domain and routes all data access to that school's own database.

```
┌─────────────────────┐     X-Tenant-Domain: greenwood.localhost
│  Angular 21 (SPA)   │ ───────────────────────────────────────────┐
│  Tailwind, JWT auth │                                             │
└─────────────────────┘                                            ▼
                                                        ┌──────────────────────┐
                                                        │  ASP.NET Core 10 API │
                                                        │  JWT + role auth     │
                                                        │  Dapper + stored     │
                                                        │  procedures          │
                                                        └──────────┬───────────┘
                                         resolve by domain         │ per-request
                                   ┌───────────────────────────────┤ connection
                                   ▼                               ▼
                        ┌────────────────────┐        ┌──────────────────────────┐
                        │  MASTER registry   │        │  School (tenant) DBs      │
                        │  dbo.Tenants       │        │  Greenwood, Riverside, …  │
                        └────────────────────┘        └──────────────────────────┘
```

## Repositories / folders

| Path | What |
|------|------|
| `G:\SchoolManagementback` | **This repo.** ASP.NET Core 10 Web API + the **master DB** SQL project (`MasterDatabase/`). |
| `G:\SchoolManagementDataBase` | The **tenant (school) DB** SQL project — schema deployed to every school database. |
| `G:\SchoolManagementFrontend` | Angular 21 + Tailwind SPA. |

## Backend layout

```
src/
  SchoolManagement.Core            Domain models, DTOs, contracts (no dependencies)
  SchoolManagement.Infrastructure  Dapper repos, tenant resolution, JWT, Cloudinary
  SchoolManagement.Api             Controllers, middleware, Swagger, DI wiring
MasterDatabase/                    Master registry SQL project (Tenants + SPs)
scripts/                           Local DB setup + DACPAC deploy scripts
.github/workflows/ci.yml           Build/test API + deploy master DB
```

### Key pieces

- **Tenant resolution** — `TenantResolutionMiddleware` reads `X-Tenant-Domain`
  (configurable), looks it up in the master registry, and populates a scoped
  `ITenantContext`. Repositories open the tenant's connection via
  `IDbConnectionFactory.CreateTenantConnectionAsync()`.
- **Auth** — `POST /api/auth/login` validates against the tenant's `Users` table
  (BCrypt) and issues a JWT carrying the role. Endpoints use `[Authorize(Roles=…)]`.
- **Roles** — Principal, HeadMaster, Teacher, Accountant, Student.
- **Media** — profile pics & documents upload to Cloudinary (`/api/media/upload`),
  organised per tenant.
- **Branding** — each school's login page/theme is editable by the Principal
  (`/api/school-settings`) and read pre-login via `/api/branding`.

## Run locally

Prerequisites: .NET 10 SDK, Node 22 + Angular CLI 21, SQL Server (Express/LocalDB).

### 1. Create the databases (master + a demo school) and seed data

```bash
pwsh ./scripts/setup-local-db.ps1 -ServerInstance ".\SQLEXPRESS"
```

This creates `SchoolManagement_Master` and `SchoolManagement_Greenwood`, applies the
schema + stored procedures + seed, and registers the demo tenant
`greenwood.localhost`. Bootstrap login: **principal / Principal@123**.

### 2. Run the API

```bash
cd src/SchoolManagement.Api
dotnet run
```

Swagger UI: `http://localhost:<port>/swagger`. In Swagger, set the
**X-Tenant-Domain** value to `greenwood.localhost`, then call `/api/auth/login`.

### 3. Run the frontend

```bash
cd G:\SchoolManagementFrontend
npm install --legacy-peer-deps   # see note below
ng serve
```

App: `http://localhost:4200` → login with **principal / Principal@123**.

> **npm note:** npm 10.9 has an arborist bug resolving an optional peer dep
> (`canvas` via jsdom). Use `npm install --legacy-peer-deps`, or upgrade npm.

## Deploying schema (DACPAC)

Both SQL projects are SDK-style (`Microsoft.Build.Sql`) and build with
`dotnet build`, producing a `.dacpac`. Deployment uses `sqlpackage`:

- **Master DB** — deployed by `.github/workflows/ci.yml`.
- **All tenant DBs** — deployed by the tenant repo's
  `.github/workflows/deploy-tenant-db.yml`: it reads every active tenant's
  connection string from the master DB and publishes the tenant DACPAC to each.
  The DACPAC includes the post-deployment seed script.

Required GitHub secret: `MASTER_CONNECTION_STRING`.

Manual deploy: `scripts/Deploy-Databases.ps1`.

## Registering a new school (tenant)

`POST /api/tenants` with header `X-Admin-Key: <Admin:ApiKey>`:

```json
{
  "schoolName": "Riverside Academy",
  "domain": "riverside.myschools.com",
  "databaseName": "SchoolManagement_Riverside",
  "connectionString": "Server=...;Database=SchoolManagement_Riverside;..."
}
```

Then run the tenant deploy pipeline so the new DB gets the schema + seed.

## Status / roadmap

**Done (foundation):** multitenancy, master registry, tenant resolution, JWT +
role auth, Swagger, Cloudinary media, Classes (list/create), per-school branding,
full tenant schema (classes, sections, subjects, employees/teachers, students,
timetable, fees, documents, interests, bus routes, performance tests/results),
CI/CD, Angular shell with role-based nav + guards.

**Next increments:** teacher hiring + class-incharge assignment, student admission
(previous-school details, documents/photos to Cloudinary, sections, interests, bus
service), timetable/period scheduling, fees + pending-fee tracking, subject-wise
performance/results, student portal (results/profile).
