# SecureShareAdmin

[Status: Not Production Ready](./README.md)
[Needs Work](./README.md)

> **Warning:** This codebase is under active recovery/refactor. It is **not** ready for production deployment and still needs substantial work before it can be shipped.

---

Internal admin web app for **Secure Data Interchange (SecureShare)** — manage external users, file-sharing zones, zone permissions, and related admin tasks.

## Stack

- ASP.NET WebForms
- .NET Framework 3.5
- Solution: `SecureShareAdmin.sln`
- Main project: `SecureDataInterchange.Admin/SecureDataInterchange.Admin.UI`

## What’s in here

- View / add / edit users
- View / add / edit zones
- Zone user permissions and email notification opt-in
- AMS-authenticated admin UI (via site master / security principal)

## Local development

1. Open `SecureShareAdmin.sln` in Visual Studio.
2. Set `SecureDataInterchange.Admin.UI` as the startup project.
3. Configure connection strings and app settings in `web.config` for your environment.
4. Build and run against IIS / IIS Express as appropriate for your machine.

## Status

Treat this repo as **work in progress**. Expect incomplete features, legacy patterns, and gaps before any production release.