# Clean Architecture Web API + Next.js Monorepo

This repository hosts a .NET 9-style backend and a Next.js frontend in a single monorepo, following clean architecture principles.

## High-Level Structure

- `backend/` – .NET solution with Web API, AppHost, and supporting clean-architecture libraries
- `frontend/` – Next.js application (TypeScript, App Router)
- `docs/` – Architecture and design notes

## Workspace Layout

```text
clean_arch-webapi-starter/
├─ .github/
│  ├─ copilot-instructions.md
│  └─ workflows/
│     └─ deploy-iis.yml
├─ .vscode/
│  └─ tasks.json
├─ backend/
│  ├─ CleanArchWeb.sln
│  ├─ src/
│  │  ├─ Services/
│  │  │  ├─ CleanArchWeb.Api/
│  │  │  │  ├─ Controllers/
│  │  │  │  ├─ Properties/
│  │  │  │  │  ├─ launchSettings.json
│  │  │  │  │  └─ PublishProfiles/
│  │  │  │  │     └─ Folder.pubxml
│  │  │  │  └─ Program.cs
│  │  │  ├─ CleanArchWeb.Application/
│  │  │  ├─ CleanArchWeb.Domain/
│  │  │  └─ CleanArchWeb.Infrastructure/
│  │  └─ BackgroundServices/
│  └─ tests/
│     └─ CleanArchWeb.Api.Tests/
├─ frontend/
│  ├─ package.json
│  ├─ components.json
│  ├─ tailwind.config.ts
│  └─ src/
│     ├─ app/
│     │  ├─ page.tsx
│     │  └─ globals.css
│     └─ lib/
│        └─ utils.ts
├─ scripts/
│  └─ publish-iis.ps1
├─ docs/
└─ README.md
```

## Backend

- Solution: `backend/CleanArchWeb.sln`
- Projects:
  - `CleanArchWeb.Api` – ASP.NET Core Web API (`/backend/src/Services/CleanArchWeb.Api`)
  - `CleanArchWeb.Application` – application layer (`/backend/src/Services/CleanArchWeb.Application`)
  - `CleanArchWeb.Domain` – domain entities and logic (`/backend/src/Services/CleanArchWeb.Domain`)
  - `CleanArchWeb.Infrastructure` – infrastructure implementations (`/backend/src/Services/CleanArchWeb.Infrastructure`)
  - `AppHost` – simple console-based host placeholder (`/backend/src/AppHost`)

Sample endpoint: `GET /api/WeatherForecast`

To run the API:

```pwsh
cd ./backend
dotnet run --project .\src\Services\CleanArchWeb.Api\CleanArchWeb.Api.csproj
```

## Frontend

- App: `frontend/` (Next.js, TypeScript, App Router)
- Home page (`src/app/page.tsx`) fetches data from the backend `WeatherForecast` endpoint using `NEXT_PUBLIC_API_BASE_URL`.

To run the frontend dev server:

```pwsh
cd ./frontend
$env:NEXT_PUBLIC_API_BASE_URL="http://localhost:5277"
npm run dev
```

## Run Both (VS Code Tasks)

- Use `Terminal > Run Task...` and pick `dev: all` to start the API and the Next.js dev server together.
- The frontend task sets `NEXT_PUBLIC_API_BASE_URL` to `http://localhost:5277` to match the API's `launchSettings.json`.

## Local Publish to IIS

Publish the API using an MSBuild publish profile via the provided PowerShell script:

```pwsh
pwsh -NoProfile -ExecutionPolicy Bypass ./scripts/publish-iis.ps1 -ProjectPath "backend/src/Services/CleanArchWeb.Api/CleanArchWeb.Api.csproj" -ProfileName "IIS-WebDeploy"
```

If you don't have a Web Deploy profile yet, you can use the included FileSystem profile to publish to a local folder:

```pwsh
pwsh -NoProfile -ExecutionPolicy Bypass ./scripts/publish-iis.ps1 -ProfileName "Folder"
```

Optional credentials (if not embedded in the `.pubxml`):

```pwsh
pwsh -NoProfile -ExecutionPolicy Bypass ./scripts/publish-iis.ps1 -ProfileName "IIS-WebDeploy" -UserName "${env:IIS_PUBLISH_USER}" -Password "${env:IIS_PUBLISH_PASSWORD}"
```

Expected profile location: `backend/src/Services/CleanArchWeb.Api/Properties/PublishProfiles/<ProfileName>.pubxml`.

You can also run this via `Terminal > Run Task...` and select `publish: api to iis` (it will prompt for the profile name). Enter `Folder` for a local folder publish or your Web Deploy profile name when ready.

### Remote IIS (Web Deploy)

An example Web Deploy profile is included at:

- `backend/src/Services/CleanArchWeb.Api/Properties/PublishProfiles/IIS-WebDeploy.pubxml`

Before using it, edit these values in the `.pubxml`:

- `MSDeployServiceURL`: e.g., `your-server.example.com:8172/msdeploy.axd`
- `DeployIisAppPath`: e.g., `Default Web Site/CleanArchWeb.Api`
- Optionally set `UserName`, or pass `-UserName`/`-Password` to the script

Then publish with credentials (recommended to pass via parameters):

```pwsh
pwsh -NoProfile -ExecutionPolicy Bypass ./scripts/publish-iis.ps1 -ProfileName "IIS-WebDeploy" -UserName "${env:IIS_PUBLISH_USER}" -Password "${env:IIS_PUBLISH_PASSWORD}"
```

Prereqs on the server: IIS + Web Deploy enabled (WMSVC), Management Service running and allowing remote connections, correct site/app path provisioned, firewall open for port 8172, and an account with publish rights.

## CI/CD

GitHub Actions workflow at `.github/workflows/deploy-iis.yml` builds, tests, and publishes the API using the same publish profile. Configure repository secrets as needed (e.g., `IIS_PUBLISH_PASSWORD` if not included in the profile).

## IIS Remote Setup Guide

For a step-by-step IIS server setup for Web Deploy (remote deployment), see `docs/iis-remote-deploy-setup.md`.

## Next Steps

- Implement the .NET Aspire host and services in `backend/`.
- Implement the Next.js app in `frontend/`.
- Add tasks and CI as needed.
