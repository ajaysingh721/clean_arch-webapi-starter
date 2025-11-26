---
description: Copilot instructions tailored to this monorepo
applyTo: "**"
---

# Copilot Instructions (Project-Specific)

Big picture

- Monorepo with backend (.NET Clean Architecture) and frontend (Next.js App Router).
- Backend projects live under `backend/src/Services/*`; solution file `backend/CleanArchWeb.sln`.
- Frontend under `frontend/` with Tailwind v4 + shadcn/ui; uses `NEXT_PUBLIC_API_BASE_URL`.

Backend patterns

- Layers: `CleanArchWeb.Domain`, `CleanArchWeb.Application`, `CleanArchWeb.Infrastructure`, `CleanArchWeb.Api`.
- DI via extension methods: `Application/DependencyInjection.cs` and `Infrastructure/DependencyInjection.cs` are invoked in `Api/Program.cs`.
- Sample endpoint: `GET /api/WeatherForecast` in `CleanArchWeb.Api/Controllers/WeatherForecastController.cs` implemented via `IWeatherForecastService` with a random in-memory implementation in Infrastructure.
- Target `net10.0`; prefer file-scoped namespaces and modern C# features.

Frontend patterns

- App Router in `frontend/src/app/*`. Home page fetches forecasts from the API.
- Tailwind CSS v4 (no legacy `@tailwind` in CSS; uses imports) and `tailwindcss-animate` plugin.
- Avoid `next/dynamic({ ssr: false })` in Server Components; move client-only logic into a Client Component (`'use client'`).

Dev workflows (VS Code tasks & commands)

- Start both: `Terminal > Run Task...` → `dev: all` (runs API + Next.js). Frontend task sets `NEXT_PUBLIC_API_BASE_URL` to `http://localhost:5277`.
- API direct: `dotnet run --project backend/src/Services/CleanArchWeb.Api/CleanArchWeb.Api.csproj` (port `http://localhost:5277`, see `launchSettings.json`).
- Tests: from `backend/`, run `dotnet test` (tests under `backend/tests/CleanArchWeb.Api.Tests`).
- Lint frontend: from `frontend/`, run `npm run lint`.

Deployment

- Local publish: `scripts/publish-iis.ps1 -ProfileName Folder` (FileSystem to `%USERPROFILE%\cleanarchweb-publish\api\`).
- Remote IIS (Web Deploy): configure `backend/src/Services/CleanArchWeb.Api/Properties/PublishProfiles/IIS-WebDeploy.pubxml`, then run
  `./scripts/publish-iis.ps1 -ProfileName "IIS-WebDeploy" -UserName "$env:IIS_PUBLISH_USER" -Password "$env:IIS_PUBLISH_PASSWORD"`.
- CI: `.github/workflows/deploy-iis.yml` builds, tests, and publishes using the same profile.
- IIS setup guide: `docs/iis-remote-deploy-setup.md` (WMSVC, firewall 8172, delegation).

Conventions used here

- Clean Architecture boundaries: keep domain logic in Domain; Application orchestrates; Infrastructure wires external concerns; Api hosts.
- Tests name and focus: mirror controller/service behavior; keep unit tests focused and mock dependencies where applicable.
- Frontend fetches server-side when possible; client components only for interactivity; environment config through `NEXT_PUBLIC_*`.

Key references

- Backend DI: `backend/src/Services/CleanArchWeb.Application/DependencyInjection.cs`, `backend/src/Services/CleanArchWeb.Infrastructure/DependencyInjection.cs`.
- API entry: `backend/src/Services/CleanArchWeb.Api/Program.cs`.
- Example controller: `backend/src/Services/CleanArchWeb.Api/Controllers/WeatherForecastController.cs`.
- Frontend page: `frontend/src/app/page.tsx`; Tailwind config: `frontend/tailwind.config.ts`.
- Tasks: `.vscode/tasks.json`; Publish script: `scripts/publish-iis.ps1`.
