# IIS Remote Deployment Setup (Web Deploy)

This guide configures a Windows Server with IIS for remote deployments using Web Deploy (WMSVC) and the included `IIS-WebDeploy.pubxml` profile.

## 1) Install prerequisites

Run these PowerShell commands as Administrator on the server:

```pwsh
# IIS + Management Service + common features
Install-WindowsFeature Web-Server, Web-WebServer, Web-Common-Http, Web-Default-Doc, Web-Static-Content, Web-Http-Errors, Web-Http-Redirect, Web-Health, Web-Http-Logging, Web-Log-Libraries, Web-Request-Monitor, Web-Http-Tracing, Web-Performance, Web-Stat-Compression, Web-Dyn-Compression, Web-Security, Web-Filtering, Web-Basic-Auth, Web-Windows-Auth, Web-App-Dev, Web-Net-Ext45, Web-Asp-Net45, Web-ISAPI-Ext, Web-ISAPI-Filter, Web-Mgmt-Tools, Web-Mgmt-Service

# Optional: .NET Extensibility for classic modules (API uses Kestrel/Hosting Bundle)
Install-WindowsFeature Web-Mgmt-Compat -ErrorAction SilentlyContinue

# Start and enable Management Service (WMSVC)
Set-ItemProperty -Path HKLM:\SOFTWARE\Microsoft\WebManagement\Server -Name EnableRemoteManagement -Value 1
Start-Service WMSVC
Set-Service WMSVC -StartupType Automatic

# Open firewall for Web Deploy (8172)
New-NetFirewallRule -Name "WebDeploy8172" -DisplayName "Web Deploy (8172)" -Protocol TCP -LocalPort 8172 -Direction Inbound -Action Allow
```

Install the following manually (download from Microsoft):

- Web Deploy 4.0 (Server)
- ASP.NET Core Hosting Bundle (matching your .NET runtime, e.g., .NET 10)

Reboot the server after installing the Hosting Bundle.

## 2) Enable remote connections and certificate

In IIS Manager (inetmgr):

1. Select the server node → Management Service.
2. Check "Enable remote connections".
3. Choose a server certificate (create a self-signed cert if needed).
4. Ensure service is started.

Self-signed certs are fine for testing; the provided publish profile sets `AllowUntrustedCertificate=True`.

## 3) Create the IIS site or application

Decide your target path (examples):

- Site: `Default Web Site`
- App: `Default Web Site/CleanArchWeb.Api`

Then either:

- Create an application under your site pointing to a folder (e.g., `C:\inetpub\wwwroot\CleanArchWeb.Api`), or
- Deploy once to create it (with appropriate delegation—see below), then adjust bindings/app pool as needed.

Recommended App Pool settings for ASP.NET Core apps:

- .NET CLR version: No Managed Code
- Managed pipeline: Integrated

## 4) Choose authentication model for deployment

You can deploy as:

- Local/Domain Windows account (common)
- IIS Manager User (non-admin scenario)

### A) Windows account (simpler)

1. Create or choose a local/domain user (e.g., `server\deployuser`).
2. Grant NTFS Modify permissions on the site folder (e.g., `C:\inetpub\wwwroot\CleanArchWeb.Api`).
3. Use the account in the publish script with `-UserName`/`-Password`.

### B) IIS Manager User (non-admin)

1. In IIS Manager → server node → IIS Manager Users → Add…
2. On the site → IIS Manager Permissions → Allow the user.
3. Configure Delegation: server node → Management Service Delegation → Add Rule…
   - Providers: `contentPath, iisApp`
   - Operations: `createApp, sync`
   - Path: your site/app path
   - User: your IIS Manager User
4. Grant NTFS Modify permissions on the physical folder.

## 5) Configure the publish profile

Edit `backend/src/Services/CleanArchWeb.Api/Properties/PublishProfiles/IIS-WebDeploy.pubxml`:

- `MSDeployServiceURL`: `your-server.example.com:8172/msdeploy.axd`
- `DeployIisAppPath`: `Default Web Site/CleanArchWeb.Api`
- Leave `UserName` blank and pass credentials via the script, or set it in the profile.

## 6) Test connectivity

From your dev machine:

```pwsh
# Quick port check
Test-NetConnection your-server.example.com -Port 8172
```

If port is closed, verify firewall/network rules and that WMSVC is running.

## 7) Deploy from this repo

Use the script with Web Deploy profile and credentials:

```pwsh
pwsh -NoProfile -ExecutionPolicy Bypass ./scripts/publish-iis.ps1 -ProfileName "IIS-WebDeploy" -UserName "${env:IIS_PUBLISH_USER}" -Password "${env:IIS_PUBLISH_PASSWORD}"
```

The CI workflow `.github/workflows/deploy-iis.yml` can do the same on push; set secrets for credentials if your `.pubxml` does not contain them.

## 8) Post-deploy checks

- Browse the site/app URL to confirm it’s serving.
- Check Event Viewer → Windows Logs → Application for ASP.NET Core Module logs.
- Ensure the ASP.NET Core Hosting Bundle is installed and matches the deployed runtime.
- Review file permissions if you see access errors.

## Troubleshooting

- 403/401 during deploy: verify credentials and delegation rules.
- 404 after deploy: confirm `DeployIisAppPath` and that the app was created correctly.
- 502.5 Process Failure: check Hosting Bundle, application pool identity, and application logs.
- SSL/Cert warnings: use a trusted cert or keep `AllowUntrustedCertificate=True` for test environments.
