# SecureShareAdmin

[![Status: Not Production Ready](https://img.shields.io/badge/status-NOT%20PRODUCTION%20READY-critical?style=for-the-badge)](./README.md)
[![Needs Work](https://img.shields.io/badge/deploy-DO%20NOT%20SHIP-red?style=for-the-badge)](./README.md)

> **Warning:** This codebase is under active modernization. It is **not** ready for production deployment.

---

Internal admin web app for **Secure Share** — manage external users, file-sharing zones, zone permissions, and admin notification preferences.

## Stack

- **.NET 10** / ASP.NET Core **Razor Pages**
- Bootstrap 5 + site theme (`wwwroot/css/site.css`)
- Dapper + Microsoft.Data.SqlClient
- Windows Authentication (Negotiate)
- Project / assembly: `SecureShareAdmin`
- Root namespace: `Snsc.SecureShareAdmin`

## Solution layout

- [`SecureShareAdmin.sln`](SecureShareAdmin.sln) — solution
- [`SecureShareAdmin/`](SecureShareAdmin/) — active web app
- [`_archive_SecureDataInterchange.Admin/`](_archive_SecureDataInterchange.Admin/) — archived ASP.NET WebForms / .NET Framework 3.5 source (reference only)

## Features

| Area | Routes |
|------|--------|
| Users | `/Users`, `/Users/Create`, `/Users/Edit/{externalUserId}` |
| Zones | `/Zones`, `/Zones/Create`, `/Zones/Edit/{id}` |
| Diagnostics | `/Diagnostics` (AMS id + zone counts) |

## Configuration

`appsettings.json`:

- `SnConfig:WebServiceUrl` — SNConfig ASMX endpoint
- `SnConfig:EnvironmentName` — environment key for SNConfig
Connection strings for `db:core` and `db:lmssystem` are loaded from SNConfig (same model as the legacy app).

## Run locally

```bash
dotnet run --project SecureShareAdmin
```

Or open `SecureShareAdmin.sln` in Visual Studio and set **SecureShareAdmin** as the startup project.

Windows Authentication is required (Negotiate). There is no anonymous or AMS-id override — if Windows auth fails or the login has no LMS `NTLoginName` match, the app fails with an explicit error.

## CI / packages

GitHub Actions (`.github/workflows/ci.yml`) builds on PRs and pushes.

- **Versioning:** [MinVer](https://github.com/adamralph/minver) from git tags prefixed with `v` (example: `git tag v1.0.0 && git push --tags`).
- **main:** publishes a self-contained **win-x64 Release** zip (same flags as `PublishAndDeployTo_STAGING.ps1`), uploads it as a workflow artifact, and refreshes the prerelease tag/release `main-latest`.
- **Tags `v*`:** creates a GitHub Release with the same zip attached.

## Status

Treat this as **work in progress**. Expect gaps versus the archived WebForms app until cutover is validated in a real environment.
